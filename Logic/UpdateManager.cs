using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox.ModAPI;

using VRage.Collections;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;

using SEGarden.Logging;
using SEGarden.Threading;

namespace SEGarden.Logic {

    /// <summary>
    /// <para>Completely circumvents MyGameLogicComponent to avoid conflicts, and 
    /// offers a bit more flexibility.</para>
    /// <para>Will send updates after creating object until object is closing.</para>
    /// <para>Creation of script objects is delayed until MyAPIGateway fields are filled.</para>
    /// <para>If an update script throws an exception, it will stop receiving updates.</para>
    /// </summary>
    /// <remarks>
    /// There doesn't seem to be any benefit to register before vs during vs after update
    /// So no harm in combining here.
    /// And after is one character shorter than before.
    /// <para>
    /// An adaptation of Rynchodon's excellent UpdateManager for autopilot, 
    /// github.com/Rynchodon/autopilot
    /// </para>
    /// <para>Comparision to MyGameLogicComponent</para>
    /// <para>    Disadvantages of MyGameLogicComponent:</para>
    /// <para>        NeedsUpdate can be changed by the game after you set it, so you have to work around that. i.e. For remotes it is set to NONE and UpdatingStopped() never gets called.</para>
    /// <para>        Scripts can get created before MyAPIGateway fields are filled, which can be a serious problem for initializing.</para>
    /// <para> </para>
    /// <para>    Advantages of UpdateManager:</para>
    /// <para>        Scripts can be registered conditionally. MyGameLogicComponent now supports subtypes but UpdateManager can technically do any condition.</para>
    /// <para>        Elegant handling of Exception thrown by script.</para>
    /// <para>        You don't have to create a new object for every entity if you don't need one.</para>
    /// <para>        You can set any update frequency you like without having to create a counter.</para>
    /// <para>        UpdateManager supports characters and could be improved to include any entity.</para>
    /// </remarks>
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class UpdateManager : MySessionComponentBase {

        #region Subclasses

        // Holds the update action for a component as well as a reference to the
        // component itself. Useful for storing updates by frequency but still
        // being able to reference their component
        private struct ComponentUpdate {
            public SessionComponent Component;
            public Action UpdateAction;
            public bool TerminatesOnError;
        }

        #endregion
        #region Static Fields

        private static Logger Log = new Logger("UpdateManager");
        private static UpdateManager Instance;

        // Holds work to be done from public methods called outside of our update
        private static MyConcurrentQueue<Action> UpdateActions =
            new MyConcurrentQueue<Action>(8);

        // Definitely looking for a better way to do this
        private static bool SEGardenAdded = false;

        #endregion
        #region Instance Fields

        private RunStatus Status = RunStatus.NotInitialized;
        private RunLocation RunningOn = RunLocation.Unknown;
        private ulong Frame;

        // Used to manage initialization and termination
        // Includes both SessionComponents and EntityComponents
        private List<SessionComponent> RegisteredComponents =
            new List<SessionComponent>();

        // Used to manage updates
        private Dictionary<uint, List<ComponentUpdate>> RegisteredUpdates =
            new Dictionary<uint, List<ComponentUpdate>>();

        // Used to create new entity components for each matching added entity
        // We store these as actions instead of functions to let the action take
        // care of all registry for updates, init, etc. for flexibility of use
        private Dictionary<MyObjectBuilderType, List<Action<IMyEntity>>> EntityComponentConstructors =
            new Dictionary<MyObjectBuilderType, List<Action<IMyEntity>>>();

        // temporary helpers for update to save it from re-allocating every frame
        private HashSet<SessionComponent> ComponentsToTerminate =
            new HashSet<SessionComponent>();
        private HashSet<SessionComponent> ComponentsToDeregister =
            new HashSet<SessionComponent>();

        #endregion
        #region Field Access Helpers

        /// <summary>
        /// Gets the constructor list mapped to a MyObjectBuilderType
        /// </summary>
        private List<Action<IMyEntity>> GetEntityComponentConstructors(MyObjectBuilderType objBuildType) {
            List<Action<IMyEntity>> scripts;
            if (!EntityComponentConstructors.TryGetValue(objBuildType, out scripts)) {
                scripts = new List<Action<IMyEntity>>();
                EntityComponentConstructors.Add(objBuildType, scripts);
            }
            
            return scripts;
        }

        /// <summary>
        /// Gets the update list mapped to a frequency
        /// </summary>
        private List<ComponentUpdate> GetUpdateList(uint frequency) {
            //Log.Trace("Getting it on", "GetUpdateList");
            List<ComponentUpdate> updates;
            if (!RegisteredUpdates.TryGetValue(frequency, out updates)) {
                updates = new List<ComponentUpdate>();
                RegisteredUpdates.Add(frequency, updates);
            }
            //Log.Trace("Finished getting it on", "GetUpdateList");
            return updates;
        }

        #endregion
        #region Public Registry Methods

        public static void RegisterSessionComponent(
            SessionComponent component,
            RunLocation targetLocation = RunLocation.Any,
            bool terminateOnError = false
            ) 
        {
            UpdateActions.Enqueue(() => {
                Instance.RegisterComponentInternal(component, targetLocation, terminateOnError);
            });
        }


        // Helper for those who prefer to handle the creation logic directly
        /*
        public static void RegisterInstantiatedEntityComponent(
            EntityComponent component,
            RunLocation targetLocation = RunLocation.Any,
            bool terminateOnError = false) 
        {
            RegisterInstantiatedComponent(component, targetLocation, terminateOnError);
        }

        public static void RegisterEntityComponentConstructor(
            Action<IMyEntity> constructorAction,
            MyObjectBuilderType entityType,
            RunLocation targetLocation = RunLocation.Any,
            String[] subTypeNames = null,
            bool terminateOnError = false) 
        {

            Action<IMyEntity> fullConstructor = ((e) => {

                if (subTypeNames != null &&
                    !subTypeNames.Contains(e.GetObjectBuilder().SubtypeName))
                    return;
                
                EntityComponent c = constructor.Invoke(e);

                RegisterInstantiatedComponent(c, targetLocation, terminateOnError);
                
            });

            UpdateActions.Enqueue(() => {
                Instance.RegisterEntityComponentConstructorInternal(
                    fullConstructor, entityType, targetLocation);
            });
         * 
         public static void RegisterUpdate(uint frequency, Action update, 
            SessionComponent component) 
        {
            UpdateActions.Enqueue(() => {
                Instance.RegisterUpdateForComponent(frequency, update, component);
            });
        }
        }
        */

        public static void RegisterEntityComponent(
            Func<IMyEntity, EntityComponent> constructor,
            MyObjectBuilderType entityType,
            RunLocation targetLocation = RunLocation.Any,
            String[] subTypeNames = null,
            bool terminateOnError = false,
            bool allowTransparent = false) 
        {


            // This will be run during update once an entity with the right 
            // entity Type has been added, if we're running on  targetLocation
            // This gets run from within an UpdateAction, so don't enqueue
            // again or the game will freeze. :)
            Action<IMyEntity> fullConstructor = ((e) => {

                // Check transparent
                if (e.Transparent && !allowTransparent) return;

                // Check subtypes
                if (subTypeNames != null) {
                    MyObjectBuilder_EntityBase builder = e.GetObjectBuilder();

                    if (builder == null) {
                        Log.Error("Got null builder for entity", "EntityComponentCtr");
                    }

                    if (!subTypeNames.Contains(builder.SubtypeName))
                        return;                    
                }


                // Run constructor
                EntityComponent c;
                //Log.Trace("Inside constructor wrapper, invoking " + e.EntityId, "EntityComponentCtr");
                try { c = constructor.Invoke(e); }
                catch (Exception e2) { 
                    Log.Error("Error invoking constructor: " + e2, "EntityComponentCtr");
                    return;
                }

                if (c == null) {
                    Log.Warning("Return null ctr, aborting ", "EntityComponentCtr");
                    return;
                }

                // Register instantiated component
                Instance.RegisterComponentInternal(c, targetLocation, terminateOnError);
            
            });

            // Register the above constructor to run on entity add
            UpdateActions.Enqueue(() => {
                Instance.RegisterEntityComponentConstructorInternal(
                    fullConstructor, entityType, targetLocation);
            });
    
        }

        #endregion
        #region Component Registration

        private bool ShouldRunForRegistered(RunLocation registered) {
            return registered == RunLocation.Any ||
                registered == RunningOn ||
                (RunningOn == RunLocation.Singleplayer && 
                    (registered == RunLocation.Client || 
                    registered == RunLocation.Server));
        }

        private void RegisterComponentInternal(
            SessionComponent component,
            RunLocation targetLocation = RunLocation.Any,
            bool terminateOnError = false) 
        {
            if (ShouldRunForRegistered(targetLocation)) {
                //Log.Debug("Registering component " + component.ComponentName, 
                //    "RegisterComponentInternal");
                InitializeComponent(component, terminateOnError);
                RegisteredComponents.Add(component);
            }
        }

        private void RegisterEntityComponentConstructorInternal(
            Action<IMyEntity> constructor,
            MyObjectBuilderType entityType,
            RunLocation targetLocation = RunLocation.Any) 
        {
            if (!ShouldRunForRegistered(targetLocation))
                return;

            RememberEntityComponentConstructor(entityType, constructor);

            // init entity components with existing entities
            //Log.Trace("Running newly saved constructor on existing entities.", 
            //    "RememberEntityComponentConstructor");
            HashSet<IMyEntity> allEntities = new HashSet<IMyEntity>();
            MyAPIGateway.Entities.GetEntities(allEntities);
            List<IMyEntity> targetEntities = allEntities.Where(
                (e) => e.GetObjectBuilder().GetType() == entityType).
                ToList();

            foreach (IMyEntity entity in targetEntities)
                RunEntityComponentConstructorOnEntity(constructor, entity);
        }

        private void RememberEntityComponentConstructor(
            MyObjectBuilderType entityType, Action<IMyEntity> constructor) 
        {
            //Log.Trace("Saving entity component constructor for " + entityType + 
            //    " to dictionary.", "RememberEntityComponentConstructor");

            GetEntityComponentConstructors(entityType).Add(constructor);
        }

        private void RunAllEntityComponentConstructorsOnAdded(IMyEntity entity) {
            Type entityBuilderType = entity.GetObjectBuilder().GetType();

            List<Action<IMyEntity>> registerActions = 
                GetEntityComponentConstructors(entityBuilderType);

            //Log.Trace("Running " + registerActions.Count + " saved constructors " +
            //    " for entity " + entity.EntityId + " of type " + entityBuilderType,
            //    "RunAllEntityComponentConstructorsOnAdded");

            foreach (Action<IMyEntity> constructor in registerActions) {
                RunEntityComponentConstructorOnEntity(constructor, entity);
            }
        }

        /*
        private void TerminateAllEntityComponentsOnRemoved(IMyEntity entity) {
            if (entity == null) {
                Log.Warning("Received null entity", "TerminateAllEntityComponentsOnRemoved");
                return;
            }

            MyObjectBuilder_EntityBase builder;

            try {
                builder = entity.GetObjectBuilder();
            } 
            catch (Exception e) {
                Log.Error("Error getting object builder for " + entity.ToString() + " : " + e, "TerminateAllEntityComponentsOnRemoved");
                return;
            }

            if (builder == null) {
                Log.Warning("Received null builder for entity " + entity.EntityId, "TerminateAllEntityComponentsOnRemoved");
                return;
            }

            Type entityBuilderType = builder.GetType();

            //Log.Trace("Terminating components for entity " + entity.EntityId, 
            //    "TerminateAllEntityComponentsOnRemoved");
            //    " for entity " + entity.EntityId + " of type " + entityBuilderType,
            //    "RunAllEntityComponentConstructorsOnAdded");

            foreach (SessionComponent c in Enumerable.Reverse(RegisteredComponents)) {
                EntityComponent entityComp = c as EntityComponent;

                if (entityComp != null && entityComp.EntityId == entity.EntityId &&
                    entityComp.IsRunning) {
                    TerminateComponent(c);
                }
            }
        }
        */

        private void RunEntityComponentConstructorOnEntity(Action<IMyEntity> constructor, IMyEntity entity) {
            //Log.Trace("Running constructor on entity " + entity.EntityId,
            //    "RunEntityComponentConstructorOnAdded");

            try { constructor.Invoke(entity); }
            catch (Exception e) {
                Log.Error("Exception in entity constructor: " + e,
                    "RunEntityComponentConstructorOnAdded");
            }
        }

        #endregion
        #region Component state changes

        private void InitializeComponent(SessionComponent c, bool terminateOnError = false) {
            if (c.Status == RunStatus.NotInitialized) {
                
                try {
                    //Log.Debug("Initializing component " + c.ComponentName, "InitializeComponent");
                    c.Initialize();
                    RegisterUpdatesForComponent(c, terminateOnError);
                    c.Status = RunStatus.Running;
                }
                catch (Exception e) {
                    Log.Error("Error Initializing component " + c.ComponentName + "  : " + e, "InitializeComponent");
                    c.Status = RunStatus.Terminated;
                }
            }
            else {
                Log.Warning("Tried to initialize already initialized " + c.ComponentName, "InitializeComponent");
            }
        }

        private void TerminateComponent(SessionComponent c) {
            if (c.Status == RunStatus.Terminated) {
                Log.Warning("Tried to terminate already terminated " + c.ComponentName, "TerminateComponent");
                return;
            }

            Log.Debug("Terminating component", "TerminateComponent");

            try { c.Terminate(); }
            catch (Exception e) {
                Log.Error("Error terminating component " + c.ComponentName + " : " + e, "TerminateComponent");
            }
            c.Status = RunStatus.Terminated;
            DeregisterUpdatesForComponent(c);
            //Log.Trace("Finished terminating component " + c.ComponentName, "TerminateComponent");
        }

        private void RegisterUpdatesForComponent(
            SessionComponent c, 
            bool terminateOnError = false) 
        {
            //Log.Trace("Start RegisterUpdatesForComponent", "RegisterUpdatesForComponent");
            //Log.Trace("Registering component " + c.ComponentName + " for updates.", "RegisterUpdatesForComponent");
            Dictionary<uint, Action> componentUpdates;

            try {componentUpdates = c.UpdateActions; }
            catch (Exception e) {
                Log.Error("Error getting UpdateActions, ensure you have a " +
                    "valid dictionary: " + e, "RegisterUpdatesForComponent");
                return;
            }
            if (componentUpdates == null) return;

            foreach (KeyValuePair<uint, Action> kvp in componentUpdates) {
                RegisterUpdateForComponent(kvp.Key, kvp.Value, c, terminateOnError);
            }
            //Log.Trace("End RegisterUpdatesForComponent", "RegisterUpdatesForComponent");
        }

        private void RegisterUpdateForComponent(
            uint frequency, 
            Action update, 
            SessionComponent c, 
            bool terminateOnError = false) 
        {
            //Log.Trace("Start RegisterUpdateForComponent", "RegisterUpdateForComponent");
            if (update == null) return;

            //Log.Trace("Registering update action for component " + c.ComponentName + " with frequency " + frequency, "RegisterUpdateForComponent");
            GetUpdateList(frequency).Add(new ComponentUpdate {
                UpdateAction = update,
                Component = c,
                TerminatesOnError = terminateOnError,
            });
            //Log.Trace("End RegisterUpdateForComponent", "RegisterUpdateForComponent");
        }

        private void DeregisterUpdatesForComponent(SessionComponent c) {
            List<uint> updateFeqs = new List<uint>(RegisteredUpdates.Keys);

            foreach (uint frequency in updateFeqs) {
                RegisteredUpdates[frequency] = RegisteredUpdates[frequency].
                    Where((x) => x.Component != c).
                    ToList();

                if (RegisteredUpdates[frequency].Count == 0) {
                    RegisteredUpdates.Remove(frequency);
                    continue;
                }
            }
        }

        #endregion
        #region SE Events

        /// <summary>
        /// Initializes if needed, issues updates.
        /// </summary>
        public override void UpdateAfterSimulation() {
            // A helper for maintaining a lock on the main thread
            //MainLock.MainThread_ReleaseExclusive();

            switch (Status) {
                case RunStatus.NotInitialized:
                    Initialize();
                    return;
                case RunStatus.Terminated:
                    return;
            }

            Update();

            //MainLock.MainThread_AcquireExclusive();
        }

        private void Entities_OnEntityAdd(IMyEntity entity) {
            //Log.Trace("Entity " + entity.EntityId + " added to game", "Entities_OnEntityAdd");
            //.Save helps us sort out all the stuff that doesn't have OBs, like bullets
            if (entity.Save && !entity.Transparent) {
                UpdateActions.Enqueue(() => {
                    //Log.Trace("Registering entity from OnEntityAdded", "action");
                    RunAllEntityComponentConstructorsOnAdded(entity);
                });
            }
        }

        /*
        private void Entities_OnEntityRemove(IMyEntity entity) {
            //Log.Trace("Entity " + entity.EntityId + " removed from game", "Entities_OnEntityRemove");
            //.Save helps us sort out all the stuff that doesn't have OBs, like bullets
            if (entity.Save) {
                UpdateActions.Enqueue(() => {
                    TerminateAllEntityComponentsOnRemoved(entity);
                });
            }
        }
        */

        protected override void UnloadData() {
            //Log.Trace("Update Manager unloading data", "UnloadData");
            base.UnloadData();
            Terminate();
        }

        public override void UpdatingStopped() {
            //Log.Trace("Update Manager component stopped", "UpdatingStopped");
            base.UnloadData();
            Terminate();
        }

        #endregion
        #region Internal Initialize/Update/Terminate

        private void Initialize() {
            Log.Debug("Begin Initialize Update Manager", "Initialize");

            if (MyAPIGateway.CubeBuilder == null || 
                MyAPIGateway.Entities == null || 
                MyAPIGateway.Multiplayer == null || 
                MyAPIGateway.Parallel == null || 
                MyAPIGateway.Players == null || 
                MyAPIGateway.Session == null || 
                MyAPIGateway.TerminalActionsHelper == null || 
                MyAPIGateway.Utilities == null)
                return;

            if (MyAPIGateway.Multiplayer.MultiplayerActive) {
                if (MyAPIGateway.Multiplayer.IsServer)
                    RunningOn = RunLocation.Server;
                else
                    RunningOn = RunLocation.Client;
            } else {
                RunningOn = RunLocation.Singleplayer;
            }

            MyAPIGateway.Entities.OnEntityAdd += Entities_OnEntityAdd;
            // Removal termination seems to be handled best by MyEntity.OnClose

            Instance = this;
            Status = RunStatus.Running;

            // Register SE Garden on init, before anything else
            RegisterComponentInternal(new GardenGateway());

            Log.Debug("Finished Initializing Update Manager", "Initialize");
        }

        private void Update() {

            // Outside functions and entity Add force us to construct and
            // initialize new components at any time. We queue that to do
            // more safely here once we're fully initialized.
            if (UpdateActions.Count > 0) {
                Log.Debug("Running " + UpdateActions.Count + " update actions", "Update");
                try {
                    Action updateAction;
                    while (UpdateActions.TryDequeue(out updateAction))
                        updateAction.Invoke();
                }
                catch (Exception e) {
                    Log.Error("Exception in AddRemoveActions: " + e, "Update");
                }
            }

            // Perform registered updates and catch components that shouldn't run
            foreach (KeyValuePair<uint, List<ComponentUpdate>> kvp in RegisteredUpdates) {
                if (Frame % kvp.Key == 0) {
                    foreach (ComponentUpdate item in kvp.Value) {
                        //Log.Trace("Running components for freq " + kvp.Key + " frame " + Frame, "Update");

                        if (item.Component.Status != RunStatus.Running) {
                            Log.Debug("Found terminated component in update list, " +
                                "removing", "Update");

                            if (!ComponentsToDeregister.Contains(item.Component))
                                ComponentsToDeregister.Add(item.Component);

                            continue;
                        }

                        try {
                            item.UpdateAction.Invoke();
                        }
                        catch (Exception e) {
                            Log.Error("Script threw exception: " + e, "Update");

                            if (item.TerminatesOnError) {
                                if (!ComponentsToTerminate.Contains(item.Component)) {
                                    ComponentsToTerminate.Add(item.Component);
                                }
                            }
                        }

                        //Log.Trace("Finished running components for freq " + kvp.Key + " frame " + Frame, "Update");
                    }
                }
            }

            // Terminate components caught misbehaving
            if (ComponentsToDeregister.Any()) {
                foreach (SessionComponent c in ComponentsToDeregister)
                    DeregisterUpdatesForComponent(c);

                ComponentsToDeregister.Clear();
            }

            // Note this both stops them AND deregisters them
            if (ComponentsToTerminate.Any()) {
                foreach (SessionComponent c in ComponentsToTerminate)
                    TerminateComponent(c);

                ComponentsToTerminate.Clear();
            }

            ++Frame;
        }

        private void Terminate() {
            if (Status != RunStatus.Running) {
                Log.Warning("Trying to terminate already terminated", "Terminate");
                return;
            }

            Log.Debug("Begin Terminate Update Manager", "Terminate");
            // First initialized = last terminated, this helps keep GardenGateway
            // at the end so we have logging until the very last minute.
            foreach (SessionComponent c in Enumerable.Reverse(RegisteredComponents)) {
                TerminateComponent(c);
            }

            MyAPIGateway.Entities.OnEntityAdd -= Entities_OnEntityAdd;
            // Removal termination seems to be handled best by MyEntity.OnClose

            Status = RunStatus.Terminated;
            Log.Debug("Finished Terminating Update Manager", "Terminate");
        }

        #endregion


        /* At some point I'd like to let people just pass us types instead of constructors
       public static object GetNewObject(Type t) {
           try {
               return t.GetConstructor(new Type[] { }).Invoke(new object[] { });
           }
           catch {
               return null;
           }
       }
       */
    }
}

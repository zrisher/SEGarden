using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using Sandbox.ModAPI;
using VRage.ModAPI;

using SEGarden.Extensions;
using SEGarden.Logging;

namespace SEGarden.Logic {

    public abstract class EntityComponent : SessionComponent {

        protected IMyEntity Entity;

        protected Logger Log;

        public EntityComponent(IMyEntity entity) : base() {
            Entity = entity;
            Log = new Logger("SEGarden.Logic.EntityComponent", (() => EntityId.ToString()));
            //Log.Trace("Finished EntityComponent ctr", "ctr");
        }

        public EntityComponent(VRage.ByteStream stream) : base() {
            long entityId = stream.getLong();
            Entity = MyAPIGateway.Entities.GetEntityById(entityId);
            Log = new Logger("SEGarden.Logic.EntityComponent", (() => EntityId.ToString()));
            //Log.Trace("Finished EntityComponent deserialize", "ctr");
        }

        public long EntityId { get { return Entity.EntityId; } }
        // This can change over time, no events, so get it instead of storing
        public string DisplayName { get { return Entity.DisplayName;  } }

        public override void Initialize() {
            //Log.Trace("Initialize entity component abstract", "Initialize");
            Entity.OnMarkForClose += OnClose; // should we use closing or close instead?
            base.Initialize();
        }

        private void OnClose(IMyEntity e) {
            //Log.Trace("Attached entity is closing", "OnClose");
            try { TerminateConditional(); }
            catch (Exception ex) {
                Log.Error("Error terminating on close: " + ex, "OnClose");
            }

        }

        public override void Terminate() {
            //Log.Trace("Terminate entity component abstract", "Terminate");
            Entity.OnMarkForClose -= OnClose;
            base.Terminate();
        }

        public virtual void AddToByteStream(VRage.ByteStream stream) {
            stream.addLong(EntityId);
        }

    }

}

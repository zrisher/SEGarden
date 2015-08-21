using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Files;
using SEGarden.Chat;
using SEGarden.Messaging;

using SEGarden.Logging;
using SEGarden.Logic;

namespace SEGarden {

    ///<summary>
    /// Public MyAPIGateway helpers
    /// </summary>
    /// <remarks>
    /// TODO: Should we further hide these in their own static classes
    /// so we can stop from exposing their Initialize and Terminate methods?
    /// 
    /// We could make them their own session components, problem solved
    /// 
    /// On the other hand, it's nice to have them all centralized for access
    /// And the fact that they depend on initialization is nicely apparent
    /// But ComponentManager needs to be accessed the other way.
    /// Logic might be clearer to just have them all work like that
    /// Or we could provide a layer over component manager to hide its dangerous
    /// public methods
    /// 
    /// NVM, no matter what initialize and terminate for the components will have
    /// to be public. There's really no way around this. 
    ///
    /// TODO: Provide thread-safety for anything accessed through here
    /// 
    /// IMPORTANT: Files needs to be constructed first, initialized first, and 
    /// terminated last, because all logging depends on it.
    /// </remarks>
    [Sandbox.Common.MySessionComponentDescriptor(Sandbox.Common.MyUpdateOrder.BeforeSimulation)]
    class GardenGateway : GardenSession {

        public GardenGateway() : base(RunLocation.Any, false) { }

        private static Logger Log = new Logger("SEGarden.GardenGateway");

        public static FileManager Files { get; private set; }
        public static ChatManager Commands { get; private set; }
        public static MessageManager Messages { get; private set; }
        public static bool Initialized { get; private set; }
        public static RunLocation RunningOn { get; private set; }

        /// <summary>
        /// This only needs to be called from ComponentManager
        /// </summary>
        protected override void Initialize() {
            Files = new FileManager(); 
            Files.Initialize();  // logging depends on this
            Log.Info("Starting SE Garden v" + ModData.Version, "");

            Commands = new ChatManager();
            Commands.Initialize();

            Messages = new MessageManager();
            Messages.Initialize();

            RunningOn = RunningOn;
            Initialized = true;
            base.Initialize();
        }


        /// <summary>
        /// This only needs to be called from ComponentManager
        /// </summary>
        protected override void Terminate() {
            Log.Info("Stopping SE Garden.", "");
            Commands.Terminate();
            Messages.Terminate();
            Files.Terminate();  // logging depends on this
            Initialized = false;
            base.Terminate();
        }

    }
}

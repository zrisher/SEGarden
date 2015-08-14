using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Files;
using SEGarden.Chat;
using SEGarden.Messaging;

using SEGarden.Logging;

namespace SEGarden {

    // TODO: Provide thread-safety for anything accessed through here

    class GardenGateway {

        // Public MyAPIGateway helpers
        // TODO: Should we further hide these in their own static classes
        // so we can stop from exposing them to = null and extraneous calls to
        // Initialize and Terminate?

        // TODO: We really need the update manager to be our primary entry point,
        // It should bootstrap the application, including making sure SE Garden
        // starts first, then any reliant mods can register their logic components

        // IMPORTANT: Files needs to be constructed first, initialized first, and 
        // terminated last, because all logging depends on it.

        public const String Version = "0.5"; 

        private static Logger Log = new Logger("SEGarden.GardenGateway");

        public static FileManager Files = new FileManager(); 
        public static ChatManager Commands = new ChatManager();
        public static MessageManager Messages = new MessageManager();
        public static bool Initialized;

        /// <summary>
        /// This only needs to be called from SEGarden.Session
        /// </summary>
        public static void Initialize() {
            Files.Initialize();  // logging depends on this
            Log.Info("Starting SE Garden v" + Version, "");
            Commands.Initialize();
            Messages.Initialize();
            Initialized = true;
        }


        /// <summary>
        /// This only needs to be called from SEGarden.Session
        /// </summary>
        public static void Terminate() {
            Log.Info("Stopping SE Garden.", "");
            Commands.Terminate();
            Messages.Terminate();
            Files.Terminate();  // logging depends on this
            Initialized = false;
        }

    }
}

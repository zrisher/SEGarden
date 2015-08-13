using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Files;
using SEGarden.Chat;

namespace SEGarden {

    // TODO: Provide thread-safety for anything accessed through here

    class GardenGateway {

        // Public MyAPIGateway helpers
        // TODO: Should we further hide these in their own static classes
        // so we can stop from exposing them to = null and extraneous calls to
        // Initialize and Terminate?

        public static FileManager Files = new FileManager();
        public static CommandProcessor Commands = new CommandProcessor();

        /// <summary>
        /// This only needs to be called from SEGarden.Session
        /// </summary>
        public static void Initialize() {
            Files.Initialize();
            Commands.Initialize();
        }

        /// <summary>
        /// This only needs to be called from SEGarden.Session
        /// </summary>
        public static void Terminate() {
            Commands.Terminate();
            Files.Terminate();
        }

    }
}

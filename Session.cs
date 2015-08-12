using System;
using System.Collections.Generic;
using System.Text;

using Sandbox.Common;
//using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.ModAPI;
//using VRage.Library.Utils;
//using Interfaces = Sandbox.ModAPI.Interfaces;
//using InGame = Sandbox.ModAPI.Ingame;

using SEGarden.Logic;
using SEGarden.Logging;

namespace SEGarden {

    /// <summary>
    /// LoadData, UnloadData, Update Before/After/Simulate, UpdatingStopped
    /// </summary>
	[Sandbox.Common.MySessionComponentDescriptor(Sandbox.Common.MyUpdateOrder.BeforeSimulation)]
	class InternalSession : GardenSession {

        private static Logger Log = new Logger("SEGarden.InternalSession");
        //private static SEGarden.Chat.Commands.Processor CommandProcessor;

        /*
        protected override void Initialize() {
            Log.Info("Intializing SE Garden", "Initialize");
            SEGarden.Files.Manager.Initialize();
            base.Initialize();
        }

        protected override void Terminate() {
            Log.Info("Terminating SE Garden", "Initialize");
            SEGarden.Files.Manager.Close();
            base.Terminate();
        }
         * */

        public override void UpdateBeforeSimulation() {
            base.UpdateBeforeSimulation();

            Log.Info("How come I never get called?", "UpdateBeforeSimulation");
        }

		//public override void Init(MyObjectBuilder_SessionComponent sessionComponent) {
		//	base.Init(sessionComponent);
		//}
	}

}

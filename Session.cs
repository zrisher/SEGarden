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
	class MainSession : GardenSession {

        private static Logger Log = new Logger("SEGarden.MainSession");

        protected override void Initialize() {
            Log.Info("Intializing SE Garden Internal Session", "Initialize");
            GardenGateway.Initialize();
            base.Initialize();
        }

        protected override void Terminate() {
            Log.Info("Terminating SE Garden Internal Session", "Terminate");
            GardenGateway.Terminate();
            base.Terminate();
        }

        /*

        public override void UpdateBeforeSimulation() {
            base.UpdateBeforeSimulation();

        }

        protected override void UpdateBeforeSimulation10() {
            base.UpdateBeforeSimulation10();

            //Log.Info("Garden internal session hit 10", "UpdateBeforeSimulation10");
        }

        protected override void UpdateBeforeSimulation100() {
            base.UpdateBeforeSimulation100();

            //Log.Info("Garden internal session hit 100", "UpdateBeforeSimulation100");
        }

        protected override void UpdateBeforeSimulation1000() {
            base.UpdateBeforeSimulation1000();

            //Log.Info("Garden internal session hit 1000", "UpdateBeforeSimulation1000");
        }
        
        */


	}

}

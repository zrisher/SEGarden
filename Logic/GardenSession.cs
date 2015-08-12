//using System;
//using System.Collections.Generic;

using Sandbox.Common;
//using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
//using VRage;
//using VRage.Collections;
//using VRage.ModAPI;
//using VRage.ObjectBuilders;

using SEGarden.Logging;

namespace SEGarden.Logic {

	/// <summary>
    /// A layer over the MySessionComponentBase. Inherit from this to help with:
    ///   * Waiting for initialization until MyAPIGateway is ready
    ///   * Using lower resolution updates
    /// 
    /// Heavily inspired by Rynchodon's Update Manager
    /// https://github.com/Rynchodon/Autopilot/
    /// 
    /// I'd love to provide more of the features that he has, but need to find the
    /// time to incorporate that and make it so we can register scripts at runtime.
    /// Otherwise consumers of SEGarden will always have to edit its files.
	/// </summary>
	public abstract class GardenSession : MySessionComponentBase {

        private static Logger Log = new Logger("SEGarden.Logic.GardenSession");

        private enum RunStatus : byte { NotInitialized, Initialized, Terminated }
        private RunStatus Status = RunStatus.NotInitialized;
        private uint Frame = 0;


        // Inheriting classes should override this if True
        protected virtual bool ServerOnly() { return false; }

		/// <summary>
		/// For MySessionComponentBase
		/// </summary>
		//public ComponentManager() { }


        /// <summary>
        /// Place your custom initialization logic that relies on ModAPI here
        /// </summary>
        protected virtual void Initialize() {
            Log.Info("Initializing", "Initialize");
            Status = RunStatus.Initialized;
        }

        protected virtual void Terminate() {
            Log.Info("Terminating", "Initialize");
            Status = RunStatus.Terminated;
        }

		private void TryInit() {

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
                if (ServerOnly() && !MyAPIGateway.Multiplayer.IsServer) {
                    Log.Info("Terminating - not allowed on client", "TryInit");
				    Status = RunStatus.Terminated;
				    return;
                }
                Log.Info("Initializing as server", "TryInit");
            }
            Log.Info("Initializing as single player", "TryInit");

            Initialize();
		}

        protected bool ShouldRun() {
            switch (Status) {
				case RunStatus.NotInitialized:
				case RunStatus.Terminated:
					return false;
			}

            return true;
        }

        private bool ShouldRunTryInit() {
            switch (Status) {
				case RunStatus.NotInitialized:
					TryInit();
					return false;
				case RunStatus.Terminated:
					return false;
			}

            return true;
        }

        /// TODO: Add more resolutions?
        /// 
        // Every 16.6ms at 60 UPS
        protected virtual void UpdateBeforeSimulation10() { }
        protected virtual void UpdateAfterSimulation10() { }

        // Every 1.6s at 60 UPS
        protected virtual void UpdateBeforeSimulation100() { }
        protected virtual void UpdateAfterSimulation100() { }

        // Every 16s at 60 UPS
        protected virtual void UpdateBeforeSimulation1000() { }
        protected virtual void UpdateAfterSimulation1000() { }

        /// <summary>
        /// Calls lower resolution update methods
        /// </summary>
        /// <remarks>
        /// Inheriting methods must remember to call base. if they want to use the 
        /// lower resolution update methods in the rest of the class
        /// They might also want to check ShouldRun()
        /// </remarks>

        public override void UpdateBeforeSimulation() {
            base.UpdateBeforeSimulation();

            // TODO: 
            // Doesn't seem like sim logic is running
            // Our initialization needs to be smarter about which side of the 
            // update it runs on, because the components could be registered for just
            // before, just after, just simulation, or maybe even all of them
            // Maybe we should just expect the user to call it?
            //
            // Also, terminate needs to be called on unload

            Frame++;

            if (!ShouldRun()) return;

            if (Frame % 10 == 0){
                //Log.Info("Update10", "UpdateBeforeSimulation");
                UpdateBeforeSimulation10();

                if (Frame % 100 == 0){
                    Log.Info("Update100", "UpdateBeforeSimulation");
                    UpdateBeforeSimulation100();

                    if (Frame % 1000 == 0){
                        Log.Info("Update1000", "UpdateBeforeSimulation");
                        UpdateBeforeSimulation1000();
                        Frame = 0; // can only store up to 65535
                    }
                }
            }
        }


        /// <summary>
		/// Initializes if needed, issues updates.
		/// </summary>
        /// <remarks>
        /// Inheriting methods must remember to call base. if they want to use the 
        /// lower resolution update methods in the rest of the class
        /// They might also want to check ShouldRun()
        /// </remarks>
        public override void UpdateAfterSimulation() {
            base.UpdateAfterSimulation();

            // We try to initialize here, but it will take effect next frame.
            if (!ShouldRunTryInit()) return;

            if (Frame % 10 == 0){
                //Log.Info("Update10", "UpdateAfterSimulation");
                UpdateAfterSimulation10();

                if (Frame % 100 == 0){
                    Log.Info("Update100", "UpdateAfterSimulation");
                    UpdateAfterSimulation100();

                    if (Frame % 1000 == 0){
                        Log.Info("Update1000", "UpdateAfterSimulation");
                        UpdateAfterSimulation1000();
                    }
                }
            }
		}	

    }

}

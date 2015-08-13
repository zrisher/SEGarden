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
    ///   * Using various resolution updates
    ///   * Catching update errors - TODO
    /// 
    /// Heavily inspired by Rynchodon's Update Manager
    /// https://github.com/Rynchodon/Autopilot/
    /// 
    /// I'd love to provide more of the features that he has, but need to find the
    /// time to incorporate that and make it so we can register scripts at runtime.
    /// Otherwise consumers of SEGarden will always have to edit its files.
    /// 
    /// We don't know what updates we're schedule for, because even if we detected
    /// they can change. So our initialization checks have to run Before, After, and
    /// on Sim.  We could avoid all that if we required the user to do their 
    /// initialization checks manunally.
    /// 
    /// We have to maintain independent frame counts for each update time for the 
    /// same reason. 
	/// </summary>
	public abstract class GardenSession : MySessionComponentBase {

        private enum RunStatus : byte { 
            NotInitialized, Initialized, Terminated 
        }

        private static Logger Log = new Logger("SEGarden.Logic.GardenSession");
        private RunStatus Status = RunStatus.NotInitialized;
        private uint BeforeFrame = 0;
        private uint AfterFrame = 0;

        // TODO: Allow marking as client-only too

        /// <summary>
        /// Inheriting classes should override this if True
        /// </summary>
        protected virtual bool ServerOnly() { return false; }

        /// <summary>
        /// Place your custom initialization logic that relies on ModAPI here
        /// </summary>
        protected virtual void Initialize() { }

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
            Status = RunStatus.Initialized;
        }

        /// <summary>
        /// Place your custom termination logic that relies on ModAPI here
        /// </summary>
        protected virtual void Terminate() { }

        private void TerminateIfRunning() {
            if (Status != RunStatus.Terminated) {
                Terminate();
                Log.Info("Marking terminated", "Terminate");
                Status = RunStatus.Terminated;
            }
        }

        /// <summary>
        /// Inheriting classes should check this in their updates
        /// TODO - use actions for inherited updates to remove this requirement
        /// </summary>
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
        /// Don't forget to check ShouldRun() when using these
        /// TODO: We should really just have consumers register update actions,
        /// then we can do error checking on them if requested

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
        /// Calls lower resolution update methods and trys init if needed
        /// </summary>
        /// <remarks>
        /// Inheriting methods must remember to call base. if they want to use the 
        /// lower resolution update methods in the rest of the class
        /// They might also want to check ShouldRun()
        /// </remarks>

        public override void UpdateBeforeSimulation() {
            base.UpdateBeforeSimulation();

            BeforeFrame++;

            if (!ShouldRunTryInit()) return;

            if (BeforeFrame % 10 == 0){
                //Log.Info("Update10", "UpdateBeforeSimulation");
                UpdateBeforeSimulation10();

                if (BeforeFrame % 100 == 0){
                    //Log.Info("Update100", "UpdateBeforeSimulation");
                    UpdateBeforeSimulation100();

                    if (BeforeFrame % 1000 == 0){
                        //Log.Info("Update1000", "UpdateBeforeSimulation");
                        UpdateBeforeSimulation1000();
                        BeforeFrame = 0; // can only store up to 65535
                    }
                }
            }
        }

        public override void Simulate() {
            base.Simulate();

            ShouldRunTryInit();
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

            AfterFrame++;

            // We try to initialize here, but it will take effect next frame.
            if (!ShouldRunTryInit()) return;

            if (BeforeFrame % 10 == 0){
                //Log.Info("Update10", "UpdateAfterSimulation");
                UpdateAfterSimulation10();

                if (BeforeFrame % 100 == 0){
                    //Log.Info("Update100", "UpdateAfterSimulation");
                    UpdateAfterSimulation100();

                    if (BeforeFrame % 1000 == 0){
                        //Log.Info("Update1000", "UpdateAfterSimulation");
                        UpdateAfterSimulation1000();
                        AfterFrame = 0; // can only store up to 65535
                    }
                }
            }
		}

        protected override void UnloadData() {
            base.UnloadData();
            TerminateIfRunning();
        }

        public override void UpdatingStopped() {
            base.UpdatingStopped();
            TerminateIfRunning();
        }
    }

}

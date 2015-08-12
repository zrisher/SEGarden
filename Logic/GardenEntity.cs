//using System;
//using System.Collections.Generic;

using Sandbox.Common;
using Sandbox.Common.Components;
//using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
//using VRage;
//using VRage.Collections;
//using VRage.ModAPI;
//using VRage.ObjectBuilders;

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
    public abstract class GardenEntity : MyGameLogicComponent {

        private enum RunStatus : byte { NotInitialized, Initialized, Terminated }
        private RunStatus Status = RunStatus.NotInitialized;
        //private uint Frame = 0;
        //private Logger myLogger = new Logger(null, "UpdateManager");

        // Inheriting classes should override this if True
        protected virtual bool ServerOnly() { return false; }

        private void CheckRunnable() {

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
                    // Log info - Not allowed to run
                    Status = RunStatus.Terminated;
                    return;
                }
                // Log info - Running in Multiplayer
            }
            // log info - running in single pl;ayer

            Status = RunStatus.Initialized;
        }

        protected bool ShouldRun() {
            switch (Status) {
                case RunStatus.NotInitialized:
                case RunStatus.Terminated:
                    return false;
            }

            return true;
        }

        protected bool ShouldRunTryInit() {
            switch (Status) {
                case RunStatus.NotInitialized:
                    CheckRunnable();
                    return false;
                case RunStatus.Terminated:
                    return false;
            }

            return true;
        }

    }

}

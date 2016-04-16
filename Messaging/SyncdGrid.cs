/* 
 * 
 * An example for when VRage.Library.Sync.SyncHelpers is whitelisted 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Definitions; // from Sandbox.Game.dll
using Sandbox.Game.Entities; // from Sandbox.Game.dll
using Sandbox.ModAPI;

using VRage; // from VRage.dll and VRage.Library.dll
using VRage.Game; // from VRage.Game.dll
using VRage.Game.Entity; // from VRage.Game.dll
using VRage.Game.ModAPI; // from VRage.Game.dll
using VRage.ModAPI; // from VRage.Game.dll
using VRage.ObjectBuilders;

using SEGarden.Extensions;
using SEGarden.Logging;
using SEGarden.Logic;
using SEGarden.World.Inventory;

using GC.Sessions;
using GC.World.Blocks;

using VRage.Library.Sync;


namespace GC.World.Grids {

    public class SyncdGrid : EntityComponent {

        public readonly SyncType SyncType;

        readonly Sync<int> SyncdVar1;
        readonly Sync<int> SyncdVar2;
        readonly Sync<bool> SyncdVar3;


        #region Properties

        // EntityComponent
        public override Dictionary<uint, Action> UpdateActions {
            get {
                var actions = base.UpdateActions;
                actions.Add(47, DisplayValues);
                actions.Add(103, UpdateValues);
                return actions;
            }
        }

        public override string ComponentName { get { return "GC.SyncdGrid"; } }

        #endregion
        #region Constructors

        // Creation from ingame entity
        public SyncdGrid(IMyEntity entity) : base(entity) {
            Log.ClassName = ComponentName;
            Log.Trace("Start SyncdGrid constructor", "ctr");

            SyncType = SyncHelpers.Compose(this);

            SyncdVar1.ValidateNever();
            SyncdVar2.ValidateNever();
            SyncdVar3.ValidateNever();            

            Log.Trace("Finished SyncdGrid constructor", "ctr");
        }

        #endregion
        #region Init/Terminate

        public override void Initialize() {
            Log.Trace("Initializing SyncdGrid", "Terminate");

            base.Initialize();
        }

        public override void Terminate() {
            Log.Trace("Terminating SyncdGrid", "Terminate");


            base.Terminate();
        }

        #endregion

        #region Update

        private void UpdateValues() {
            if (SEGarden.GardenGateway.RunningOn == RunLocation.Server) {
                Log.Trace("Updating SyncdValues", "Update");
                SyncdVar1.Value ++;
                SyncdVar2.Value ++;
                SyncdVar3.Value = !SyncdVar3.Value;
            }
        }

        private void DisplayValues() {
            Log.Trace("SyncdVar1:" + SyncdVar1.Value, "Update");
            Log.Trace("SyncdVar2:" + SyncdVar2.Value, "Update");
            Log.Trace("SyncdVar3:" + SyncdVar3.Value, "Update");
        }

        #endregion



    }
}
*/
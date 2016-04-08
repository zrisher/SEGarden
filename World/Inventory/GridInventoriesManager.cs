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

using SEGarden.Logging;

namespace SEGarden.World.Inventory {
    
    /// <summary>
    /// Tracks the counts for a configurable set of items across a 
    /// configurable set of blocks on a given grid.
    /// </summary>
    /// <remarks>
    /// TODO: Filter found inventories by a configurable subtypename
    /// </remarks>
    class GridInventoriesManager {

        public InventoryItemsCount Totals;

        readonly Dictionary<MyInventoryBase, InventoryItemsCount> InventoryTotals;
        readonly List<MyDefinitionId> WatchedItems;
        readonly Logger Log;
        readonly IMyCubeGrid Grid;

        public GridInventoriesManager(IMyCubeGrid grid, List<MyDefinitionId> watchedItems = null) {
            Grid = grid;
            Log = new Logger("SEGarden.World.Inventory.GridInventoriesManager", (() => Grid.EntityId.ToString()));
            Totals = new InventoryItemsCount();
            InventoryTotals = new Dictionary<MyInventoryBase, InventoryItemsCount>();
            WatchedItems = watchedItems; 
        }

        public void Initialize() {
            Grid.OnBlockAdded += BlockAdded;
            Grid.OnBlockRemoved += BlockRemoved;

            List<IMySlimBlock> allBlocks = new List<IMySlimBlock>();
            Grid.GetBlocks(allBlocks);
            foreach (var block in allBlocks) {
                BlockAdded(block);
            }
        }

        public void Terminate() {
            Grid.OnBlockAdded -= BlockAdded;
            Grid.OnBlockRemoved -= BlockRemoved;

            foreach (var inventory in InventoryTotals.Keys) {
                inventory.ContentsChanged -= UpdateInventory;
            }
        }

        private void BlockAdded(IMySlimBlock slimblock) {
            //Log.Trace(slimblock.ToString() + " added to InventoryManager for " + Grid.DisplayName, "blockAdded");

            if (slimblock == null) {
                Log.Error("Received null slimblock", "blockAdded");
                return;
            }
                
            MyEntity fatEntity = slimblock.FatBlock as MyEntity;
            if (fatEntity == null) return;

            MyInventoryBase inventory;
            if (!fatEntity.TryGetInventory(out inventory)) return;

            Log.Trace("Adding inventory " + slimblock.ToString(), "blockAdded");
            InventoryTotals[inventory] = new InventoryItemsCount();
            UpdateInventory(inventory);
            inventory.ContentsChanged += UpdateInventory;
        }

        private void BlockRemoved(IMySlimBlock slimblock) {
            //Log.Trace(slimblock.ToString() + " removed from InventoryManager for " + Grid.DisplayName, "blockRemoved");

            if (slimblock == null) {
                Log.Error("Received null slimblock", "blockRemoved");
                return;
            }

            MyEntity fatEntity = slimblock.FatBlock as MyEntity;
            if (fatEntity == null)
                return;

            MyInventoryBase inventory;
            if (!fatEntity.TryGetInventory(out inventory))
                return;

            Log.Trace("Removing inventory " + slimblock.ToString(), "blockRemoved");
            if (!InventoryTotals.Remove(inventory)) {
                Log.Error("Received an removal for inventory we're not tracking.", "blockRemoved");
                return;
            }
            inventory.ContentsChanged -= UpdateInventory;
        }

        private void UpdateInventory(MyInventoryBase inventory) {
            Log.Trace("Updating inventory cache with inventory " + inventory.Entity.ToString(), "UpdateInventory");
            InventoryItemsCount itemCache;
            if (!InventoryTotals.TryGetValue(inventory, out itemCache)) {
                Log.Error("Received an update for inventory we're not tracking.", "UpdateInventory");
                return;
            }

            Totals -= itemCache;

            if (WatchedItems != null) {
                foreach (var id in WatchedItems) {
                    itemCache.Set(id, inventory.GetItemAmount(id));
                }
            }
            else {
                foreach (var item in inventory.GetItems()) {
                    itemCache.Set(item.Content.GetObjectId(), item.Amount);
                }
            }

            Totals += itemCache;

            DebugPrint();
        }

        private void DebugPrint() {
            Log.Debug("Displaying inventory contents for grid: " + Grid.DisplayName, "DebugInventories");

            foreach (var kvp in Totals.GetCounts()) {
                Log.Debug("    " + kvp.Key.ToString() + " - " + kvp.Value, "DebugInventories");
            }

            /*
            foreach (var kvp in Counts) {
                Log.Debug("  contents for block: " + kvp.Key.Entity.ToString(), "DebugInventories");

                foreach (var def in WatchedItems) {
                    Log.Debug("    " + def.DisplayNameText + " - " + kvp.Value.Get(def.Id), "DebugInventories");
                }
            }
             * */
        }

    }

}

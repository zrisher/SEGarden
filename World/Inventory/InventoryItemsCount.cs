﻿using System;
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
    /// Represents a collection of item counts as a vector
    /// </summary>
    public class InventoryItemsCount {

        public static InventoryItemsCount Zero = new InventoryItemsCount();

        private static Logger Log = new Logger("InventoryItemsCount");

        #region Operators

        public static InventoryItemsCount operator +(InventoryItemsCount value1, InventoryItemsCount value2) {
            VRage.Exceptions.ThrowIf<ArgumentNullException>(value1 == null, "value1");
            VRage.Exceptions.ThrowIf<ArgumentNullException>(value2 == null, "value2");

            InventoryItemsCount result = value1.Copy();

            foreach (var kvp in value2.Counts) {
                result.Counts[kvp.Key] = result.Get(kvp.Key) + kvp.Value;
            }

            return result;
        }

        public static InventoryItemsCount operator -(InventoryItemsCount value1, InventoryItemsCount value2) {
            VRage.Exceptions.ThrowIf<ArgumentNullException>(value1 == null, "value1");
            VRage.Exceptions.ThrowIf<ArgumentNullException>(value2 == null, "value2");

            InventoryItemsCount result = value1.Copy();

            foreach (var kvp in value2.Counts) {
                result.Counts[kvp.Key] = result.Get(kvp.Key) - kvp.Value;
            }

            return result;
        }

        public static InventoryItemsCount operator *(InventoryItemsCount value1, float value2) {
            VRage.Exceptions.ThrowIf<ArgumentNullException>(value1 == null, "value1");

            //Log.Trace(String.Format("Calculating {0} * {1}", value1.ToString(), value2), "operator *");

            InventoryItemsCount result = value1.Copy();

            foreach (var key in value1.Counts.Keys) {
                //Log.Trace(String.Format("Multiplying {0} * {1} for {3}", result.Counts[kvp.Key], value2, kvp.Key), "operator *");
                result.Counts[key] *= value2;
            }

            //Log.Trace("Returning result", "operator *");
            return result;
        }

        public static InventoryItemsCount operator /(InventoryItemsCount value1, float value2) {
            //Log.Trace(String.Format("Calculating {0} / {1}", value1.ToString(), value2), "operator /");
            return value1 * (1 / value2);
        }

        /*
        public static bool operator <(InventoryItemsCount value1, InventoryItemsCount value2) {
            return value2.Contains(value1);
        }

        public static bool operator <=(InventoryItemsCount value1, InventoryItemsCount value2) {
            return value1 < value2;
        }
        */

        #endregion

        public readonly Dictionary<MyDefinitionId, MyFixedPoint> Counts =
            new Dictionary<MyDefinitionId, MyFixedPoint>();

        public InventoryItemsCount() { }

        public InventoryItemsCount(Dictionary<MyDefinitionId, MyFixedPoint> counts) {
            Counts = counts;
        }

        /*
        public void SetCount(MyDefinitionId defId, MyFixedPoint count) {
            Counts[defId] = count;
        }

        public MyFixedPoint GetCount(MyDefinitionId defId) {
            return Counts[defId];
        }
        */

        public MyFixedPoint Get(MyDefinitionId definitionId) {
            return Counts.ContainsKey(definitionId) ? Counts[definitionId] : MyFixedPoint.Zero;
        }

        public void Set(MyDefinitionId defId, MyFixedPoint count) {
            Counts[defId] = count;
        }


        public bool IsEmpty() {
            return Counts.Values.All((x) => (x <= MyFixedPoint.Zero));
        }

        public bool Contains(InventoryItemsCount other) {
            return other.Counts.All((kvp) => (kvp.Value <= Get(kvp.Key)));
        }

        /*
        public Dictionary<MyDefinitionId, MyFixedPoint> GetCounts() {
            VRage.Exceptions.ThrowIf<FieldAccessException>(this.Counts == null, "this.Counts");

            return new Dictionary<MyDefinitionId, MyFixedPoint>(Counts);
        }
        */

        public InventoryItemsCount Copy() {
            //Log.Trace(String.Format("Copying {0}", this.ToString()), "Copy");
            InventoryItemsCount result = new InventoryItemsCount();

            foreach (var kvp in this.Counts) 
                result.Set(kvp.Key, kvp.Value);

            //Log.Trace("Returning result", "Copy");
            return result;
        }

        public String ToString() {
            List<String> results = new List<String>();
            foreach (var kvp in Counts) {
                if (kvp.Value == 0) continue;
                results.Add(kvp.Key.SubtypeName.ToString() + ": " + kvp.Value.ToString());
            }

            return "[ " + String.Join(", ", results) + " ]";
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRage.Game;

namespace SEGarden.Logic {

    /// <summary>
    /// DISABLED - currently reflection not allowed in scripts.
    /// Use this attribute to tag SessionComponents for ComponentManager to load
    /// </summary>
    /// <remarks>
    /// Should we let this be inherited?
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SessionComponentDescriptor : System.Attribute {

        /// <summary>
        /// Where should it run?
        /// </summary>
        public RunLocation TargetLocation;

        /// <summary>
        /// Lower InitOrder is loaded before higher InitOrder
        /// </summary>
        public int InitOrder;

        /// <summary>
        /// Should we stop the component when it encounters an update error?
        /// </summary>
        public bool TerminateOnError;

        public SessionComponentDescriptor(RunLocation targetLocation)
            : this(targetLocation, 1000) {
        }

        public SessionComponentDescriptor(RunLocation targetLocation, int initOrder)
            : this(targetLocation, 1000, false) {
        }

        public SessionComponentDescriptor(RunLocation targetLocation, int initOrder,
            bool terminateOnError) {
            TargetLocation = targetLocation;
            InitOrder = initOrder;
            TerminateOnError = terminateOnError;
        }
    }

}

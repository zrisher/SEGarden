//using System;
//using System.Collections.Generic;

using Sandbox.Common;
//using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
//using VRage;
//using VRage.Collections;
//using VRage.ModAPI;
//using VRage.ObjectBuilders;

using SEGarden.Logic.Common;
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
	public interface GardenLogicInterface  {

        /// <summary>
        /// Place your custom initialization logic that relies on ModAPI here
        /// </summary>
        void Initialize();

        /// <summary>
        /// Place your custom termination logic that relies on ModAPI here
        /// </summary>
        void Terminate();

        void Update();

        ushort UpdateFrequency { get; }

    }

}

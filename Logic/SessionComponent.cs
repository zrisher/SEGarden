using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Logic {

    /// <summary>
    /// A layer over the MySessionComponentBase. Inherit from this to help with:
    ///   * Waiting for initialization until MyAPIGateway is ready
    ///   * Using various resolution updates
    ///   
    /// These can be declared with SessionComponentDescriptors to run automatically,
    /// or registered with the UpdateManager manually with greater flexibility
    /// </summary>
    public abstract class SessionComponent {

        // Useful for logging
        public String ComponentName { get { return ""; } }

        // The update manager will set these and provide them for reference here
        // It will check them when Initializing and Terminating
        public RunStatus Status { get; set; }
        public RunLocation CurrentLocation { get; set; }

        // A little syntatic sugar for you and me
        public bool IsRunning { get { return Status == RunStatus.Running; } }
        public bool IsTerminated { get { return Status == RunStatus.Terminated; } }

        // Inheriting classes should override this to change target location
        // Update manager will check this when Initializing and Terminating
        // Actually let's load this as an attribute because it's a primitive
        //public virtual readonly RunLocation RunOn = RunLocation.Any;

        /// <summary>
        /// Declare actions and the resolution at which they should run here
        /// NOTE: You actions need to take care of checking Status! They will
        /// always run unless terminated by an exception with that option ON.
        /// This is because in order to check the status of each component before
        /// running updates each frame, we would have to iterate through the list
        /// of components instead of one shared registry of all updates for that frame.
        /// It's just a lot slow, too slow to do every frame. There's no great way
        /// around this that I can think of.
        /// </summary>
        /// Doing this with registry logic for now, more flexible and keeps configuration
        /// completely out of this class
        /// 
        /// Actually you can do both. We'll check update actions and automatically
        /// register them, but you can also leave it empty and register them in 
        /// your constructor
        public virtual Dictionary<uint, Action> UpdateActions { 
            get {
                return new Dictionary<uint, Action>();
            } 
        }

        /// <summary>
        /// This is called by UpdateManager when running is started.
        /// Attach all managed resource aquisition here
        /// Stuff that depends on ModAPI can be safely used at this point
        /// You should call base.Initialize or manually mark initialize if attached
        /// </summary>
        /// <remarks>
        /// Really, this is called directly after the constructor every time,
        /// So we could just use the constructor.
        /// But separating this call from the constructor I believe actually provides
        /// more flexibility and better logical separation of concerns.
        /// And makes more sense with the terminate function below,
        /// because components should not wait until they're being destroyed to close
        /// </remarks>
        public virtual void Initialize() { 
            Status = RunStatus.Running;
        }

        public void InitializeConditional() {
            if (!IsRunning) Initialize();
        }

        /// <summary>
        /// Stops the component, detatching all hooks and managed resources
        /// This is called by UpdateManager when the script is terminated by the game
        /// either by a shutdown or simply a termination call
        /// You should call base.Terminate or otherwise mark terminated.
        /// </summary>
        public virtual void Terminate() { 
            Status = RunStatus.Terminated;
        }

        public void TerminateConditional() {
            if (IsRunning) Terminate();
        }

        /// <summary>
        /// Override this if you want custom run conditions
        /// </summary>
        /// <remarks>
        /// This can really only be checked on init, doing it every update
        /// cycle seems a little overkill. And at that point we might as well
        /// use the entity descriptors and take this away from the inheriting
        /// component's responsibilities.
        /// </remarks>
        /*
        public virtual bool ShouldRun() {
            return RunOn == RunLocation.Any || RunOn == CurrentLocation;
        }
        */


    }

}

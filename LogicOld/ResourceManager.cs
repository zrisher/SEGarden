using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Logging;

namespace SEGarden.Logic {
    abstract class ResourceManager {

        protected Logger Log = new Logger("SEGarden.Logic.Manager");

        protected RunStatus Status = RunStatus.NotInitialized;

        public virtual void Initialize() {
            switch (Status) {
                case RunStatus.NotInitialized:
                    Log.Trace("Initializing", "Initialize");
                    InitializeInternal();
                    Status = RunStatus.Running;
                    break;
                case RunStatus.Running:
                    Log.Warning("Already initialized", "Initialize");
                    break;
                case RunStatus.Terminated:
                    Log.Error("Can't initialize, terminated", "Initialize");
                    break;
            }
        }

        protected abstract void InitializeInternal();

        public virtual void Terminate() {
            switch (Status) {
                case RunStatus.Running:
                    Log.Trace("Terminating", "Terminate");
                    TerminateInternal();
                    Status = RunStatus.Terminated;
                    break;
                case RunStatus.NotInitialized:
                    Log.Warning("Can't terminate, not initialized", "Initialize");
                    break;
                case RunStatus.Terminated:
                    Log.Warning("Already terminated", "Initialize");
                    break;
            }
        }

        protected abstract void TerminateInternal();
    }
}

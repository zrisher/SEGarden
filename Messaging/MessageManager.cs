using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Library.Utils;
using Interfaces = Sandbox.ModAPI.Interfaces;
using InGame = Sandbox.ModAPI.Ingame;

//using SEGarden.Core;

using SEGarden.Logging;

using SEGarden.Logic.Common;

namespace SEGarden.Messaging {


    public class MessageManager {

        private static Logger Log = new Logger("SEGarden.Messaging.MessageManager");

        private Dictionary<ushort, MessageHandlerBase> RegisteredHandlers = 
            new Dictionary<ushort, MessageHandlerBase>();

        private Dictionary<ushort, MessageHandlerBase> HandlersToRegister =
            new Dictionary<ushort, MessageHandlerBase>();

        private RunStatus Status = RunStatus.NotInitialized;

        public virtual void Initialize() {
            switch (Status) {
                case RunStatus.NotInitialized:
                    Log.Trace("Initializing Message Manager", "Initialize");
                    Status = RunStatus.Initialized;
                    RegisterHandlersFromQueue();
                    break;
                case RunStatus.Initialized:
                    Log.Warning("Already initialized", "Initialize");
                    break;
                case RunStatus.Terminated:
                    Log.Error("Can't initialize, terminated", "Initialize");
                    break;
            }
        }

        public virtual void Terminate() {
            switch (Status) {
                case RunStatus.Initialized:
                    Log.Trace("Terminating Message Manager", "Terminate");
                    UnregisterHandlers();
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

        public void AddHandler(ushort messageId, MessageHandlerBase handler) {
            Log.Trace("Adding handler for " + messageId, "AddHandler");
            switch (Status) {
                case (RunStatus.Initialized):
                    RegisterHandler(messageId, handler);
                    Log.Trace("Registered", "AddHandler");
                    break;
                case (RunStatus.NotInitialized):
                    QueueHandler(messageId, handler);
                    Log.Trace("Queued", "AddHandler");
                    break;
                case (RunStatus.Terminated):
                    Log.Error("Terminated, can't add handler for MessageId " + messageId, "AddHandler");
                    break;
            }
        }

        private void RegisterHandler(ushort domainId, MessageHandlerBase handler) {
            if (RegisteredHandlers.ContainsKey(domainId)) {
                Log.Error("Cannot register another handler for message domain " + domainId, 
                    "AddHandler");
                return;
            }
                MyAPIGateway.Multiplayer.RegisterMessageHandler(domainId, handler.ReceiveBytes);
            RegisteredHandlers.Add(domainId, handler);
            Log.Trace("Registered message handler for Id " + domainId, "AddHandler");
        }

        private void QueueHandler(ushort messageId, MessageHandlerBase handler) {
            HandlersToRegister.Add(messageId, handler);
            Log.Info("Queued message handler for Id " + messageId, "QueueHandler");
        }

        private void RegisterHandlersFromQueue() {
            if (HandlersToRegister.Count < 1) return;

            Log.Trace("Registering " + HandlersToRegister.Count + 
                " Queued message handlers", "RegisterHandlersFromQueue");

            foreach (KeyValuePair<ushort, MessageHandlerBase> kvp in HandlersToRegister) {
                RegisterHandler(kvp.Key, kvp.Value);
            }

            HandlersToRegister.Clear();
        }

        private void UnregisterHandlers() {
            if (RegisteredHandlers.Count < 1) return;

            Log.Trace("Unregistering " + RegisteredHandlers.Count +
                " message handlers", "UnregisterHandlers");

            foreach (KeyValuePair<ushort, MessageHandlerBase> kvp in RegisteredHandlers) {
                MyAPIGateway.Multiplayer.UnregisterMessageHandler(kvp.Key, kvp.Value.ReceiveBytes);
            }

            RegisteredHandlers.Clear();
        }

    }

}


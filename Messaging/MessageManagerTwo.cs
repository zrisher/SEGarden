using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using Sandbox.ModAPI;

using VRage;
using VRage.Collections;

//using SEGarden.Core;
using SEGarden.Logging;
using SEGarden.Logic;

using SEGarden.Exceptions;

namespace SEGarden.Messaging {

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Registering a new message handler when receiving a message
    /// WILL CREATE A DEADLOCK
    /// </remarks>
    public class MessageManagerTwo {

        static Logger Log = new Logger("SEGarden.Messaging.MessageManagerTwo");

        readonly FastResourceLock Lock = new FastResourceLock();

        readonly Dictionary<ushort, Func<Byte[], MessageBase>> Constructors =
            new Dictionary<ushort, Func<Byte[], MessageBase>>();

        readonly Dictionary<ushort, Dictionary<ushort, List<Action<MessageBase>>>> Handlers =
            new Dictionary<ushort, Dictionary<ushort, List<Action<MessageBase>>>>();

        RunStatus Status = RunStatus.NotInitialized;

        public virtual void Initialize() {
            switch (Status) {
                case RunStatus.NotInitialized:
                    Log.Trace("Initializing Message Manager", "Initialize");
                    RegisterForDelayedDomains();
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

        public virtual void Terminate() {
            switch (Status) {
                case RunStatus.Running:
                    Log.Trace("Terminating Message Manager", "Terminate");
                    UnregisterAll();
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

        /// <summary>
        /// Provide MessageManager with a deserializing constructor for a 
        /// message inheriting from MessageBase
        /// </summary>
        public void AddConstructor(
            ushort messageTypeId, Func<Byte[], MessageBase> constructor
        ) {
            if (constructor == null)
                throw new ArgumentException("null constructor");

            if (Status == RunStatus.Terminated)
                throw new InvalidOperationException("Manager not running");

            using (Lock.AcquireExclusiveUsing()) {
                if (Constructors.ContainsKey(messageTypeId))
                    throw new InvalidOperationException(
                        "Received second constructor for messageType " +
                        messageTypeId);
                else
                    Constructors[messageTypeId] = constructor;
            }

            Log.Trace("Registered constructor for messageType " +
                messageTypeId, "RegisterHandler");
        }

        /// <summary>
        /// Register any message-handling action for a particular domain and
        /// message type. Constructors for the type must be already registered.
        /// </summary>
        public void AddHandler(
            ushort domainId, ushort messageTypeId, Action<MessageBase> handler
        ) {
            if (handler == null)
                throw new ArgumentException("null handler");

            if (Status == RunStatus.Terminated)
                throw new InvalidOperationException("Manager not running");

            using (Lock.AcquireExclusiveUsing()) {
                if (!Constructors.ContainsKey(messageTypeId))
                    throw new ArgumentException("No ctr for message type");

                if (!Handlers.ContainsKey(domainId)) {
                    Handlers[domainId] =
                        new Dictionary<ushort, List<Action<MessageBase>>>();

                    // wait to actually register domain with MyAPIGateway if
                    // it's not ready yet (i.e. we're not initialized)
                    if (Status == RunStatus.Running) {
                        MyAPIGateway.Multiplayer.RegisterMessageHandler(
                            domainId, TryHandleMessage
                        );
                    }
                }

                if (!Handlers[domainId].ContainsKey(messageTypeId)) {
                    Handlers[domainId][messageTypeId] =
                        new List<Action<MessageBase>>();
                }

                Handlers[domainId][messageTypeId].Add(handler);
            }
            
            Log.Trace("Registered handler for message " + messageTypeId +
                " in domain " + domainId, "AddHandler");
        }

        /// <summary>
        /// Deregister any message-handling action for a particular domain and
        /// message type.
        /// </summary>
        public void RemoveHandler(
            ushort domainId, ushort messageTypeId, Action<MessageBase> handler
        ) {
            if (handler == null)
                throw new ArgumentException("null handler");

            if (Status == RunStatus.Terminated)
                throw new InvalidOperationException("Manager not running");

            using (Lock.AcquireExclusiveUsing()) {
                // bubble up key errors
                Handlers[domainId][messageTypeId].Remove(handler);

                if (Handlers[domainId][messageTypeId].Count == 0) {
                    Handlers[domainId].Remove(messageTypeId);
                }

                if (Handlers[domainId].Keys.Count == 0) {
                    Handlers.Remove(domainId);

                    if (Status == RunStatus.Running) {
                        MyAPIGateway.Multiplayer.UnregisterMessageHandler(
                            domainId, TryHandleMessage
                        );
                    }
                }
            }

            Log.Trace("Unregistered handler for message " + messageTypeId +
                " in domain {1}" + domainId, "RemoveHandler");
        }

        /// <summary>
        /// Registers the arbitrary handler with SE for all domains that were 
        /// registered with MessageManager before we init'd. 
        /// See the note in AddHandler.
        /// </summary>
        private void RegisterForDelayedDomains() {
            using (Lock.AcquireSharedUsing()) {
                foreach (var domainId in Handlers.Keys) {
                    MyAPIGateway.Multiplayer.RegisterMessageHandler(
                        domainId, TryHandleMessage
                    );
                }
            }
        }
        
        private void UnregisterAll() {
            Log.Trace("Unregistering message handlers", "UnregisterAll");
            using (Lock.AcquireExclusiveUsing()) {
                foreach (var domainId in Handlers.Keys) {
                    MyAPIGateway.Multiplayer.UnregisterMessageHandler(
                        domainId, TryHandleMessage
                    );
                }

                Constructors.Clear();
                Handlers.Clear();
            }
        }
        
        ///<remarks>
        /// We wrap the whole message handling in a try-catch, just like every
        /// other entry point to SEGarden-run logic. Let's not crash the game.
        ///</remarks>
        private void TryHandleMessage(byte[] bytes) {
            try { HandleMessage(bytes); }
            catch (Exception e) {
                Log.Error("Error handling message: " + e, "ReceiveBytes");
            }
        }

        private void HandleMessage(byte[] bytes) {
            Log.Info("Got message of size " + bytes.Length, "ReceiveBytes");

            // Deserialize base message
            MessageContainer container = MessageContainer.FromBytes(bytes);

            using (Lock.AcquireSharedUsing()) {

                Dictionary<ushort,List<Action<MessageBase>>> handlersByType;
                if (!Handlers.TryGetValue(container.DomainId, out handlersByType)) {
                    Log.Error("Failed to find handler for domain " + 
                        container.DomainId, "HandleMessage");
                }

                List<Action<MessageBase>> handlers;
                if (!handlersByType.TryGetValue(container.TypeId, out handlers)) {
                    Log.Error("Failed to find handler for type " + 
                        container.TypeId, "HandleMessage");
                }

                Func<Byte[],MessageBase> ctr;
                if (!Constructors.TryGetValue(container.TypeId, out ctr)) {
                    Log.Error("Failed to find constructor for type " + 
                        container.TypeId, "HandleMessage");
                }

                MessageBase msg = ctr(container.Body);

                // should we have another try-catch so individual handlers 
                // don't trip up others?
                foreach(var handler in handlers) {
                    handler(msg);
                }
            }
        }

    }

}

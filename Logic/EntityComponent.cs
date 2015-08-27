//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using Sandbox.ModAPI;
using VRage.ModAPI;

using SEGarden.Extensions;
using SEGarden.Logging;

namespace SEGarden.Logic {

    public abstract class EntityComponent : SessionComponent {

        protected IMyEntity Entity;

        private static Logger Log = new Logger("SEGarden.Logic.EntityComponent");

        public EntityComponent(IMyEntity entity) : base() {
            Entity = entity;
            EntityId = entity.EntityId;
        }

        public EntityComponent(VRage.ByteStream stream) : base() {
            EntityId = stream.getLong();
            Entity = MyAPIGateway.Entities.GetEntityById(EntityId);
        }

        public long EntityId { get; protected set; }
        // This can change over time, no events, so get it instead of storing
        public string DisplayName { get { return Entity.DisplayName;  } }

        public override void Initialize() {
            Log.Trace("Initialize entity component abstract", "Initialize");
            Entity.OnMarkForClose += OnClose; // should we use closing or close instead?
            base.Initialize();
        }

        private void OnClose(IMyEntity e) {
            Log.Trace("Attached entity is closing", "OnClose");
            TerminateConditional();
        }

        public override void Terminate() {
            Log.Trace("Terminate entity component abstract", "Terminate");
            Entity.OnMarkForClose -= OnClose;
            base.Terminate();
        }

        public virtual void AddToByteStream(VRage.ByteStream stream) {
            stream.addLong(EntityId);
        }

    }

}

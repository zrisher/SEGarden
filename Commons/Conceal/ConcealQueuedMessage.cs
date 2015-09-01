using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Extensions;

namespace SEGarden.Commons.Conceal {

    class ConcealQueuedMessage : ConcealMessageBase {

        private static readonly int SIZE = sizeof(long) + ConcealMessageBase.SIZE;

        public ConcealQueuedMessage(long entityId) :
            base((ushort)MessageType.ConcealQueuedMessage)
        {
            EntityId = entityId;
        }

        public ConcealQueuedMessage(byte[] bytes) : 
            base((ushort)MessageType.ConcealQueuedMessage) 
        {
            VRage.ByteStream stream = new VRage.ByteStream(bytes, bytes.Length);
            EntityId = stream.getLong();
            Log.Info("Deserialized ConcealQueuedNotification for " + EntityId, 
                "byte ctr");
        }

        public long EntityId;

        protected override byte[] ToBytes() {
            VRage.ByteStream stream = new VRage.ByteStream(SIZE);
            stream.addLong(EntityId);
            Log.Info("Serialized ConcealQueuedNotification for " + EntityId,
                "ToBytes");
            return stream.Data;
        }

    }

}

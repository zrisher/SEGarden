using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Messaging;

namespace SEGarden.Messaging {
    abstract class MessageBase {

        protected abstract ushort DomainId { get; }
        protected abstract ushort TypeId { get; }
        
        protected abstract byte[] ToBytes();

        public void Send(long destId, MessageDestinationType destType, 
            bool reliable = true) {

            MessageContainer container = new MessageContainer() {
                Body = ToBytes(),
                DestinationId = (ulong)destId,
                DestinationType = destType,
                MessageDomainId = DomainId,
                MessageTypeId = TypeId,
                Reliable = reliable
            };

            container.Send();
        }
    }
}

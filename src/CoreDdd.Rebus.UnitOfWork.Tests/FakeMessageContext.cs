﻿using System.Collections.Generic;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;

namespace CoreDdd.Rebus.UnitOfWork.Tests
{
    public class FakeMessageContext : IMessageContext
    {
        public ITransactionContext TransactionContext { get; }
        public IncomingStepContext IncomingStepContext { get; }
        public TransportMessage TransportMessage { get; }
        public Message Message { get; }
        public Dictionary<string, string> Headers { get; }
    }
}

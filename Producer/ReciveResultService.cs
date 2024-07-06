using Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    internal class ReciveResultService:IDisposable
    {
        private readonly IHandlerMsg _handler;

        private readonly SyncConsumer _syncConumer;

        public ReciveResultService(IHandlerMsg handler, string hostName, string queueName)
        {
            _handler = handler;
            _syncConumer = new SyncConsumer(hostName, queueName, handler);
            
        }

        public void Dispose()
        {
            _syncConumer.Dispose();
        }
    }
}

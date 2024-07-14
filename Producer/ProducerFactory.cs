using Producer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    public class ProducerFactory
    {

        private static IWorker _worker;
        private static ActionsQueue _actionQueue;
        private static ProcessingManager _processingManager;

        public static IWorker GetWorker(string hostName, string queueName)
        {
            _actionQueue = new ActionsQueue();
            _worker = new BackgroundWorker(_actionQueue);
            _processingManager = new ProcessingManager(_actionQueue, new Sender(hostName, queueName));
            _processingManager.Start();

            return _worker;
        }

        public static void Stop()
        {
            _processingManager.Stop();            
        }
    }
}

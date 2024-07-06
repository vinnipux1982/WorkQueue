using Producer.Models;
using System.Collections.Concurrent;


namespace Producer
{
    internal class ActionsQueue
    {
        private readonly BlockingCollection<ActionInfo> _queue = [];

        public void Enqueue(ActionInfo action)
        {
            _queue.Add(action);
        }

        public ActionInfo Dequeue() { 
        
            return _queue.Take();
        
        }
    }
}

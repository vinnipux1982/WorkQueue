using Producer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    internal class ActionsStorage
    {

        private readonly ConcurrentDictionary<Guid,ActionInfo> _actions = [];

        public void Add(ActionInfo action)
        {
            _actions.TryAdd(action.ClientId, action);

        }

        public ActionInfo? GetActionInfo(Guid requestId)
        {
            if(_actions.TryGetValue(requestId, out var actionInfo))
            {
                return actionInfo;
            }
            return null;

        }

    }
}

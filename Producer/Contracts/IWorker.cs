using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Contracts
{
    public interface IWorker
    {
        Task SendActionAsync(string payload);
    }
}

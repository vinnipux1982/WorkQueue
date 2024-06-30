using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Models
{
    internal record ActionInfo(TaskCompletionSource<string?> TaskCompletionSource, string Payload, Guid ClientId);
    
}

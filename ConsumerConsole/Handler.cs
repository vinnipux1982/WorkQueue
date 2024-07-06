using Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerConsole
{
    internal class Handler : IHandlerMsg
    {
        public void Processing(string message)
        {
            Console.WriteLine(message);
            Thread.Sleep(1000);
        }
    }
}

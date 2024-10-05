// See https://aka.ms/new-console-template for more information

using Consumer;
using ConsumerConsole;

Console.WriteLine("Test consumer");
_ = new SyncConsumer("127.0.0.1", "request_action", new Handler());
Console.ReadKey();
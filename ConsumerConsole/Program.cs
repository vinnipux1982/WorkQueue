// See https://aka.ms/new-console-template for more information

using Consumer;
using ConsumerConsole;

Console.WriteLine("Hello, World!");
var consumer = new SyncConsumer("127.0.0.1", "request_action", new Handler(),"127.0.0.1", "result_channel");
Console.ReadKey();
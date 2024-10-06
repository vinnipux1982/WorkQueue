// See https://aka.ms/new-console-template for more information

using Consumer;
using ConsumerConsole;

Console.WriteLine("Test consumer");
_ = new SyncConsumer("localhost", "Test", new Handler());
Console.ReadKey();
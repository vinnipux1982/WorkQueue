// See https://aka.ms/new-console-template for more information
using Consumer;
using ConsumerConsole;

Console.WriteLine("Hello, World!");
var consumer = new SyncConsumer("localhost", "test", new Handler());
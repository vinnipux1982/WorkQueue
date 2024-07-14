// See https://aka.ms/new-console-template for more information

using Producer;

Console.WriteLine("Hello, World!");
var worker = ProducerFactory.GetWorker("127.0.0.1", "test");
await worker.SendActionAsync("test");
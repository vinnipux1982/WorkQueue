// See https://aka.ms/new-console-template for more information

using Producer;

Console.WriteLine("Hello, World!");
var worker = ProducerFactory.GetWorker("127.0.0.1", "request_action","result_channel");
await worker.SendActionAsync("test");
Console.WriteLine("end program");
using Producer;

Console.WriteLine("Hello, World!");
var factory = new WorkerFactory("127.0.0.1", "request_action", "result_channel"); 
var worker = factory.GetWorker();
await worker.SendActionAsync("test");
Console.WriteLine("end program");
# WorkQueue

Distributed computing in synchronous and asynchronous modes.
Task to be solved:
There is a backend that receives requests requiring a large amount of resources. The computations themselves can take
from 1 second to several minutes. It is necessary to delegate the processing of such requests to other servers, while
the client that sent the request must wait for the processing results (on average, the waiting time will be around 3-10
seconds). The library should allow organizing two modes of operation:
• Synchronous
• Asynchronous

Synchronous mode of operation:
A client request is queued for processing on a remote server and is put into a waiting state (the connection with the
client is not broken) until the result is obtained. Upon receiving the result, the obtained data is sent back to the
client.

Asynchronous mode of operation:
The client sends a request for processing, the request is assigned an ID and returned to the client. The client
periodically queries the results by ID, and if they are ready, they are returned.

## Use

RabbitMQ

## Solve

**BackgroundWorker** - отправляет запрос на обработку, присваивая ему уникальный Id, при получении ответа от сервера
пробуждает поток клиента и отправляем ему результат обработки.


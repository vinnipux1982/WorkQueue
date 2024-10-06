# WorkQueue

Distributed computing in asynchronous modes.
Task to be solved:
There is a backend that receives requests requiring a large amount of resources. The computations themselves can take
from 1 second to several minutes. It is necessary to delegate the processing of such requests to other servers, while
the client that sent the request must wait for the processing results (on average, the waiting time will be around 3-10
seconds). 

## How it works

To run the application, specify the RabbitMQ connection settings in the RabbitMQ section. Start the application, and when prompted to enter a text string, provide the input. If the server is connected and there is a handler on the other side, a response will be returned with the message processing date. The handler has a 1.5-second delay. In the Producer project, you can find the code responsible for sending messages.

Workflow:

Upon receiving a message to send, IRabbitMqSender creates a send action and places it in a queue. To wait for the response, a TaskCompletionSource is created, and a Task is returned.
The BackgroundProcess class, upon queueing the action, sends it to the RabbitMQ server.
When the handler receives a response, ReceiveHandler locates the action in the store and passes the response to the task result, completing the message processing.
Features:

A message lifetime is set. If it expires, the message is deleted from RabbitMQ and the message store, and an error is sent back to the sender.
The connection to RabbitMQ is automatically restored. 

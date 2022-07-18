# MassTransit requests shutdown demo

Requests module does not shutdown gracefully - if message is in flight it won't be received. If queue is temporary, it will be removed and `RespondAsync` will fail (or create queue depending on broker).

## Repro steps:

1. Run RabbitMq:
    ```
    docker run --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.10-management
    ```
2. Run producer app and consumer app
3. Type something in producer console
4. Verify that message in consumer (it should print `waiting x ms`)
5. Try to shutdown producer app (i.e. press ctrl+c)

## Expected:

Producer waits for request to finish

## Actual:

Producer quits immediately

# Log from producer app:

Producer is canceled about 2 seconds after message is sent. Message has 15 seconds timeout. We can see that right after requesting shutdown bus is stopped, and no message can be received.

```text
[16:35:47] info: MessageSender[0]
Sending 123123123
[16:35:47] dbug: MassTransit.Messages[0]
SEND rabbitmq://localhost/Contracts:Message 445d0000-9a3c-0005-95e7-08da6993f867 Contracts.Message
[16:35:49] info: Microsoft.Hosting.Lifetime[0]
Application is shutting down...
[16:35:49] dbug: Microsoft.Extensions.Hosting.Internal.Host[3]
Hosting stopping
[16:35:49] dbug: MassTransit.Transports.BusDepot[0]
Stopping bus instances: IBus
[16:35:49] dbug: MassTransit[0]
Stopping bus: rabbitmq://localhost/
[16:35:49] dbug: MassTransit[0]
Endpoint Stopping: rabbitmq://localhost/queue_name?temporary=true
[16:35:49] dbug: MassTransit[0]
Stopping receive transport: rabbitmq://localhost/queue_name?temporary=true
[16:35:49] dbug: MassTransit[0]
Stopping Consumer: rabbitmq://localhost/queue_name?temporary=true - amq.ctag-vQ8jfpQ6vh-2rQjq3wVd9A
[16:35:49] dbug: MassTransit.ReceiveTransport[0]
Consumer Cancel Ok: rabbitmq://localhost/queue_name?temporary=true - amq.ctag-vQ8jfpQ6vh-2rQjq3wVd9A
[16:35:49] dbug: MassTransit[0]
Stopping send transport: Contracts:Message
[16:35:49] dbug: MassTransit[0]
Endpoint Completed: rabbitmq://localhost/queue_name?temporary=true
[16:35:49] dbug: MassTransit[0]
Consumer completed amq.ctag-vQ8jfpQ6vh-2rQjq3wVd9A: 1 received, 1 concurrent
[16:35:49] dbug: MassTransit[0]
Disconnect: guest@localhost:5672/
[16:35:49] dbug: MassTransit[0]
Disconnected: guest@localhost:5672/
[16:35:49] info: MassTransit[0]
Bus stopped: rabbitmq://localhost/
[16:35:54] dbug: Microsoft.Extensions.Hosting.Internal.Host[4]
Hosting stopped

```
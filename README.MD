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

Bus is stopped immediately, request never arrives

# Log from producer app:

Producer is canceled about 2 seconds after message is sent. Message has 15 seconds timeout. We can see that right after requesting shutdown bus is stopped, and no message can be received.

```text
test
[09:37:17] info: MessageSender[0]
      Sending test
[09:37:17] dbug: MassTransit.Messages[0]
      SEND rabbitmq://localhost/Contracts:Message e0670000-9a3c-0005-33a4-08da6a22ac32 Contracts.Message
[09:37:19] info: Microsoft.Hosting.Lifetime[0]
      Application is shutting down...
[09:37:19] dbug: Microsoft.Extensions.Hosting.Internal.Host[3]
      Hosting stopping
[09:37:19] dbug: MassTransit.Transports.BusDepot[0]
      Stopping bus instances: IBus
[09:37:19] dbug: MassTransit[0]
      Stopping bus: rabbitmq://localhost/
[09:37:19] dbug: MassTransit[0]
      Endpoint Stopping: rabbitmq://localhost/mypc_ProducerApp_bus_hbuoyyr48oyykd4abdpgweuuyq?temporary=true
[09:37:19] dbug: MassTransit[0]
      Stopping receive transport: rabbitmq://localhost/mypc_ProducerApp_bus_hbuoyyr48oyykd4abdpgweuuyq?temporary=true
[09:37:19] dbug: MassTransit[0]
      Stopping Consumer: rabbitmq://localhost/mypc_ProducerApp_bus_hbuoyyr48oyykd4abdpgweuuyq?temporary=true - amq.ctag-Obv62l_ESfoyBt5zCxfWqA
[09:37:19] dbug: MassTransit.ReceiveTransport[0]
      Consumer Cancel Ok: rabbitmq://localhost/mypc_ProducerApp_bus_hbuoyyr48oyykd4abdpgweuuyq?temporary=true - amq.ctag-Obv62l_ESfoyBt5zCxfWqA
[09:37:19] dbug: MassTransit[0]
      Endpoint Completed: rabbitmq://localhost/mypc_ProducerApp_bus_hbuoyyr48oyykd4abdpgweuuyq?temporary=true
[09:37:19] dbug: MassTransit[0]
      Stopping send transport: Contracts:Message
[09:37:19] dbug: MassTransit.Messages[0]
      Consumer Completed: rabbitmq://localhost/mypc_ProducerApp_bus_hbuoyyr48oyykd4abdpgweuuyq?temporary=true: 2 received, 1 concurrent, amq.ctag-Obv62l_ESfoyBt5zCxfWqA
[09:37:19] dbug: MassTransit[0]
      Disconnect: guest@localhost:5672/
[09:37:19] dbug: MassTransit[0]
      Disconnected: guest@localhost:5672/
[09:37:19] info: MassTransit[0]
      Bus stopped: rabbitmq://localhost/
[09:37:47] info: MessageSender[0]
      Receive failed with exception: Timeout waiting for response, RequestId: e0670000-9a3c-0005-290f-08da6a22ac32
[09:37:47] dbug: Microsoft.Extensions.Hosting.Internal.Host[4]
      Hosting stopped
```
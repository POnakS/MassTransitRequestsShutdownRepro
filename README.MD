# MassTransit requests shutdown demo

Requests module prevents application from shutting down correctly.

Repro steps:

1. Run RabbitMq:
    ```
    docker run --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.10-management
    ```
2. Run producer app and consumer app
3. Type something in producer console
4. Verify that message arrived (Producer prints "Received " + typed message)
5. Try to shutdown producer app (i.e. press ctrl+c)
6. Application logs:
 
   ```
      Application is shutting down...
   ```

7. Application does not shutdown
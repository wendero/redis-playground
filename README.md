# Using Redis an Event Broker

## Playground
- docker run -itd --rm -p 6379:6379 --name redis redis
- docker exec -it redis redis-cli
- SUBSCRIBE SomethingHappened
- Run `Redis.Services.EventProcessor` project
- Run `Redis.Services.EventSource` project

## Details
- The EventProcessor will listen SomethingHappened channel
- Redis Client Tool (redis-cli) will listen SomethingHappened channel
- The EventSource service will send an event message to the Event Broker at the channel SomethingHappened each 5 seconds
- EventProcessor will consume received events
- Redis Client will consume/show the raw message

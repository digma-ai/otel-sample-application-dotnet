# Simple ASP.NET CORE MVC API with OpenTelemety

This simple single service application demonstrates how to include [Digma](https://github.com/digma-ai/digma) in the observability instrumentation.
The application mocks a very rudimentary 'money transfer' api, intended to showcase use cases where extral services may cause delays and complex errors may appear in runtime.

### The 'Money Transfer API'

This is a simple 'hello world' level application at the moment. The intent was create some depth of classes and processes with domain services as well as an application server, some trace hierarchy and simulated wait for external operations.


![image](https://user-images.githubusercontent.com/93863/163081141-9f99f13a-7220-4098-b4fc-d253d6511fcb.png)


### Sample.Client.Test utility

A simple test utility to create traffic, exceptions, and interesting situations to review with [Digma](https://github.com/digma-ai/digma). This is meant to be an open utility where additional use cases and scenarios could easily be defined.


#### Running the Money Tranfer API

```sh
cd Sample.MoneyTransfer.Api
docker compose up 
```
or

```sh
cd Sample.MoneyTransfer.Api
dotnet restore Sample.MoneyTransfer.API.csproj
dotnet run
```


### Running the tester script


```sh
cd Sample.Client.Test
docker compose up
```
or

```sh
cd Sample.Client.Test
dotnet restore ClientTester.csproj
dotnet run
```


### Further work

Additional planned work includes:

Integration with other OTEL instrumentable service to show how [Digma](https://github.com/digma-ai/digma) can glean important insights from the accumulated observability data:
- [ ] RabbitMQ
- [ ] Kafka
- [ ] Postgres db queries
- [ ] K8s cluster instrumentation

In addition, the architecture will become more complex as we separate out additional microservices.

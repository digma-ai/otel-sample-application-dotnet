# Simple ASP.NET CORE MVC API with OpenTelemety

This simple single service application demonstrates how to include [Digma](https://github.com/digma-ai/digma) in the observability instrumentation.
The application mocks a very rudimentary 'money transfer' api, intended to showcase use cases where extral services may cause delays and complex errors may appear in runtime.

### Further work

This is a simple 'hello world' application at the moment. 
More work will include integration with other OTEL instrumentable service to show how [Digma](https://github.com/digma-ai/digma)  can glean important insights from the accumulated observability data. 
In addition, the architecture calls for more microservices to be developed to demonstrate distributed traceability.

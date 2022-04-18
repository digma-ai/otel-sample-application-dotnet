package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"time"

	"example.com/user/user"
	"github.com/gorilla/mux"
	"go.opentelemetry.io/contrib/instrumentation/github.com/gorilla/mux/otelmux"

	// "go.opentelemetry.io/contrib/instrumentation/net/http/otelhttp"

	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"

	"go.opentelemetry.io/otel"
	"go.opentelemetry.io/otel/exporters/otlp/otlptrace/otlptracegrpc"
	"go.opentelemetry.io/otel/propagation"
	"go.opentelemetry.io/otel/sdk/resource"
	sdktrace "go.opentelemetry.io/otel/sdk/trace"
	semconv "go.opentelemetry.io/otel/semconv/v1.7.0"
)

var (
	port = "8010"
)

func initTracer() func() {
	ctx := context.Background()
	res, err := resource.New(ctx,
		resource.WithAttributes(
			// the service name used to display traces in backends
			semconv.ServiceNameKey.String("user-service"),
		),
	)
	handleErr(err, "failed to create resource")

	ctx, cancel := context.WithTimeout(ctx, time.Second*10)
	defer cancel()
	conn, err := grpc.DialContext(ctx, "localhost:4317", grpc.WithTransportCredentials(insecure.NewCredentials()), grpc.WithBlock())
	handleErr(err, "failed to create gRPC connection to collector")

	traceExporter, err := otlptracegrpc.New(ctx, otlptracegrpc.WithGRPCConn(conn))
	handleErr(err, "failed to create trace exporter")

	tracerProvider := sdktrace.NewTracerProvider(
		sdktrace.WithSampler(sdktrace.AlwaysSample()),
		sdktrace.WithResource(res),
		sdktrace.WithBatcher(traceExporter),
	)
	otel.SetTracerProvider(tracerProvider)
	otel.SetTextMapPropagator(propagation.NewCompositeTextMapPropagator(propagation.TraceContext{}, propagation.Baggage{}))
	return func() {
		// Shutdown will flush any remaining spans and shut down the exporter.
		handleErr(tracerProvider.Shutdown(ctx), "failed to shutdown TracerProvider")
	}
}

// func initTracerProvider() func() {
// 	ctx := context.Background()
// 	res, err := resource.New(ctx,
// 		resource.WithAttributes(
// 			// the service name used to display traces in backends
// 			semconv.ServiceNameKey.String("test-service"),
// 		),
// 	)
// 	handleErr(err, "failed to create resource")

// 	ctx, cancel := context.WithTimeout(ctx, time.Second*10)
// 	defer cancel()
// 	conn, err := grpc.DialContext(ctx, "localhost:4317", grpc.WithTransportCredentials(insecure.NewCredentials()), grpc.WithBlock())
// 	handleErr(err, "failed to create gRPC connection to collector")

// 	traceExporter, err := otlptracegrpc.New(ctx, otlptracegrpc.WithGRPCConn(conn))
// 	handleErr(err, "failed to create trace exporter")

// 	tracerProvider := sdktrace.NewTracerProvider(
// 		sdktrace.WithSampler(sdktrace.AlwaysSample()),
// 		sdktrace.WithResource(res),
// 		sdktrace.WithBatcher(traceExporter),
// 	)
// 	otel.SetTracerProvider(tracerProvider)
// 	// set global propagator to tracecontext (the default is no-op).
// 	otel.SetTextMapPropagator(propagation.NewCompositeTextMapPropagator(propagation.TraceContext{}, propagation.Baggage{}))
// 	return func() {
// 		// Shutdown will flush any remaining spans and shut down the exporter.
// 		handleErr(tracerProvider.Shutdown(ctx), "failed to shutdown TracerProvider")
// 	}
// }
// func getUsers(service user.Service) (http.Handler, error) {
// 	var handler http.Handler

// 	impl := func(w http.ResponseWriter, req *http.Request) {
// 		users, _ := service.List()
// 		response, _ := json.MarshalIndent(users, "", "  ")
// 		fmt.Fprint(w, response) //, traceURL, traceURL)
// 	}

// 	handler = http.HandlerFunc(impl)
// 	//handler = otelhttp.WithRouteTag("/", handler)
// 	handler = otelhttp.NewHandler(handler, "index-handler")
// 	return handler, nil
// }
// func addUser(service user.Service) (http.Handler, error) {
// 	var handler http.Handler

// 	impl := func(w http.ResponseWriter, req *http.Request) {
// 		users, _ := service.List()
// 		response, _ := json.MarshalIndent(users, "", "  ")
// 		fmt.Fprint(w, response) //, traceURL, traceURL)
// 	}

// 	handler = http.HandlerFunc(impl)
// 	//handler = otelhttp.WithRouteTag("/", handler)
// 	handler = otelhttp.NewHandler(handler, "index-handler")
// 	return handler, nil
// }

func main() {

	// injected latency
	if s := os.Getenv("EXTRA_LATENCY"); s != "" {
		v, err := time.ParseDuration(s)
		if err != nil {
			log.Fatalf("failed to parse EXTRA_LATENCY (%s) as time.Duration: %+v", v, err) //%+v: variant will include the struct’s field names.
		}
		user.ExtraLatency = v
		log.Printf("extra latency enabled (duration: %v)", v)
	} else {
		user.ExtraLatency = time.Duration(0)
	}

	shutdown := initTracer()
	defer shutdown()

	service := user.NewUserService()
	service.Init()
	controller := user.UserController{
		Service: service,
	}

	router := mux.NewRouter().StrictSlash(true)
	router.Use(otelmux.Middleware("user-service"))
	router.HandleFunc("/users", controller.Add).Methods("POST")
	router.HandleFunc("/users/{id}", controller.Get).Methods("GET")
	router.HandleFunc("/users", controller.All).Methods("GET")

	fmt.Println("listening on :" + port)
	err := http.ListenAndServe(":"+port, router)
	handleErr(err, "failed to listen & serve")

	// tracer := otel.Tracer("test-tracer")

	// // labels represent additional key-value descriptors that can be bound to a
	// // metric observer or recorder.
	// commonLabels := []attribute.KeyValue{
	// 	attribute.String("labelA", "chocolate"),
	// 	attribute.String("labelB", "raspberry"),
	// 	attribute.String("labelC", "vanilla"),
	// }

	// // work begins
	// _, span := tracer.Start(
	// 	context.Background(),
	// 	"CollectorExporter-Example",
	// 	trace.WithAttributes(commonLabels...))
	// defer span.End()
	// fmt.Println("hello")
}

// func indexHandler(w http.ResponseWriter, req *http.Request) {
// 	//	ctx := req.Context()

// 	//	traceURL := otelplay.TraceURL(trace.SpanFromContext(ctx))
// 	tmpl := `
// 	<html>
// 		<p>Here are some routes for you:</p>
// 		<ul>
// 			<li><a href="/hello/world">Hello world</a></li>
// 			<li><a href="/hello/foo-bar">Hello foo-bar</a></li>
// 		</ul>
// 		<p><a href="%s" target="_blank">%s</a></p>
// 	</html>
// 	`
// 	fmt.Fprint(w, tmpl) //, traceURL, traceURL)
// }

func handleErr(err error, message string) {
	if err != nil {
		log.Fatalf("%s: %v", message, err)
	}
}

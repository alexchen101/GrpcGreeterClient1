using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MahjongServer;

namespace GrpcGreeterClient1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await UnaryTest();
            await StreamTest();
        }

        static async Task UnaryTest()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "alex" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static async Task StreamTest()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Greeter.GreeterClient(channel);
            //var reply = await client.SayHelloAsync(new HelloRequest { Name = "alex" });

            using var call = client.StreamTest();

            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(response.Message);
                    // Echo messages sent to the service
                }
            });
            int index = new Random().Next(1, 1000);
            while (true)
            {
                var result = Console.ReadLine();
                if (string.IsNullOrEmpty(result))
                {
                    break;
                }

                await call.RequestStream.WriteAsync(new StreamRequest { Id = index, Message = result });
            }

            //Console.WriteLine($"{reply.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            await call.RequestStream.CompleteAsync();
        }
    }
}

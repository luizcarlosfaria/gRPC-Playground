using Grpc.Core;
using System;

namespace gRPCClient
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));


            var channel = new Channel("localhost:50051", ChannelCredentials.Insecure);

            var client = new gRPCHost.Greeter.GreeterClient(channel);


            var response = client.SayHello(new gRPCHost.HelloRequest() { Name = "gaGO.io" });


            Console.WriteLine(response.Message);


            await channel.ShutdownAsync();

        }
    }
}

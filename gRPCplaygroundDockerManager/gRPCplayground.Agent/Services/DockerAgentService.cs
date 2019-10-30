using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace gRPCplayground.Agent
{
    public class DockerAgentService : Services.ServicesBase
    {
        private readonly ILogger<DockerAgentService> _logger;
        private readonly DockerClient dockerClient;

        public DockerAgentService(ILogger<DockerAgentService> logger, DockerClient dockerClient)
        {
            _logger = logger;
            this.dockerClient = dockerClient;
        }


        public override async Task<GetContainersResponse> GetContainers(GetContainersRequest request, ServerCallContext context)
        {
            var containers = await this.dockerClient.Containers.ListContainersAsync(new Docker.DotNet.Models.ContainersListParameters()
            {
                All = request.ShowStopped ? true : false
            });

            var results = containers
            .Where(it =>
                string.IsNullOrWhiteSpace(request.Name)
                ||
                (string.Join(", ", it.Names).ToLowerInvariant().Contains(request.Name.ToLowerInvariant()))
            )
            .Select(it => new GetContainersResponse.Types.ContainerInfo()
            {
                Name = string.Join(", ", it.Names),
                Status = it.Status,
                State = it.State,
                Image = it.Image,
                Id = it.ID

            }).ToArray();

            var response = new GetContainersResponse();
            response.Results.Add(results);

            return response;
        }

        public override async Task GetLogStream(GetLogStreamRequest request, IServerStreamWriter<GetLogStreamResponse> responseStream, ServerCallContext context)
        {
            var logStream = await this.dockerClient.Containers.GetContainerLogsAsync(
                request.ContainerId,
                new Docker.DotNet.Models.ContainerLogsParameters()
                {
                    ShowStdout = true,
                    ShowStderr = true,
                    Follow = true
                },
                default);


            using (var reader = new StreamReader(logStream, System.Text.Encoding.UTF8))
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    string log = reader.ReadLine();

                    await responseStream.WriteAsync(new GetLogStreamResponse()
                    {
                        Text = log
                    });
                }
            }

        }


        //public  Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        //{
        //    return Task.FromResult(new HelloReply
        //    {
        //        Message = "Hello " + request.Name
        //    });
        //}
    }
}

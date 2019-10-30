FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["gRPCplayground.Agent/gRPCplayground.Agent.csproj", "gRPCplayground.Agent/"]
RUN dotnet restore "gRPCplayground.Agent/gRPCplayground.Agent.csproj"
COPY . .
WORKDIR "/src/gRPCplayground.Agent"
RUN dotnet build "gRPCplayground.Agent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gRPCplayground.Agent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gRPCplayground.Agent.dll"]
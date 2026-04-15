FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/RealtimeDemo.Api/RealtimeDemo.Api.csproj", "src/RealtimeDemo.Api/"]
RUN dotnet restore "src/RealtimeDemo.Api/RealtimeDemo.Api.csproj"

COPY . .
WORKDIR /src/src/RealtimeDemo.Api
RUN dotnet publish "RealtimeDemo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RealtimeDemo.Api.dll"]

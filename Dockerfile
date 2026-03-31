FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["EventHub/EventHub.Web.csproj", "EventHub/"]
COPY ["EventHub.Core/EventHub.Core.csproj", "EventHub.Core/"]
COPY ["EventHub.Infrastructure/EventHub.Infrastructure.csproj", "EventHub.Infrastructure/"]
COPY ["EventHub.Services/EventHub.Services.csproj", "EventHub.Services/"]
COPY ["EventHub.Repositories/EventHub.Repositories.csproj", "EventHub.Repositories/"]

RUN dotnet restore "EventHub/EventHub.Web.csproj"

COPY . .
WORKDIR "/src/EventHub"
RUN dotnet publish "EventHub.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "EventHub.Web.dll"]
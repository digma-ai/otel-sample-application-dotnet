FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["Sample.Client.Test/ClientTester.csproj", "Sample.Client.Test/"]
COPY ["Sample.MoneyTransfer.Api/Sample.MoneyTransfer.API.csproj", "Sample.MoneyTransfer.Api/"]
RUN dotnet restore "Sample.Client.Test/ClientTester.csproj"
COPY "Sample.MoneyTransfer.Api" "Sample.MoneyTransfer.Api"
COPY "Sample.Client.Test" "Sample.Client.Test"
WORKDIR "/src/Sample.Client.Test"
RUN dotnet build "ClientTester.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClientTester.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClientTester.dll"]

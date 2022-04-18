FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sample.MoneyTransfer.API.csproj", "./"]
RUN dotnet restore "Sample.MoneyTransfer.API.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Sample.MoneyTransfer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.MoneyTransfer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.MoneyTransfer.API.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY "Sample.MoneyTransfer.Api/Sample.MoneyTransfer.API.csproj" "Sample.MoneyTransfer.Api/Sample.MoneyTransfer.API.csproj"

RUN dotnet restore "Sample.MoneyTransfer.Api/Sample.MoneyTransfer.API.csproj"

COPY ["Sample.MoneyTransfer.Api/", "./"]


WORKDIR /Sample.MoneyTransfer.Api/

RUN dotnet publish --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sample.MoneyTransfer.API.dll"]

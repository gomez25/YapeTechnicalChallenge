FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/TransactionService/Presentation/TransactionService.Api/TransactionService.Api.csproj TransactionService/Presentation/TransactionService.Api/
COPY src/TransactionService/Application/TransactionService.Application/TransactionService.Application.csproj TransactionService/Application/TransactionService.Application/
COPY src/TransactionService/Infrastructure/TransactionService.Infrastructure/TransactionService.Infrastructure.csproj TransactionService/Infrastructure/TransactionService.Infrastructure/

RUN dotnet restore TransactionService/Presentation/TransactionService.Api/TransactionService.Api.csproj

COPY ./src/ .

WORKDIR /app/TransactionService/Presentation/TransactionService.Api
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TransactionService.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/AntiFraudService/Presentation/AntiFraudService.Api/AntiFraudService.Api.csproj ./AntiFraudService/Presentation/AntiFraudService.Api/
COPY src/AntiFraudService/Application/AntiFraudService.Application/AntiFraudService.Application.csproj ./AntiFraudService/Application/AntiFraudService.Application/
COPY src/AntiFraudService/Infrastructure/AntiFraudService.Infrastructure/AntiFraudService.Infrastructure.csproj ./AntiFraudService/Infrastructure/AntiFraudService.Infrastructure/

RUN dotnet restore AntiFraudService/Presentation/AntiFraudService.Api/AntiFraudService.Api.csproj

COPY ./src/ .
WORKDIR /app/AntiFraudService/Presentation/AntiFraudService.Api
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AntiFraudService.Api.dll"]

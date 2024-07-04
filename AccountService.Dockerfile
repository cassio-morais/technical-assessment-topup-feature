#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/AccountService ./AccountService
WORKDIR /src/AccountService/Backend.Account.Api
RUN dotnet restore .
RUN dotnet publish -c Release --property:PublishDir=/artifacts /p:UseAppHost=false --no-restore

#Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /artifacts .

ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
ENV ASPNETCORE_HTTP_PORTS=8082;

EXPOSE 8082

ENTRYPOINT ["dotnet", "Backend.Account.Api.dll"]
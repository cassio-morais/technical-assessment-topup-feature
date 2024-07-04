#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/TopUpService ./TopUpService
COPY ./src/FakeUserService ./FakeUserService
WORKDIR /src/TopUpService/TopUp.Backend.Api
RUN dotnet restore .
RUN dotnet publish -c Release --property:PublishDir=/artifacts /p:UseAppHost=false --no-restore

#Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /artifacts .

ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

EXPOSE 8080

ENTRYPOINT ["dotnet", "Backend.TopUp.Api.dll"]
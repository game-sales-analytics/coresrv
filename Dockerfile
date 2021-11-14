FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /source

COPY Coresrv.csproj .

RUN dotnet restore --runtime linux-x64 -p:PublishReadyToRun=true

COPY . .

RUN dotnet publish --self-contained true --configuration release --output /app --runtime linux-x64 --no-restore


# FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine-amd64
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app

COPY --from=build /app .

ENV PORT=8080

EXPOSE ${PORT}

ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["./Coresrv"]

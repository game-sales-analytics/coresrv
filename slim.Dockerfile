FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /source

COPY coresrv.sln .

COPY App/App.csproj ./App/

COPY Ping/Ping.csproj ./Ping/

RUN dotnet restore --runtime linux-musl-x64

COPY . .

RUN dotnet publish --self-contained true --configuration release --output /app --runtime linux-musl-x64 --no-restore

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine-amd64

WORKDIR /app

COPY --from=build /app .

ENV PORT=8080

EXPOSE ${PORT}

ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["./Coresrv"]

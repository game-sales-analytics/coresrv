# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG USER

ARG UID=1000

ARG GID=1000

RUN groupadd --gid ${GID} normals && useradd --create-home --gid ${GID} --uid ${UID} ${USER}

USER ${USER}

WORKDIR /home/${USER}/source

ARG PORT=8080

ENV ASPNETCORE_URLS=http://+:${PORT}/

ENV ASPNETCORE_ENVIRONMENT=Development

CMD [ "dotnet", "watch", "--no-hot-reload", "run", "--project", "App" ]
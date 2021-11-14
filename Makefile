clean:
	dotnet clean
.PHONY: clean

lint:
	dotnet format
.PHONY: lint

build:
	dotnet build
.PHONY: build

build-clean: clean build
.PHONY: build-clean

docker-down:
	DOCKER_BUILDKIT=1 docker compose --project-name gsa-core --file ./compose.yml down --rmi local
.PHONY: docker-down

docker-build:
	DOCKER_BUILDKIT=1 docker compose --project-name gsa-core --file ./compose.yml build --progress plain
.PHONY: docker-build

docker-up:
	docker compose --project-name gsa-core --file ./compose.yml up
.PHONY: docker-up

docker-build-up: docker-build docker-up
.PHONY: docker-build-up
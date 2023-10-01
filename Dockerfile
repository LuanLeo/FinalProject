# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
ARG src="TablesideOrdering/."
COPY *.sln .
COPY  ${src} ./TablesideOrdering/
WORKDIR /source/TablesideOrdering
RUN dotnet publish -c release -o /app

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "TablesideOrdering.dll"]

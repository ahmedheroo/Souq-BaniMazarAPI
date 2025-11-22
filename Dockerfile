
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Souq-BaniMazarAPI.sln ./
COPY Souq-BaniMazarAPI/ Souq-BaniMazarAPI/

RUN dotnet restore "Souq-BaniMazarAPI.sln"

COPY . .

RUN dotnet publish "Souq-BaniMazarAPI/Souq-BaniMazarAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish ./

EXPOSE 8080


ENTRYPOINT ["dotnet", "Souq-BaniMazarAPI.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY Elevador.Domain/*.csproj ./Elevador.Domain/
COPY Elevador.Service/*.csproj ./Elevador.Service/
COPY Elevador.API/*.csproj ./Elevador.API/

RUN dotnet restore

COPY . .

WORKDIR /src/Elevador.API
RUN dotnet build -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
COPY input.json .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Elevador.API.dll"]

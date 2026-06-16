FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY Firmeza.slnx ./
COPY src/Firmeza.Admin/Firmeza.Admin.csproj src/Firmeza.Admin/
COPY src/Firmeza.API/Firmeza.API.csproj src/Firmeza.API/
COPY tests/Firmeza.Tests/Firmeza.Tests.csproj tests/Firmeza.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish src/Firmeza.API/Firmeza.API.csproj \
    -c Release -o /app/publish \
    /p:ErrorOnDuplicatePublishOutputFiles=false
    
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8081

ENTRYPOINT ["dotnet", "Firmeza.API.dll"]
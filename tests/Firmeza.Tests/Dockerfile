FROM mcr.microsoft.com/dotnet/sdk:10.0 AS test

WORKDIR /app

COPY Firmeza.slnx ./
COPY src/Firmeza.Admin/Firmeza.Admin.csproj src/Firmeza.Admin/
COPY src/Firmeza.API/Firmeza.API.csproj src/Firmeza.API/
COPY tests/Firmeza.Tests/Firmeza.Tests.csproj tests/Firmeza.Tests/

RUN dotnet restore

COPY . .

RUN dotnet build --no-restore -c Release

ENTRYPOINT ["dotnet", "test", "--no-build", "-c", "Release", "--logger", "console;verbosity=normal"]
FROM mcr.microsoft.com/dotnet/sdk:6.0
MAINTAINER bernhard@emtek.at
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish -c Release -o out

EXPOSE 5000
ENV DOMAIN_NAME=vladik.local
ENV EXTERNAL_IP=192.168.1.19

ENTRYPOINT ["dotnet", "out/DummyServer.dll"]
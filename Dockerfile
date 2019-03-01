FROM microsoft/dotnet:2.2-sdk
MAINTAINER bernhard@emtek.at
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish -c Release -o out

EXPOSE 5000
ENV DummyDomainName=gree.emtek.at
ENV ExternalIP=172.16.1.1

ENTRYPOINT ["dotnet", "out/DummyServer.dll"]
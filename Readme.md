# Gree Dummy Server
This project makes it possible to block all Internet traffic from your Gree AC Devices. 
It is coded using the dotnet core 6.0 in C#. The Server must listen on port 5000, because the AC Devices sends there first command to this port.
There is also the possibility to run it in a docker container.

# Installation
* You have to run your own DNS Server. Add an entry for the Dummy Server like gree.example.com and point it to the IP Address running the Server.
#### Docker
* `docker run -d --restart=always -e DOMAIN_NAME=gree.example.com -e EXTERNAL_IP=172.16.1.1 -p 5000:5000 emtek/greeac-dummyserver:latest`
#### Bare Metal
* set the environment variables DOMAIN_NAME, and EXTERNAL_IP to the same values as your DNS Record
* start the Server with dotnet DummyServer.dll

# Device Configuration
For the Device Configuration use my ConfigTool. https://github.com/emtek-at/GreeAC-ConfigTool

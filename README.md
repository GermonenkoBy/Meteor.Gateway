# Meteor web API gateway

## Docker Build/Push

To build the image run the following command
```shell
docker build -f src/Meteor.Gateway.Api/Dockerfile -t sgermonenko/meteor-gateway:{version} --build-arg NUGET_USER={USERNAME} --build-arg NUGET_PASSWORD={PASSWORD} .
```
where:
- {version} is microservice release version
- {USERNAME} is private nuget feed username
- {PASSWORD} is private nuget feed password


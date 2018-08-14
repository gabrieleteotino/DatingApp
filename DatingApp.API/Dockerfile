
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY ./bin/Release/netcoreapp2.1/publish/. ./

ENTRYPOINT ["dotnet", "DatingApp.API.dll"]

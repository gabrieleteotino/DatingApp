
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env ./

ENTRYPOINT ["dotnet", "DatingApp.API.dll"]

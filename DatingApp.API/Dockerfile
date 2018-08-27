
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app

ENTRYPOINT ["dotnet", "DatingApp.API.dll"]

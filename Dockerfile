FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY *.sln .
COPY src/ValidataAPI.Api/*.csproj ./src/ValidataAPI.Api/
COPY src/ValidataAPI.ApiContract/*.csproj ./src/ValidataAPI.ApiContract/
COPY src/ValidataAPI.ApplicationService/*.csproj ./src/ValidataAPI.ApplicationService/
COPY src/ValidataAPI.Container/*.csproj ./src/ValidataAPI.Container/
COPY src/ValidataAPI.Domain/*.csproj ./src/ValidataAPI.Domain/
COPY src/ValidataAPI.Repository/*.csproj ./src/ValidataAPI.Repository/
COPY src/ValidataAPI.Utils/*.csproj ./src/ValidataAPI.Utils/
COPY test/ValidataAPI.Api.Tests/*.csproj ./test/ValidataAPI.Api.Tests/
COPY test/ValidataAPI.Utils.Tests/*.csproj ./test/ValidataAPI.Utils.Tests/
RUN dotnet restore

# copy full solution over
COPY . .
RUN dotnet build
FROM build AS testrunner
WORKDIR /app/test/ValidataAPI.Api.Tests
CMD ["dotnet", "test", "--logger:trx"]
# run the unit tests
FROM build AS test
WORKDIR /app/test/ValidataAPI.Api.Tests
RUN dotnet test --logger:trx
# run the component tests
FROM build AS componenttestrunner
WORKDIR /app/test/ValidataAPI.Utils.Tests
CMD ["dotnet", "test", "--logger:trx"]
# publish the API
FROM build AS publish
WORKDIR /app/src/ValidataAPI.Api
RUN dotnet publish -c Release -o out
# run the api
FROM mcr.microsoft.com/dotnet/aspnet:5.0  AS runtime
WORKDIR /app
COPY --from=publish /app/src/ValidataAPI.Api/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "ValidataAPI.Api.dll"]
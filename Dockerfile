FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY BlazorApp2.sln .
COPY BlazorApp2.csproj .
RUN dotnet restore "BlazorApp2.csproj"

# copy everything else and build app
COPY . .
WORKDIR /src
RUN dotnet build "BlazorApp2.csproj" -c release -o /app/build --no-restore
RUN dotnet publish "BlazorApp2.csproj" -c release -o /app/publish --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 as Run
WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 5085
ENTRYPOINT ["dotnet", "BlazorApp2.dll"]



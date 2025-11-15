# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS dev
WORKDIR /app

# restore
COPY .editorconfig /app/
COPY src/LifeSync.API/LifeSync.API.csproj LifeSync.API/

WORKDIR /app/LifeSync.API
RUN dotnet restore

EXPOSE 8080
CMD ["dotnet", "watch", "run", "--no-launch-profile", "--project LifeSync.API.csproj"]

# Stage 2: Build and publish
FROM dev AS build

COPY ./src /app
WORKDIR /app/LifeSync.API
RUN dotnet publish "LifeSync.API.csproj" -c Release -o /out

#Stage 3: Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS run
WORKDIR /app
COPY --from=build /out ./
ENTRYPOINT [ "dotnet", "LifeSync.API.dll" ]

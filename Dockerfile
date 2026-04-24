FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /data
ENV DATA_DIR=/data
EXPOSE 8080
ENV PORT=8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CriticsDB.dll"]

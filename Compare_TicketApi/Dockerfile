FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Compare_TicketApi.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# SQLite-Datei liegt in einem Volume, damit die Tickets Neustarts ueberleben.
RUN mkdir /data && chown $APP_UID /data
USER $APP_UID

ENV Urls=http://+:5300 \
    ConnectionStrings__Tickets="Data Source=/data/tickets.db"
EXPOSE 5300
ENTRYPOINT ["dotnet", "Compare_TicketApi.dll"]

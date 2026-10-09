FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Compare_Blazor_TicketTracker.csproj .
RUN dotnet restore
COPY . .
# Die API-URL steht in wwwroot/appsettings.json und wird vom Browser aufgerufen.
RUN dotnet publish -c Release -o /out --no-restore

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /out/wwwroot /usr/share/nginx/html
EXPOSE 80

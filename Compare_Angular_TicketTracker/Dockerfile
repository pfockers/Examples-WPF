FROM node:20-alpine AS build
WORKDIR /app
ENV NG_CLI_ANALYTICS=false
COPY package*.json ./
RUN npm ci
COPY . .
# Die API-URL steht in src/environments/environment.ts (Production-Build) und wird vom Browser aufgerufen.
RUN npm run build

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/angular-example/browser /usr/share/nginx/html
EXPOSE 80

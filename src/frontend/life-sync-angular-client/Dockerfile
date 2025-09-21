# Stage 1: Build
FROM node:20-alpine AS dev
WORKDIR /app
COPY package*.json ./
RUN npm install

FROM dev AS build
WORKDIR /app
COPY . .
RUN npm run build

# Stage 2: Serve
FROM nginx:alpine
VOLUME /var/cache/nginx
COPY --from=build /app/dist/life-sync-angular-client/browser /usr/share/nginx/html
COPY ./docker/nginx/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]
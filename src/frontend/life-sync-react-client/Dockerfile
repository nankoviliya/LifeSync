# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .

RUN npm run build

# Stage 2: Serve
FROM nginx:alpine AS production
VOLUME /var/cache/nginx
COPY --from=build /app/dist /usr/share/nginx/html
COPY ./docker/nginx/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]
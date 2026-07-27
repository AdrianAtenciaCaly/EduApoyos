# EduApoyosFront — production image (nginx)
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration=production

FROM nginx:1.27-alpine
# Nombre del proyecto en package.json / angular.json → edu-apoyos-front
COPY --from=build /app/dist/edu-apoyos-front/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
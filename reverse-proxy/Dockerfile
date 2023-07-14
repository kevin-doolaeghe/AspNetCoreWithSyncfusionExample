FROM nginx:alpine
COPY reverse-proxy/nginx.conf /etc/nginx/nginx.conf
RUN rm /etc/nginx/conf.d/default.conf

RUN apk update && apk add openssl
RUN openssl req -x509 -nodes -days 1461 \
    -subj "/C=FR/ST=Lille/O=SM/CN=example.site" \
    -addext "subjectAltName=DNS:example.site" -newkey rsa:2048 \
    -keyout /etc/ssl/private/app-cert.key -out /etc/ssl/certs/app-cert.crt

EXPOSE 80 443
ENTRYPOINT [ "nginx", "-g", "daemon off;" ]

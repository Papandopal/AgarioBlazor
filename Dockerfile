FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

# Устанавливаем cloudflared
RUN apt-get update && apt-get install -y wget && \
    wget -q https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb && \
    dpkg -i cloudflared-linux-amd64.deb || true && \
    apt-get install -yf && \
    rm cloudflared-linux-amd64.deb && \
    apt-get clean

# Копируем ВСЁ из текущей директории (где лежит Dockerfile)
COPY . .

# Делаем start.sh исполняемым
RUN chmod +x /app/start.sh

EXPOSE 8080

ENTRYPOINT ["/app/start.sh"]

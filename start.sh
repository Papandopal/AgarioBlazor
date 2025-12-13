#!/bin/bash

echo "Restoring packages..."
dotnet restore

echo "Building project..."
dotnet build -c Release

echo "Publishing..."
dotnet publish -c Release -o /app/out

echo "Starting Cloudflare Tunnel..."
cloudflared tunnel --url http://0.0.0.0:8080 &

echo "Starting ASP.NET app..."
cd /app/out
dotnet SPPR13_2.dll

#!/bin/bash

set -e

echo "🛠️ Створення .NET рішення та проєктів..."

# Create solution
dotnet new sln -n UrlShortenerApp

# Create backend Web API
mkdir -p backend
cd backend
dotnet new webapi -n UrlShortenerApp.API

# Create test project
dotnet new xunit -n UrlShortenerApp.Tests

# Go back to root and add projects to solution
cd ..
dotnet sln UrlShortenerApp.sln add backend/UrlShortenerApp.API/UrlShortenerApp.API.csproj
dotnet sln UrlShortenerApp.sln add backend/UrlShortenerApp.Tests/UrlShortenerApp.Tests.csproj

# Add project reference (tests -> API)
dotnet add backend/UrlShortenerApp.Tests/UrlShortenerApp.Tests.csproj reference backend/UrlShortenerApp.API/UrlShortenerApp.API.csproj

echo "✅ Backend готовий."

echo "🌐 Створення Angular frontend..."

# Create frontend Angular app
mkdir -p frontend
cd frontend
npx -y @angular/cli new url-shortener-angular --routing --style=css --skip-git

echo "✅ Frontend готовий."

echo "🔁 Налаштування проксі для Angular → ASP.NET API..."

cat <<EOP > url-shortener-angular/proxy.conf.json
{
  "/api": {
    "target": "https://localhost:5001",
    "secure": false,
    "changeOrigin": true,
    "logLevel": "debug"
  }
}
EOP

echo "📦 Встановлення залежностей Angular..."
cd url-shortener-angular
npm install

echo "✅ Успішна ініціалізація всього проєкту."
echo "🔹 Backend: backend/UrlShortenerApp.API"
echo "🔹 Frontend: frontend/url-shortener-angular"

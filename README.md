# DevOps ASP.NET 9 Demo

Minimalistyczny projekt DevOps oparty o ASP.NET 9 Web API, Docker i GitHub Actions.

## 🎯 Cel projektu
Celem projektu jest zaprezentowanie pełnego procesu DevOps:
- tworzenie aplikacji webowej
- testy jednostkowe
- CI/CD (GitHub Actions)
- konteneryzacja (Docker)
- wdrożenie do Azure
- monitoring i bezpieczeństwo

## 🧰 Technologie
- .NET 9 (ASP.NET Web API)
- Docker
- GitHub Actions (CI/CD)
- Azure App Service (Containers)
- Azure Application Insights
- Infrastructure as Code (Bicep)

## 📡 Endpointy
- `GET /` – status aplikacji
- `GET /products` – dane z zewnętrznego API (CoinGecko)
- `GET /health` - health check

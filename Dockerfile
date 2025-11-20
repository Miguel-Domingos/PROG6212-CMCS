# Build do frontend (Vue.js)
FROM node:18-alpine as frontend-build
WORKDIR /app
COPY prog6212-cmcs.client/package*.json ./
RUN npm install
COPY prog6212-cmcs.client/ ./
RUN npm run build

# Build do backend (ASP.NET Core)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /src
COPY PROG6212-CMCS.Server/PROG6212-CMCS.Server.csproj PROG6212-CMCS.Server/
RUN dotnet restore "PROG6212-CMCS.Server/PROG6212-CMCS.Server.csproj"
COPY . .
WORKDIR "/src/PROG6212-CMCS.Server"
RUN dotnet publish "PROG6212-CMCS.Server.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=backend-build /app/publish .
COPY --from=frontend-build /app/dist ./wwwroot
EXPOSE 80
ENTRYPOINT ["dotnet", "PROG6212-CMCS.Server.dll"]
# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6002 
EXPOSE 6012

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/User Service/Services.User.WebAPI/Services.User.WebAPI.csproj", "Microservices/User Service/Services.User.WebAPI/"]
COPY ["Microservices/User Service/Services.User.Application/Services.User.Application.csproj", "Microservices/User Service/Services.User.Application/"]
COPY ["Microservices/User Service/Services.User.Domain/Services.User.Domain.csproj", "Microservices/User Service/Services.User.Domain/"]
COPY ["Microservices/User Service/Services.User.Persistence/Services.User.Persistence.csproj", "Microservices/User Service/Services.User.Persistence/"]
RUN dotnet restore "./Microservices/User Service/Services.User.WebAPI/Services.User.WebAPI.csproj"
COPY . .
WORKDIR "/src/Microservices/User Service/Services.User.WebAPI"
RUN dotnet build "./Services.User.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Services.User.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["Microservices/User Service/Services.User.WebAPI/certs/userservice.pfx", "/https/aspnetapp.pfx"]

# Переменные окружения для Kestrel
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=${CERT_PASSWORD}
ENV ASPNETCORE_URLS=http://+:6002;https://+:6012

ENTRYPOINT ["dotnet", "Services.User.WebAPI.dll"]
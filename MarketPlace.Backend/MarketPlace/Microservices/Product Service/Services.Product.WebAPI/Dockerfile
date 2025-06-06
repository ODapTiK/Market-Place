# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/Product Service/Services.Product.WebAPI/Services.Product.WebAPI.csproj", "Microservices/Product Service/Services.Product.WebAPI/"]
COPY ["Microservices/Product Service/Services.Product.Application/Services.Product.Application.csproj", "Microservices/Product Service/Services.Product.Application/"]
COPY ["Microservices/Product Service/Services.Product.Domain/Services.Product.Domain.csproj", "Microservices/Product Service/Services.Product.Domain/"]
COPY ["Microservices/Product Service/Services.Product.Persistence/Services.Product.Persistence.csproj", "Microservices/Product Service/Services.Product.Persistence/"]
RUN dotnet restore "./Microservices/Product Service/Services.Product.WebAPI/Services.Product.WebAPI.csproj"
COPY . .
WORKDIR "/src/Microservices/Product Service/Services.Product.WebAPI"
RUN dotnet build "./Services.Product.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Services.Product.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["Microservices/Product Service/Services.Product.WebAPI/certs/productservice.pfx", "/https/aspnetapp.pfx"]

# Переменные окружения для Kestrel
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=${CERT_PASSWORD}
ENV ASPNETCORE_URLS=http://+:6004;https://+:6014

EXPOSE 6004 6014

ENTRYPOINT ["dotnet", "Services.Product.WebAPI.dll"]
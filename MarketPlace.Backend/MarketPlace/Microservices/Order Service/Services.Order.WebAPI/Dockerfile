# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6003


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/Order Service/Services.Order.WebAPI/Services.Order.WebAPI.csproj", "Microservices/Order Service/Services.Order.WebAPI/"]
COPY ["Microservices/Order Service/Services.Order.Application/Services.Order.Application.csproj", "Microservices/Order Service/Services.Order.Application/"]
COPY ["Microservices/Order Service/Services.Order.Domain/Services.Order.Domain.csproj", "Microservices/Order Service/Services.Order.Domain/"]
COPY ["Microservices/Order Service/Services.Order.Persistence/Services.Order.Persistence.csproj", "Microservices/Order Service/Services.Order.Persistence/"]
RUN dotnet restore "./Microservices/Order Service/Services.Order.WebAPI/Services.Order.WebAPI.csproj"
COPY . .
WORKDIR "/src/Microservices/Order Service/Services.Order.WebAPI"
RUN dotnet build "./Services.Order.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Services.Order.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Services.Order.WebAPI.dll"]
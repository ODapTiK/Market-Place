# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6001


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Microservices/Authorization Service/Services.Authorization.WebAPI/Services.Authorization.WebAPI.csproj", "Microservices/Authorization Service/Services.Authorization.WebAPI/"]
COPY ["Microservices/Authorization Service/Services.Authorization.Application/Services.Authorization.Application.csproj", "Microservices/Authorization Service/Services.Authorization.Application/"]
COPY ["Microservices/Authorization Service/Services.Authorization.Domain/Services.Authorization.Domain.csproj", "Microservices/Authorization Service/Services.Authorization.Domain/"]
COPY ["Microservices/Authorization Service/Services.Authorization.Persistence/Services.Authorization.Persistence.csproj", "Microservices/Authorization Service/Services.Authorization.Persistence/"]
RUN dotnet restore "./Microservices/Authorization Service/Services.Authorization.WebAPI/Services.Authorization.WebAPI.csproj"
COPY . .
WORKDIR "/src/Microservices/Authorization Service/Services.Authorization.WebAPI"
RUN dotnet build "./Services.Authorization.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Services.Authorization.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Services.Authorization.WebAPI.dll"]
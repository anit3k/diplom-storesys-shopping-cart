# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ShoppingCart.API/ShoppingCart.API.csproj", "src/ShoppingCart.API/"]
COPY ["src/ShoppingCart.Application/ShoppingCart.Application.csproj", "src/ShoppingCart.Application/"]
COPY ["src/ShoppingCart.Domain/ShoppingCart.Domain.csproj", "src/ShoppingCart.Domain/"]
COPY ["src/ShoppingCart.Infrastructure/ShoppingCart.Infrastructure.csproj", "src/ShoppingCart.Infrastructure/"]

RUN dotnet restore "src/ShoppingCart.API/ShoppingCart.API.csproj"

COPY . .
WORKDIR "/src/src/ShoppingCart.API"
RUN dotnet build "ShoppingCart.API.csproj" -c Release -o /app/build

# ---- Publish stage ----
FROM build AS publish
RUN dotnet publish "ShoppingCart.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---- Final stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

USER $APP_UID

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingCart.API.dll"]
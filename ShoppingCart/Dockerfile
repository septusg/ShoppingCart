# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製專案
COPY . .

# restore 指定 csproj
RUN dotnet restore ShoppingCart.csproj

# build & publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:7055
EXPOSE 7055
ENTRYPOINT ["dotnet", "ShoppingCart.dll"]

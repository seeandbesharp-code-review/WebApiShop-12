# שלב 1: בנייה (Build)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. העתקת קבצי הפרויקט (.csproj) של כל השכבות לצורך Restore יעיל
COPY ["WebApiShop/WebApiShop.csproj", "WebApiShop/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["DTOs/DTOs.csproj", "DTOs/"]

# ביצוע Restore לכל הפרויקטים דרך קובץ ה-API הראשי
RUN dotnet restore "WebApiShop/WebApiShop.csproj"

# 2. העתקת כל שאר קבצי הקוד של כל השכבות
COPY . .

# בניית פרויקט ה-API (זה יבנה אוטומטית גם את כל הפרויקטים התלויים בו)
RUN dotnet build "WebApiShop/WebApiShop.csproj" -c Release -o /app/build

# שלב 2: פרסום (Publish)
FROM build AS publish
RUN dotnet publish "WebApiShop/WebApiShop.csproj" -c Release -o /app/publish /p:UseAppHost=false

# שלב 3: הרצה (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiShop.dll"]
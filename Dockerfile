FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["proje_mvc/proje_mvc.csproj", "proje_mvc/"]
RUN dotnet restore "proje_mvc/proje_mvc.csproj"
COPY . .
WORKDIR "/src/proje_mvc"
RUN dotnet build "proje_mvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "proje_mvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "proje_mvc.dll"]
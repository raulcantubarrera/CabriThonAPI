FROM mcr.microsoft.com/dotnet/aspnet:9.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 5272

ENV ASPNETCORE_URLS=http://+:5272

FROM mcr.microsoft.com/dotnet/sdk:9.0-nanoserver-1809 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/CabriThonAPI.WebAPI/CabriThonAPI.WebAPI.csproj", "src/CabriThonAPI.WebAPI/"]
RUN dotnet restore "src\CabriThonAPI.WebAPI\CabriThonAPI.WebAPI.csproj"
COPY . .
WORKDIR "/src/src/CabriThonAPI.WebAPI"
RUN dotnet build "CabriThonAPI.WebAPI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CabriThonAPI.WebAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CabriThonAPI.WebAPI.dll"]

# Use the official .NET SDK image to build your application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CabriThonAPI.csproj", "./"] # Adjust this if your csproj is in a subfolder
RUN dotnet restore "CabriThonAPI.csproj"
COPY . .
RUN dotnet publish "CabriThonAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET ASP.NET runtime image to run your application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CabriThonAPI.dll"]

# Stage 1: Build and Publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy BOTH shopping lists into their correct folders
COPY ["API/API.csproj", "API/"]
COPY ["Common/Common.csproj", "Common/"]

# Restore the main API project (this will automatically restore Common too)
RUN dotnet restore "API/API.csproj"

# Copy all the actual C# code (from both API and Common)
COPY . .

# Publish the API project
RUN dotnet publish "API/API.csproj" -c Release -o /app/publish

# Stage 2: Run the App
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Tell ASP.NET to specifically use port 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

# The compiled app takes the name of the .csproj file!
ENTRYPOINT ["dotnet", "API.dll"]

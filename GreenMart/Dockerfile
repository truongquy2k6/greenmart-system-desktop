FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy solution and project files first to restore dependencies
COPY GreenMart.sln ./
COPY GreenMart.API/GreenMart.API.csproj ./GreenMart.API/
COPY BUS/BUS.csproj ./BUS/
COPY DAL/DAL.csproj ./DAL/
COPY DTO/DTO.csproj ./DTO/
RUN dotnet restore GreenMart.API/GreenMart.API.csproj

# Copy everything else and build the project
COPY . ./
RUN dotnet publish GreenMart.API/GreenMart.API.csproj -c Release -o out

# Create runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "GreenMart.API.dll"]

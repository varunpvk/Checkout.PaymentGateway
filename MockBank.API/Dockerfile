#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MockBank.API/MockBank.API.csproj", "MockBank.API/"]
COPY ["MockBank.PaymentsRepository/MockBank.PaymentsRepository.csproj", "MockBank.PaymentsRepository/"]
RUN dotnet restore "MockBank.API/MockBank.API.csproj"
COPY . .
WORKDIR "/src/MockBank.API"
RUN dotnet build "MockBank.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MockBank.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MockBank.API.dll"]
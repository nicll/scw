#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build
WORKDIR "/src/ScwSvc"
COPY ["ScwSvc.csproj", "."]
RUN dotnet restore "ScwSvc.csproj"
COPY . .
RUN dotnet build "ScwSvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScwSvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScwSvc.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# init base ASP.NET Core image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR "/app"
EXPOSE 80/tcp
EXPOSE 443/tcp

# init .NET SDK and build ScwSvc
# creates temporary dir /src/ScwSvc
# copies project file and restores packages
# copies remaining files and builds project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR "/src/ScwSvc"
COPY ["ScwSvc.csproj", "."]
RUN dotnet restore "ScwSvc.csproj"
COPY [".", "."]
RUN dotnet build "ScwSvc.csproj" -c Release -o /app/build

# publish ScwSvc
FROM build AS publish
RUN dotnet publish "ScwSvc.csproj" -c Release -o /app/publish

# copy published ScwSvc to base image
FROM base AS final
WORKDIR "/app"
COPY --from=publish ["/app/publish", "."]
ENTRYPOINT ["dotnet", "ScwSvc.dll"]

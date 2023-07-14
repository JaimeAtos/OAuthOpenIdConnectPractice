FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Presentation/CodeFlowsWithOpenIdConnect/CodeFlowsWithOpenIdConnect.csproj", "src/Presentation/CodeFlowsWithOpenIdConnect/"]
RUN dotnet restore "src/Presentation/CodeFlowsWithOpenIdConnect/CodeFlowsWithOpenIdConnect.csproj"
COPY . .
WORKDIR "/src/src/Presentation/CodeFlowsWithOpenIdConnect"
RUN dotnet build "CodeFlowsWithOpenIdConnect.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeFlowsWithOpenIdConnect.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeFlowsWithOpenIdConnect.dll"]

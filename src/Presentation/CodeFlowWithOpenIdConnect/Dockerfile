FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Presentation/CodeFlowWithOpenIdConnect/CodeFlowWithOpenIdConnect.csproj", "src/Presentation/CodeFlowWithOpenIdConnect/"]
RUN dotnet restore "src/Presentation/CodeFlowWithOpenIdConnect/CodeFlowWithOpenIdConnect.csproj"
COPY . .
WORKDIR "/src/src/Presentation/CodeFlowWithOpenIdConnect"
RUN dotnet build "CodeFlowWithOpenIdConnect.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeFlowWithOpenIdConnect.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeFlowWithOpenIdConnect.dll"]

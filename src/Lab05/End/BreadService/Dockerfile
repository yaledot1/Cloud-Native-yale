FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["BreadService/BreadService.csproj", "BreadService/"]
RUN dotnet restore "BreadService/BreadService.csproj"
COPY . .
WORKDIR "/src/BreadService"
RUN dotnet build "BreadService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BreadService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BreadService.dll"]

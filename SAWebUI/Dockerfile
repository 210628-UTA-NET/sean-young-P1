FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 44337

ENV ASPNETCORE_URLS=http://+:44337

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SAWebUI/SAWebUI.csproj", "SAWebUI/"]
RUN dotnet restore "SAWebUI\SAWebUI.csproj"
COPY . .
WORKDIR "/src/SAWebUI"
RUN dotnet build "SAWebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SAWebUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SAWebUI.dll"]

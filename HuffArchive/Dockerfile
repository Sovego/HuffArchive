FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["HuffArchive/HuffArchive.csproj", "HuffArchive/"]
RUN dotnet restore "HuffArchive/HuffArchive.csproj"
COPY . .
WORKDIR "/src/HuffArchive"
RUN dotnet build "HuffArchive.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HuffArchive.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HuffArchive.dll"]

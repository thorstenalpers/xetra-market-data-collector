FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build

COPY . .

RUN dotnet publish "MarketData/MarketData.API/MarketData.API.csproj" -c Release -o /app/publish  /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
EXPOSE 80
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT Production
ENV TZ=Europe/Berlin

RUN apt-get update && \
    apt-get install -yq tzdata && \
    ln -fs /usr/share/zoneinfo/Europe/Berlin /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone 


ENTRYPOINT ["dotnet", "MarketData.API.dll", "--environment=Production"]

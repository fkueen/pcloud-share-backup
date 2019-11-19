FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS publish
WORKDIR /src

COPY . .
WORKDIR /src/PCloud.Backup
RUN dotnet publish -c Release -r linux-arm -o /app --self-contained true /p:PublishTrimmed=true
RUN mkdir /app/data

FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.0-buster-slim-arm32v7 AS runtime
WORKDIR /app
COPY --from=publish /app .
VOLUME /app/data
ENTRYPOINT ["./PCloud.Backup"]

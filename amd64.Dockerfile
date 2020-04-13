FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS publish
WORKDIR /src

COPY ./*.sln ./
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done \
  && dotnet restore -r linux-x64

COPY . .

WORKDIR /src/PCloud.Backup
RUN dotnet publish -c Release -r linux-x64 -o /app --no-restore --self-contained true /p:PublishTrimmed=true  /p:PublishSingleFile=true \
  && mkdir /app/data

FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1 AS runtime
WORKDIR /app
COPY --from=publish /app .
VOLUME /app/data
ENTRYPOINT ["./PCloud.Backup"]

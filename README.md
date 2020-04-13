# Backup to shared pCloud folder

## Usage

Create the upload link from pCloud account, you'll get the following url:
`https://my.pcloud.com/#page=puplink&code=PCLOUD_SHARE_CODE`

Run with docker:
```sh
$ docker run -e BackupCronExpression=BACKUP_CRON_EXPRESSION -e PCloudCode=PCLOUD_SHARE_CODE -v PATH_TO_FOLDER:/app/data:ro kueen/pcloud:arm32v7
```

Run with docker-compose:
```yml
version: "3.7"
services:
  backup:
    image: kueen/pcloud:arm32v7
    environment:
      - PCloudCode=PCLOUD_SHARE_CODE
      - BackupCronExpression=0 0 6 * * ?
    volumes:
      - PATH_TO_FOLDER:/app/data:ro
```

## Container Options

This container have some options available via environment variables:

- **PCloudCode**: Code parameter from pCloud upload link
- **BackupCronExpression**: Cron expression (default `0 0 0 * * ?`)
- **SenderName**: Sender name (default `docker`)
- **BackupPattern**: File pattern to backup (defsault `*`)
- **BackupFolder**: Container folder (default `data`)
- **UploadBufferSize**: Http request buffer size in bytes (default `5242880`)
- **UploadTimeout**: Http request timeout (default `00:10:00`)







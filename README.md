# Backup to shared pCloud folder

## Usage

Create the upload link from pCloud account, you'll get the following url:
`https://my.pcloud.com/#page=puplink&code=PCLOUD_SHARE_CODE`

Run:
```sh
$ docker run -e BackupCronExpression=BACKUP_CRON_EXPRESSION -e PCloudCode=PCLOUD_SHARE_CODE -v PATH_TO_FOLDER:/app/data:ro kueen/pcloud:arm32v7
```

## Container Options

This container have some options available via environment variables:

- **BackupCronExpression**: Cron expression like `0 0 0 * * ?` etc.
- **PCloudCode**: Code parameter from pCloud upload link
- **SenderName**: Sender name (optional)





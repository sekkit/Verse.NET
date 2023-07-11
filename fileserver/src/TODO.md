# File server : TODO

## Level 1

- Directory/file browsing
- Download files

## Level 2

- Rich file info (name, size, last change)
- Download/view option

New directory info model:
```json
{
    "Parent": "String",
    "Base": "String",
    "Dirs": [
        {
            "Name": "String",
            "LastModified": "String"
        }
    ],
    "Files": [
        {
            "Name": "String",
            "Size": "Number",
            "LastModified": "String"
        }
    ]
}
```
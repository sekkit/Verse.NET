# File server

## Summary

This is a simple file server app built with [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) framework. You can easily browse directories and files with this app. To get started,
1. Download [.NET sdk](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) (this project requires .NET sdk 5.0)
2. Build the project: go to FileServerApp and `dotnet build`
3. Go to build output (something like bin/Debug/netcoreapp3.1/)
4. Put your files/folders to wwwroot directory in build output
5. Launch the executable file (FileServerApp)
6. Type `http://localhost:5000/` in your browser
7. Enjoy!

## What's inside

This application's backend is written in C#. Frontend is written using [Vue.js](https://vuejs.org) framework.

File Program.cs is used to initialize the web host builder and build the application.

File Startup.cs is used to configure the application on startup.

File Handlers.cs contains 3 request delegates:
- `DefaultGet` - default fallback delegate, sends index.html to client.
- `FileServer` - sends a file or directory info depending on request path.
- `AssetsDelivery` - sends an asset file (css, icons, ...) to client.

Files FsFileEntry.cs and FsDirectoryEntry contain classes with minimum necessary fields representing files and directories respectively.

File FsExtensions.cs contains some imperative code to get file system entries of current directory; and auxiliary methods to get relative path and to make FsFileEntry and FsDirectoryEntry objects from standard FileInfo and DirectoryInfo objects.

File HttpResponseExtensions.cs contains extension methods:
- `WriteAsJsonAsync<T>` - to send an object as JSON to given HTTP response.
- `SendFileAttachmentAsync` - to send a file as attachment (file should be saved).
- `SendFileAssetAsync` - to deliver an asset file (css, icon, ...).
- `NotFound` - to send a standard 404 response.

File index.html is the only frontend page.

## License 

MIT LICENSE: you can do whatever you want with this project.

## Contributing

Idk, just open a pull request...

## Authors

```json
{
  "name": "Dmitriy Naumov",
  "last-contribution-date": "2022-02-07",
  "github": "https://github.com/dmitriynaumov1024",
  "email": "naumov1024@gmail.com"
}
```

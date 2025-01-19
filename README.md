# SmugMug Sync for Windows

This project is a command line application which keeps folders from a source windows folder in sync with Albums on the photo sharing service [SmugMug](https://www.smugmug.com/) using their [SmugMug API 1.3.0](https://api.smugmug.com/services/api/json/1.3).

## Project Goals

1. Upload and sync file changes from local folders to remote SmugMug Galleries
1. Process Keywords for Images and Videos up to SmugMug, splitting up keyword trees
1. Simple use - Run program (no params) for full sync of any new folders/galleries
1. Any image updates (keywords, new files, removed files) need to sync to SmugMug
1. Syncing must persist regardless of SmugMug Organize App changes (moving folder in SmugMug, settings, etc)
1. Changes that happen to images in SmugMug (rotating, new keywords) should reverse sync back to local system

## History

This project started in 2012 as I had many folders and wanted to sync them in a no hassle manner.  I found the following workflow and have used it to this day:

1. Work on photos in Adobe Lightroom, Export JPG output files to a local folder
1. Use a separate tool (custom) to add captions to each file, and format the filenames
1. Use another tool to apply tags to the files
1. Copy the finished folder to my network drive in with a nice foldername, in a folder by year.
1. Run the *SmugMugSync* tool, which transfers any new folders to SmugMug in a private gallery for me to review
1. In SmugMug Organizer, apply template, select feature image(s) and move folder to final gallery location.

This may seem like a number of steps, but this SmugMug Sync tool, removes the hassle with any image updates after the initial changes, and it also has special processing to split tags and can pull tags associated with videos (on Windows).

Overall, I have 50k files, and this tool will in about 30s minus the new folder syncing (which is variable based on images/videos being uploaded).

## Changelog

2012 - App initially written with .Net 4.6.
       + various years of tweaks

2023-09 - App was rewritten to use .Net Core 7.0, VS Code, and includes full unit testing. Integration tests for the core library, and Moq testing for the primary application. Split out the secrets and made it more shareable.

## How to use Sync Tool

1. Request a developer key for your app [SmugMug Application Keys](https://api.smugmug.com/api/developer)
1. Put your new keys in appsettings.json (apikey/apisecret) or Environment Variables
1. Build and run "SmugMugCoreSync GENERATEKEYS" to generate your user keys to use with your app.
1. Put your user keys into the appsettings.json file or Environment Variables.
1. Settings to drive app are all in appsettings.json, instructions will be coming shortly.

### Environment Variables for User and API Keys (these will override the appsettings file values, restart VSCode after adding)
       SmugMugCoreSync::ApiKey
       SmugMugCoreSync::ApiSecret
       SmugMugCoreSync::UserAuthToken 
       SmugMugCoreSync::UserAuthSecret

## Future Planss

1. Document the appsettings file for easier usage
2. Move from the 1.3 API to 2.0 API

## Code Details

### Dependencies

1. Setup Extensions: C#, C# Dev Kit, .NET Install Tool, Code Spell Checker, Coverage Gutters
1. Install the Microsoft .NET SDK 9.X (should be triggered via VS Code)

### Testing
1. A task exists to run tests, which includes coverage.
1. TestSmugmugCoreSync is the core application, dependencies are mocked.
1. TestSmugmugCoreNetAPI will validate key Smugmug APIs, this will require your User / API keys to be setup (see above) and will use your Smugmug Account to run APIs (craeting and cleaning up what it works with).
1. For coverage details (if you want something more than Test Manager Coverage already):
       dotnet tool install -g dotnet-reportgenerator-globaltool
       Command to use: *task Generate Coverage Report*
       


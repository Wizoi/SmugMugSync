using SmugMug.Net.Core;
using SmugMug.Net.Data;
using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Repositories;
using SmugMugCoreSync.Utility;
using System.Diagnostics;
using System.IO.Abstractions;
using static System.Formats.Asn1.AsnWriter;

/*
 *  Initialize Logging and core App
 */
var appSettings = new SyncAppSettings();
var keySettings = appSettings.KeySecrets;
var appLogger = new AppLogger(appSettings.Logging);
appLogger.SetupAppLog();

var smCore = new SmugMug.Net.Core.SmugMugCore(
    userAuthToken: keySettings.UserAuthToken, userAuthSecret: keySettings.UserAuthSecret,
    apiKey: keySettings.ApiKey, apiSecret: keySettings.ApiSecret);

/* Load Source Files */
var sourceFolders = new SmugMugCoreSync.Repositories.SourceFolderRepository(fileSystem:new FileSystem(), folderConfig: appSettings.FolderSyncPaths);
sourceFolders.PopulateSourceFoldersAndFiles();

/* Load Remote Albums */
var remoteAlbums = new SmugMugCoreSync.Repositories.TargetAlbumRepository(core: smCore, folderConfig: appSettings.FolderSyncPaths);
remoteAlbums.PopulateTargetAlbums();
remoteAlbums.VerifyLinkedFolders(appSettings.RuntimeFlags, sourceFolders);
remoteAlbums.ResyncAlbumTitlesFromFolderNames(appSettings.RuntimeFlags, sourceFolders);
remoteAlbums.SyncNewFolders(appSettings.RuntimeFlags, sourceFolders);
remoteAlbums.SyncExistingFolders(appSettings.RuntimeFlags, sourceFolders);
remoteAlbums.SyncFolderFiles(appSettings.RuntimeFlags, sourceFolders);

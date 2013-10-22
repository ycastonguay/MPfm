// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

#if !IOS && !ANDROID && !WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using Spring.IO;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;
using Spring.Social.OAuth1;

namespace MPfm.Library.Services
{
    public class DropboxCoreService : ICloudLibraryService
    {
        //private const string DropboxAppKey = "6tc6565743i743n";
        //private const string DropboxAppSecret = "fbkt3neevjjl0l2";
        public const string DropboxAppKey = "m1bcpax276elhfi";
        public const string DropboxAppSecret = "2azbuj2eelkranm";

        private readonly ILibraryService _libraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private DropboxServiceProvider _dropboxServiceProvider;
        private OAuthToken _oauthToken;

        public event DropboxDataChanged OnDropboxDataChanged;

        public DropboxCoreService(ILibraryService libraryService, IAudioFileCacheService audioFileCacheService,
            ISyncDeviceSpecifications deviceSpecifications)
        {
            _libraryService = libraryService;
            _audioFileCacheService = audioFileCacheService;
            _deviceSpecifications = deviceSpecifications;
            Initialize();
        }

        private void Initialize()
        {
        }

        public bool HasLinkedAccount { get; private set; }

        public void LinkApp(object view)
        {
            try
            {
                _dropboxServiceProvider = new DropboxServiceProvider(DropboxAppKey, DropboxAppSecret,
                    AccessLevel.AppFolder);

                /* OAuth 1.0 'dance' */

                // Authorization without callback url
                Console.Write("Getting request token...");
                _oauthToken = _dropboxServiceProvider.OAuthOperations.FetchRequestTokenAsync(null, null).Result;
                Console.WriteLine("Done - FetchRequestToken - secret: {0} value: {1}", _oauthToken.Secret,
                    _oauthToken.Value);

                OAuth1Parameters parameters = new OAuth1Parameters();
                //parameters.Add("locale", CultureInfo.CurrentUICulture.IetfLanguageTag); // for a localized version of the authorization website
                string authenticateUrl = _dropboxServiceProvider.OAuthOperations.BuildAuthorizeUrl(_oauthToken.Value,
                    parameters);
                Console.WriteLine("Redirect user for authorization");
                Process.Start(authenticateUrl);
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    if (ex is DropboxApiException)
                    {
                        Console.WriteLine(ex.Message);
                        return true;
                    }
                    return false;
                });
            }

        }

        public void ContinueLinkApp()
        {
            try
            {
                Console.Write("Getting access token...");
                AuthorizedRequestToken requestToken = new AuthorizedRequestToken(_oauthToken, null);
                OAuthToken oauthAccessToken =
                    _dropboxServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
                Console.WriteLine("Done - FetchAccessToken - secret: {0} value: {1}", oauthAccessToken.Secret,
                    oauthAccessToken.Value);

                //OAuthToken oauthAccessToken2 = new OAuthToken("z20l3g6vs5bbvqcr", "b8eiq09w1gxsyad");
                /* API */

                IDropbox dropbox = _dropboxServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                //IDropbox dropbox = dropboxServiceProvider.GetApi(oauthAccessToken2.Value, oauthAccessToken2.Secret);
                //dropbox.Locale = CultureInfo.CurrentUICulture.IetfLanguageTag;

                DropboxProfile profile = dropbox.GetUserProfileAsync().Result;
                Console.WriteLine("Hi " + profile.DisplayName + "!");

                // Use step by step debugging, or not             

                //DeltaPage deltaPage = dropbox.DeltaAsync(null).Result;
                //dropbox.AccessLevel
                //Entry createFolderEntry = dropbox.CreateFolderAsync("Spring Social").Result;

                Task.Factory.StartNew(() =>
                {
                    string cursor = null;
                    while (true)
                    {
                        DeltaPage deltaPage = dropbox.DeltaAsync(cursor).Result;
                        cursor = deltaPage.Cursor;
                        Console.WriteLine("Delta check - entries: {0} cursor: {1} hasMore: {2} reset: {3}",
                            deltaPage.Entries.Count, deltaPage.Cursor, deltaPage.HasMore, deltaPage.Reset);
                        if (deltaPage.Entries.Count > 0)
                        {
                            //FileRef fileRef = dropbox.CreateFileRefAsync("File.txt").Result;
                            dropbox.DownloadFileAsync("File.txt")
                                .ContinueWith(task =>
                                {
                                    Console.WriteLine("File '{0}' downloaded ({1})", task.Result.Metadata.Path,
                                        task.Result.Metadata.Size);
                                    // Save file to "C:\Spring Social.txt"
                                    //using (FileStream fileStream = new FileStream(@"C:\Spring Social.txt", FileMode.Create))
                                    //{
                                    //    fileStream.Write(task.Result.Content, 0, task.Result.Content.Length);
                                    //}
                                });
                        }
                        Thread.Sleep(1000);
                    }
                });

                for (int a = 0; a < 10; a++)
                {
                    Thread.Sleep(4000);
                    string text = string.Format("Windows - Step {0}", a);
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
                    Console.WriteLine(text);

                    //var test = new ByteArrayResource(bytes);
                    Entry uploadFileEntry = dropbox.UploadFileAsync(
                        //new AssemblyResource("assembly://Spring.ConsoleQuickStart/Spring.ConsoleQuickStart/File.txt"),
                        new ByteArrayResource(bytes),
                        "File.txt", true, null, CancellationToken.None).Result;
                }
                //FileRef fileRef = dropbox.CreateFileRefAsync("File.txt").Result;
                ////FileRef fileRef = dropbox.CreateFileRefAsync("Spring Social/File.txt").Result;
                //Entry copyRefEntry = dropbox.CopyFileRefAsync(fileRef.Value, "Spring Social/File_copy_ref.txt").Result;
                //Entry copyEntry = dropbox.CopyAsync("Spring Social/File.txt", "Spring Social/File_copy.txt").Result;
                //Entry deleteEntry = dropbox.DeleteAsync("Spring Social/File.txt").Result;
                //Entry moveEntry = dropbox.MoveAsync("Spring Social/File_copy.txt", "Spring Social/File.txt").Result;
                //dropbox.DownloadFileAsync("Spring Social/File.txt")
                //    .ContinueWith(task =>
                //    {
                //        Console.WriteLine("File '{0}' downloaded ({1})", task.Result.Metadata.Path, task.Result.Metadata.Size);
                //        // Save file to "C:\Spring Social.txt"
                //        using (FileStream fileStream = new FileStream(@"C:\Spring Social.txt", FileMode.Create))
                //        {
                //            fileStream.Write(task.Result.Content, 0, task.Result.Content.Length);
                //        }
                //    });
                //Entry folderMetadata = dropbox.GetMetadataAsync("Spring Social").Result;
                //IList<Entry> revisionsEntries = dropbox.GetRevisionsAsync("Spring Social/File.txt").Result;
                //Entry restoreEntry = dropbox.RestoreAsync("Spring Social/File.txt", revisionsEntries[2].Revision).Result;
                //IList<Entry> searchResults = dropbox.SearchAsync("Spring Social/", ".txt").Result;
                //DropboxLink shareableLink = dropbox.GetShareableLinkAsync("Spring Social/File.txt").Result;
                //DropboxLink mediaLink = dropbox.GetMediaLinkAsync("Spring Social/File.txt").Result;
                //Entry uploadImageEntry = dropbox.UploadFileAsync(
                //    new AssemblyResource("assembly://Spring.ConsoleQuickStart/Spring.ConsoleQuickStart/Image.png"),
                //    "/Spring Social/Image.png", true, null, CancellationToken.None).Result;
                //dropbox.DownloadThumbnailAsync("Spring Social/Image.png", ThumbnailFormat.Png, ThumbnailSize.Medium)
                //    .ContinueWith(task =>
                //    {
                //        Console.WriteLine("Thumbnail '{0}' downloaded ({1})", task.Result.Metadata.Path, task.Result.Metadata.Size);
                //        // Save file to "C:\Thumbnail_Medium.png"
                //        using (FileStream fileStream = new FileStream(@"C:\Thumbnail_Medium.png", FileMode.Create))
                //        {
                //            fileStream.Write(task.Result.Content, 0, task.Result.Content.Length);
                //        }
                //    });
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    if (ex is DropboxApiException)
                    {
                        Console.WriteLine(ex.Message);
                        return true;
                    }
                    return false;
                });
            }
        }

        public void UnlinkApp()
        {
        }

        public void InitializeAppFolder()
        {
        }
    
        public void PushStuff()
        {
        }

        public string PullStuff()
        {
            return string.Empty;
        }

        public void DeleteStuff()
        {
        }

        public string PushNowPlaying(AudioFile audioFile, long positionBytes, string position)
        {
            return string.Empty;
        }

        public string PullNowPlaying()
        {
            return string.Empty;
        }

        public void DeleteNowPlaying()
        {
        }
    }
}
#endif

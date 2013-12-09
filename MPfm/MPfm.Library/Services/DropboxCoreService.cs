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

using System.Collections.Generic;
using System.Text;
using MPfm.Core;
using MPfm.Core.Helpers;
using MPfm.Library.Objects;
using MPfm.Library.Services.Exceptions;
using Newtonsoft.Json;
#if !IOS && !ANDROID && !WINDOWS_PHONE
using System;
using System.Diagnostics;
using System.IO;
using Spring.IO;
using MPfm.Library.Services.Interfaces;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;
using Spring.Social.OAuth1;

namespace MPfm.Library.Services
{
    public class DropboxCoreService : ICloudService
    {
        //private const string DropboxAppKey = "6tc6565743i743n";
        //private const string DropboxAppSecret = "fbkt3neevjjl0l2";
        public const string DropboxAppKey = "m1bcpax276elhfi";
        public const string DropboxAppSecret = "2azbuj2eelkranm";

        private IDropbox _dropbox;
        private DropboxServiceProvider _dropboxServiceProvider;
        private OAuthToken _oauthToken;

        public event CloudFileDownloaded OnCloudFileDownloaded;
        public event CloudPathChanged OnCloudPathChanged;
        public bool HasLinkedAccount { get; private set; }

        public event CloudAuthenticationFailed OnCloudAuthenticationFailed;        
        public event CloudAuthenticationStatusChanged OnCloudAuthenticationStatusChanged;

        public DropboxCoreService()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                Console.WriteLine("DropboxCoreService - Initialize - Initializing service...");
                var diskToken = LoadTokenFromDisk();
                var oauthAccessToken = new OAuthToken(diskToken.Value, diskToken.Secret);

                _dropboxServiceProvider = new DropboxServiceProvider(DropboxAppKey, DropboxAppSecret, AccessLevel.AppFolder);
                _dropbox = _dropboxServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                //dropbox.Locale = CultureInfo.CurrentUICulture.IetfLanguageTag;
                HasLinkedAccount = true;
                Console.WriteLine("DropboxCoreService - Initialize - Finished initializing service!");
            }
            catch (AggregateException ae)
            {
                HasLinkedAccount = false;
                ae.Handle(ex =>
                {
                    if (ex is DropboxApiException)
                    {
                        Console.WriteLine("DropboxCoreService - Initialize - Exception: {0}", ex);
                        return true;
                    }
                    
                    // Ignore exceptions; if we cannot login on initialize, the user will have to relink the app later. 
                    // The UI should check the HasLinkedAccount property to see if Dropbox is available.
                    return true;
                });
            }
            catch (Exception ex)
            {
                // Ignore exceptions (see previous comment)
            }
        }        

        public void LinkApp(object view)
        {
            try
            {
                if (OnCloudAuthenticationStatusChanged != null)
                    OnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType.GetRequestToken);

                // If the Initialize method has failed the service provider will be null!
                if(_dropboxServiceProvider == null)
                    _dropboxServiceProvider = new DropboxServiceProvider(DropboxAppKey, DropboxAppSecret, AccessLevel.AppFolder);

                // Authorization without callback url                
                _oauthToken = _dropboxServiceProvider.OAuthOperations.FetchRequestTokenAsync(null, null).Result;

                if (OnCloudAuthenticationStatusChanged != null)
                    OnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType.OpenWebBrowser);

                AuthenticationToken token = null;
                try
                {
                    // Try to get a previously saved token
                    token = LoadTokenFromDisk();
                }
                catch
                {
                    // Ignore exception; use the web browser to get a new token
                }

                if (token == null)
                {
                    // Open web browser for authentication
                    OAuth1Parameters parameters = new OAuth1Parameters();
                    //parameters.Add("locale", CultureInfo.CurrentUICulture.IetfLanguageTag); // for a localized version of the authorization website
                    string authenticateUrl = _dropboxServiceProvider.OAuthOperations.BuildAuthorizeUrl(_oauthToken.Value, parameters);
                    Process.Start(authenticateUrl);                    
                }
                else
                {
                    ContinueLinkApp(token);
                }
            }
            catch (AggregateException ae)
            {
                HasLinkedAccount = false;
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
            ContinueLinkApp(null);
        }

        private void ContinueLinkApp(AuthenticationToken token)
        {
            try
            {
                // Get access token either from parameter or from API
                OAuthToken oauthAccessToken = null;
                if (token == null)
                {
                    AuthorizedRequestToken requestToken = new AuthorizedRequestToken(_oauthToken, null);
                    oauthAccessToken = _dropboxServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
                }
                else
                {
                    oauthAccessToken = new OAuthToken(token.Value, token.Secret);
                }

                if (OnCloudAuthenticationStatusChanged != null)
                    OnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType.RequestAccessToken);

                // Get Dropbox API instance
                _dropbox = _dropboxServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                //dropbox.Locale = CultureInfo.CurrentUICulture.IetfLanguageTag;

                // Test Dropbox API connection (get user name from profile)
                DropboxProfile profile = _dropbox.GetUserProfileAsync().Result;
                HasLinkedAccount = true;

                // Save token to hard disk
                SaveTokenToDisk(new AuthenticationToken() {
                    Value = oauthAccessToken.Value, 
                    Secret = oauthAccessToken.Secret,
                    UserName = profile.DisplayName
                });
                
                if (OnCloudAuthenticationStatusChanged != null)
                    OnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType.ConnectedToDropbox);
            }
            catch (AggregateException ae)
            {
                HasLinkedAccount = false;
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

        private AuthenticationToken LoadTokenFromDisk()
        {
            FileStream fileStream = null;
            try
            {
                byte[] bytes = new byte[4096]; // 4k is way enough to store the token
                string filePath = Path.Combine(PathHelper.HomeDirectory, "dropbox.json");
                fileStream = File.OpenRead(filePath);
                fileStream.Read(bytes, 0, (int) fileStream.Length);
                string json = Encoding.UTF8.GetString(bytes);
                var token = JsonConvert.DeserializeObject<AuthenticationToken>(json);

                return token;
            }
            catch (Exception ex)
            {
                Tracing.Log("DropboxCoreService - LoadTokenFromDisk - Failed to load token: {0}", ex);
                throw;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

            return null;
        }

        private void SaveTokenToDisk(AuthenticationToken token)
        {
            FileStream fileStream = null;
            try
            {
                // If the file exists, decrypt it before changing its contents
                string filePath = Path.Combine(PathHelper.HomeDirectory, "dropbox.json");
                if (File.Exists(filePath))
                    File.Decrypt(filePath);

                // Write file to disk
                string json = JsonConvert.SerializeObject(token);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                fileStream = File.OpenWrite(filePath);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();

                // Encrypt file
                File.Encrypt(filePath);
            }
            catch (Exception ex)
            {
                Tracing.Log("DropboxCoreService - SaveTokenToDisk - Failed to save token: {0}", ex);
                throw;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        private void DeleteTokenFromDisk()
        {
            try
            {
                string filePath = Path.Combine(PathHelper.HomeDirectory, "dropbox.json");
                if(File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Tracing.Log("DropboxCoreService - DeleteTokenFromDisk - Failed to delete token: {0}", ex);
                throw;
            }
        }

        public void UnlinkApp()
        {
            DeleteTokenFromDisk();
            HasLinkedAccount = false;
            _dropbox = null;
        }

        public void CreateFolder(string path)
        {
            ThrowExceptionIfAppIsNotLinked();

            var entry = _dropbox.CreateFolderAsync(path).Result;

            //try
            //{
            //    var entry = _dropbox.CreateFolderAsync(path).Result;
            //}
            //catch (AggregateException ae)
            //{
            //    ae.Handle(ex =>
            //    {
            //        if (ex is DropboxApiException)
            //        {
            //            Console.WriteLine(ex.Message);
            //            return true;
            //        }
            //        return false;
            //    });
            //}
        }

        public bool FileExists(string path)
        {
            ThrowExceptionIfAppIsNotLinked();

            var metadata = _dropbox.GetMetadataAsync(path);
            throw new NotImplementedException();
            return false;
        }

        public List<string> ListFiles(string path, string extension)
        {
            ThrowExceptionIfAppIsNotLinked();

            var strings = new List<string>();
            var entries = _dropbox.SearchAsync(path, extension).Result;
            foreach (var entry in entries)
                strings.Add(entry.Path);

            return strings;
        }

        public void WatchFolder(string path)
        {
        }

        public void WatchFile(string path)
        {
        }

        public void StopWatchFile(string path)
        {
        }

        public void StopWatchFolder(string path)
        {
        }

        public void CloseAllFiles()
        {
        }

        public void DownloadFile(string path)
        {
            ThrowExceptionIfAppIsNotLinked();

            var file = _dropbox.DownloadFileAsync(path).Result;

            if (OnCloudFileDownloaded != null)
                OnCloudFileDownloaded(path, file.Content);
        }

        public void UploadFile(string path, byte[] data)
        {
            ThrowExceptionIfAppIsNotLinked();

            _dropbox.UploadFileAsync(new ByteArrayResource(data), path);
        }

        private void ThrowExceptionIfAppIsNotLinked()
        {
            if (!HasLinkedAccount)
                throw new CloudAppNotLinkedException();
        }
    }
}

#endif

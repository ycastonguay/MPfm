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

#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE

using System;
using System.Collections.Generic;
using MPfm.Library.Database;
using MPfm.Library.UpdateLibrary;
using MPfm.Sound.AudioFiles;

namespace MPfm.Library
{
    /// <summary>
    /// Interface for the Library class.
    /// </summary>
    interface ILibrary
    {        
        event Library.UpdateLibraryFinished OnUpdateLibraryFinished;
        event Library.UpdateLibraryProgress OnUpdateLibraryProgress;
		
        void AddAudioFilesToLibrary(List<string> filePaths);
        void RefreshCache();
        void RemoveAudioFilesWithBrokenFilePaths();
        void RemoveSongsFromLibrary(string folderPath);
        
        List<string> SearchMediaFilesInFolders();
        List<string> SearchMediaFilesInFolders(string folderPath, bool recursive);
        
		Dictionary<string, List<string>> SelectAlbumTitles();
        Dictionary<string, List<string>> SelectAlbumTitles(AudioFileFormat audioFileFormat);
        
		List<string> SelectArtistAlbumTitles(string artistName);
        List<string> SelectArtistAlbumTitles(string artistName, AudioFileFormat audioFileFormat);
        
		List<string> SelectArtistNames();
        List<string> SelectArtistNames(AudioFileFormat audioFileFormat);
        
		AudioFile SelectAudioFile(Guid audioFileId);
        List<AudioFile> SelectAudioFiles();
        List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat);
        List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending);
        List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName);
        List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName, string albumTitle);
        List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName, string albumTitle, string searchTerms);
		
        void UpdateAudioFilePlayCount(Guid audioFileId);
		
		void ResetLibrary();
        void UpdateLibrary();
        void UpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath);
        void UpdateLibraryReportProgress(string title, string message);
        void UpdateLibraryReportProgress(string title, string message, double percentage);
        void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition);
        void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry);
        void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath);
        void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath, UpdateLibraryProgressDataSong song);
        void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath, UpdateLibraryProgressDataSong song, Exception ex);
        void UpdateLibraryReportProgress(string title, string message, double percentage, string logEntry);
    }
}
#endif

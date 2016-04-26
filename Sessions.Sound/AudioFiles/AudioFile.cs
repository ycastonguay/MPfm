// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.ComponentModel;
using System.IO;
using Sessions.Sound.Tags;
using Sessions.Core.Attributes;
using Sessions.Sound.CueFiles;

#if !ANDROID && !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
using CoreGraphics;
#endif

#if (MACOSX || LINUX)
using Mono.Unix;
using Mono.Unix.Native;
#endif

namespace Sessions.Sound.AudioFiles
{
	/// <summary>
	/// The AudioFile class contains the metadata of an audio file.
	/// It can refresh the metadata by reading the tags inside the audio file or from
	/// the database (using Sessions.Library).
	/// </summary>
	public class AudioFile
	{
		/// <summary>
		/// Unique identifier for reading and writing audio file metadata to the database.
		/// </summary>
        [DatabaseField(true, "AudioFileId"), Browsable(false)]
	 	public Guid Id { get; set; }

        /// <summary>
        /// Defines the SV7 tag associated with this audio file. 
        /// Supported file formats: MPC (MusePack).
        /// For more information, go to http://trac.musepack.net/trac/wiki/SV7Specification.
        /// </summary>
        [DatabaseField(false), Category("Tag Sources"), Browsable(true), ReadOnly(true), Description("SV7 Tag. Supported file formats: MPC (MusePack). For more information, go to http://trac.musepack.net/trac/wiki/SV7Specification")] 
        public SV7Tag SV7Tag { get; set; }

        /// <summary>
        /// Defines the SV8 tag associated with this audio file. 
        /// Supported file formats: MPC (MusePack).
        /// For more information, go to http://trac.musepack.net/trac/wiki/SV8Specification.
        /// </summary>
        [DatabaseField(false), Category("Tag Sources"), Browsable(true), ReadOnly(true), Description("SV8 Tag. Supported file formats: MPC (MusePack). For more information, go to http://trac.musepack.net/trac/wiki/SV8Specification")]
        public SV8Tag SV8Tag { get; set; }

        /// <summary>
        /// Defines the APEv1/APEv2 tag associated with this audio file.
        /// Supported file formats: FLAC, APE, WV, MPC, OFR, TTA.
        /// For more information go to http://wiki.hydrogenaudio.org/index.php?title=APEv2_specification.
        /// </summary>
        [DatabaseField(false), Category("Tag Sources"), Browsable(true), ReadOnly(true), Description("APEv1/APEv2 Tag. Supported file formats: FLAC, APE, WV, MPC, OFR, TTA. For more information go to http://wiki.hydrogenaudio.org/index.php?title=APEv2_specification.")]
        public APETag APETag { get; set; }

		#region File Information Properties
		
		/// <summary>
		/// Full path to the audio file.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Full path to the audio file.")]
		public string FilePath { get; set; }

        /// <summary>
        /// File size (in bytes) of the audio file.
        /// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("File size (in bytes).")]
        public long FileSize { get; set; }

		/// <summary>
		/// Type of audio file (FLAC, MP3, OGG, WAV, WV, MPC, OFR, TTA).
        /// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Type of audio file (FLAC, MP3, OGG, WAV, WV, MPC, OFR, TTA).")]
        public AudioFileFormat FileType { get; set; }

		/// <summary>
		/// Position of the first block of data. Useful for reading the Xing header.
		/// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Position of the first block of data.")]
		public long FirstBlockPosition { get; set; }

		/// <summary>
		/// Position of the last block of data.
		/// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Position of the last block of data.")]
		public long LastBlockPosition { get; set; }

		#endregion

		#region Audio Properties
	   
		/// <summary>
		/// Audio bitrate. Indicates the average bitrate for VBR MP3 files.
		/// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio bitrate.")]
		public int Bitrate { get; set; }

		/// <summary>
		/// Audio bits per sample. Usually 16-bit or 24-bit.
		/// </summary>		
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio bits per sample. Usually 16-bit or 24-bit.")]
		public int BitsPerSample { get; set; }

		/// <summary>
		/// Channel mode (only for MP3 files).
		/// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Channel mode (only for MP3 files).")]
		public TagLib.Mpeg.ChannelMode ChannelMode { get; set; }

		/// <summary>
		/// Sample rate (in Hz).
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Sample rate (in Hz).")]
		public int SampleRate { get; set; }

		/// <summary>
		/// Number of audio channels.
		/// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Number of audio channels.")]
		public int AudioChannels { get; set; }

		/// <summary>
		/// Frame length.
		/// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Frame length.")]
		public int FrameLength { get; set; }

		/// <summary>
		/// Audio layer type.
		/// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio layer type.")]
		public int AudioLayer { get; set; }

		/// <summary>
		/// Audio file length (in 00:00.000 format).
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio file length (in 00:00.000 format).")]
		public string Length { get; set; }

        /// <summary>
        /// Audio file length (in bytes).
        /// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio file length (in bytes).")]
        public long LengthBytes { get; set; }

        #endregion

        #region MP3 Properties

        /// <summary>
        /// Indicates the type of header for the MP3 file.
        /// The XING header is found on MP3 files encoded using LAME and VBR/ABR settings.
        /// The INFO header is found on MP3 files encoded using LAME and CBR settings.
        /// Both headers are in fact the same.
        /// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("MP3 header type (XING or INFO).")]
        public string MP3HeaderType { get; set; }

        /// <summary>
        /// MP3 Encoder version.
        /// Ex: LAME3.98
        /// </summary>        
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("MP3 encoder version.")]
        public string MP3EncoderVersion { get; set; }

        /// <summary>
        /// MP3 Encoder delay.
        /// Ex: 576
        /// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("MP3 encoder delay.")]
        public int? MP3EncoderDelay { get; set; }

        /// <summary>
        /// MP3 Encoder padding.
        /// Ex: 1800
        /// </summary>
        [DatabaseField(false), Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("MP3 encoder padding.")]
        public int? MP3EncoderPadding { get; set; }

        ///// <summary>
        ///// Private value for the XingInfoHeader property.
        ///// </summary>
        //private XingInfoHeaderData xingInfoHeader = null;

        ///// <summary>
        ///// Xing/Info header information (only for MP3 files). Null if the Xing header
        ///// information doesn't exist. The Xing header appears on VBR and ABR files, and
        ///// the Info header appears on the CBR files. However, they're actually the same 
        ///// type of header. This data structure contains useful information for gapless playback.
        ///// </summary>
        //public XingInfoHeaderData XingInfoHeader
        //{
        //    get
        //    {
        //        return xingInfoHeader;
        //    }
        //}

        #endregion		

        #region CUE Properties

        [Category("CUE Properties"), Description("Determines the CUE file path related to this audio file.")]
        public string CueFilePath { get; set; }

        [Category("CUE Properties"), Description("Determines the start position within the audio file (requires a CUE file).")]
        public string StartPosition { get; set; }

        [Category("CUE Properties"), Description("Determines the end position within the audio file (requires a CUE file).")]
        public string EndPosition { get; set; }

        #endregion

		#region Database Properties
        
        /// <summary>
        /// Defines the number of times the audio file has been played with Sessions (information comes from the Sessions database).
        /// </summary>
        [Browsable(false)]
        public int PlayCount { get; set; }

        /// <summary>
        /// Defines the last time the audio file has been played with Sessions (information comes from the Sessions database).
        /// Null if the audio file has never been played.
        /// </summary>
        [Browsable(false)]
        public DateTime? LastPlayed { get; set; }

        #endregion

		#region Master Tag Properties

		/// <summary>
		/// Song title.
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Song title.")]
		public string Title { get; set; }

		/// <summary>
		/// Artist name.
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Artist name.")]
		public string ArtistName { get; set; }

		/// <summary>
		/// Album title.
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Album title.")]
		public string AlbumTitle { get; set; }

		/// <summary>
		/// Genre.
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Genre.")]
		public string Genre { get; set; }

		/// <summary>
		/// Disc number.
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Disc number.")]
		public uint DiscNumber { get; set; }

		/// <summary>
		/// Track number.
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Track number.")]
		public uint TrackNumber { get; set; }

        /// <summary>
        /// Returns the disc and track number in string format.
        /// If the disc number is zero, it will return the track number (ex: 10).
        /// If the disc number is higher than zero, it will return the disc number
        /// and the track number separated by a comma (ex: 1.10).
        /// </summary>    
        [Browsable(false)]
        public string DiscTrackNumber
        {
            get
            {
                // Is there a disc number?
                if (DiscNumber == 0)
                {
                    // Return track number
                    return TrackNumber.ToString();
                }

                // Return disc number and track number
                return DiscNumber.ToString() + "." + TrackNumber.ToString();
            }
        }

		/// <summary>
		/// Track number.
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Track count.")]
		public uint TrackCount { get; set; }

		/// <summary>
		/// Production year (year only).
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Production year (year only).")]
		public uint Year { get; set; }

		/// <summary>
		/// Song lyrics.
		/// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Song lyrics.")]
		public string Lyrics { get; set; }

        /// <summary>
        /// Defines the rating of the audio file, from 1 to 5. 
        /// 0 means no rating.
        /// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Defines the rating of the audio file, from 1 to 5 (0 = no rating found).")]
        public int Rating { get; set; }

        /// <summary>
        /// Defines the audio file tempo, in BPM (beats per minute).
        /// 0 means no tempo found.
        /// </summary>
        [Category("Master Tags"), Browsable(true), ReadOnly(false), Description("Defines the audio file tempo, in BPM (beats per minute).")]
        public int Tempo { get; set; }        

		#endregion

        /// <summary>
        /// Constructor for the AudioFile class. This constructor does not set any value.
        /// The FilePath property should at least be filled manually.
        /// </summary>
        public AudioFile()
        {
            Id = Guid.NewGuid();
        }

		/// <summary>
		/// Default constructor for the AudioFile class. Requires the path to the audio file.
		/// </summary>
		/// <param name="filePath">Full path to the audio file</param>
		public AudioFile(string filePath)
		{
			Initialize(filePath, Guid.NewGuid(), true);
		}

		/// <summary>
		/// Constructor for the AudioFile class. Requires the path to the audio file.
		/// </summary>
		/// <param name="filePath">Full path to the audio file</param>
		/// <param name="id">Unique identifier for database storage (if needed)</param>
		public AudioFile(string filePath, Guid id)
		{
			Initialize(filePath, id, true);
		}

		/// <summary>
		/// Constructor for the AudioFile class. Requires the path to the audio file.
		/// </summary>
		/// <param name="filePath">Full path to the audio file</param>
		/// <param name="id">Unique identifier for database storage (if needed)</param>
		/// <param name="readMetadata">If true, the metadata will be refreshed by 
		/// reading the audio file metadata (ex: ID3 tags)</param>
		public AudioFile(string filePath, Guid id, bool readMetadata)
		{
			Initialize(filePath, id, readMetadata);
		}

		/// <summary>
		/// Sets the initial properties and reads initial metadata.
		/// </summary>
		/// <param name="filePath">Audio file path</param>
		/// <param name="id">Unique identifier for database (if needed)</param>
		/// <param name="readMetadata">If true, the metadata will be refreshed by 
		/// reading the audio file metadata (ex: ID3 tags)</param>
		private void Initialize(string filePath, Guid id, bool readMetadata)
		{
            Console.WriteLine("---> Creating AudioFile; {0} / {1}", filePath, id);
			FilePath = filePath;
			Id = id;

			// Set file type based on file extension
            AudioFileFormat audioFileFormat = AudioFileFormat.Unknown;
            string fileExtension = Path.GetExtension(filePath).ToUpper().Replace(".", "");
            if (fileExtension == "M4A" || fileExtension == "MP4" || fileExtension == "AAC")
            {
                // The format can change even though the file extensions are the same
                FileType = AudioFileFormat.AAC;
            }
            else
            {                
                Enum.TryParse<AudioFileFormat>(fileExtension, out audioFileFormat);
                FileType = audioFileFormat;
            }

			if (readMetadata)
				RefreshMetadata();
		}               

		/// <summary>
		/// Refreshes the metadata of the audio file.
		/// </summary>
		public void RefreshMetadata()
        {
            // If the file is attached to a CUE file, then use the CUE file to refresh the metadata
            if (!string.IsNullOrEmpty(CueFilePath))
            {
                CueFileLoader.RefreshAudioFileMetadataFromCueFile(this);
                return; // are the next steps necessary?
            }

            var fileInfo = new FileInfo(FilePath);
            FileSize = fileInfo.Length;

            switch (FileType)
            {
                case AudioFileFormat.MP3:
                    AudioFileLoader.FromMP3(this);
                    break;
                case AudioFileFormat.FLAC:
                    AudioFileLoader.FromFLAC(this);
                    break;
                case AudioFileFormat.OGG:
                    AudioFileLoader.FromOGG(this);
                    break;
                case AudioFileFormat.APE:
                    AudioFileLoader.FromAPE(this);
                    break;
                case AudioFileFormat.MPC:
                    AudioFileLoader.FromMPC(this);
                    break;
                case AudioFileFormat.OFR:
                    AudioFileLoader.FromOFC(this);
                    break;
                case AudioFileFormat.WV:
                    AudioFileLoader.FromWV(this);
                    break;
                case AudioFileFormat.TTA:
                    AudioFileLoader.FromTTA(this);
                    break;
                case AudioFileFormat.WAV:
                    AudioFileLoader.FromWAV(this);
                    break;
                case AudioFileFormat.WMA:
                    AudioFileLoader.FromWMA(this);
                    break;
                case AudioFileFormat.AAC:
                    AudioFileLoader.FromAAC(this);
                    break;
            }              
            
			// If the song has no name, give filename as the name                
			if (String.IsNullOrEmpty(Title))
				Title = Path.GetFileNameWithoutExtension(FilePath);

			// If the artist has no name, give it "Unknown Artist"
			if (String.IsNullOrEmpty(ArtistName))
				ArtistName = "Unknown Artist";

			// If the song has no album title, give it "Unknown Album"
			if (String.IsNullOrEmpty(AlbumTitle))
				AlbumTitle = "Unknown Album";
		}

        /// <summary>
        /// Fills the AudioFile properties from the TagLib values.
        /// </summary>
        /// <param name="tag">TagLib tag structure</param>
        public void FillProperties(TagLib.Tag tag)
        {
            if (!String.IsNullOrEmpty(tag.FirstArtist))
            {
                ArtistName = tag.FirstArtist;
            }
            else if(tag.AlbumArtists.Length > 0)
            {
                ArtistName = tag.AlbumArtists[0];
            }

            AlbumTitle = tag.Album;
            Title = tag.Title;
            Genre = tag.FirstGenre;
            DiscNumber = tag.Disc;
            TrackNumber = tag.Track;
            TrackCount = tag.TrackCount;
            Lyrics = tag.Lyrics;
            Year = tag.Year;
            Tempo = (int)tag.BeatsPerMinute;
        }

        /// <summary>
        /// Fills the AudioFile properties from the APETag structure.
        /// </summary>
        /// <param name="tag">APETag structure</param>
        public void FillProperties(APETag tag)
        {
            ArtistName = tag.Artist;
            AlbumTitle = tag.Album;
            Title = tag.Title;
            Genre = tag.Genre;
            //DiscNumber = tag
            TrackNumber = (uint)tag.Track;
            //TrackCount = tag.tr
            //Lyrics = tag.ly
            Year = (uint)tag.Year.Year;
        }

        /// <summary>
        /// Saves the metadata associated with this audio file from its properties.
        /// </summary>
        public void SaveMetadata()
        {
            switch (FileType)
            {
                case AudioFileFormat.MP3:
                    AudioFileWriter.ForMP3(this);
                    break;
                case AudioFileFormat.FLAC:
                    AudioFileWriter.ForFLAC(this);
                    break;
                case AudioFileFormat.OGG:
                    AudioFileWriter.ForOGG(this);
                    break;
                case AudioFileFormat.APE:
                    AudioFileWriter.ForAPE(this);
                    break;
                case AudioFileFormat.WV:
                    AudioFileWriter.ForWV(this);
                    break;
                case AudioFileFormat.WMA:
                    AudioFileWriter.ForWMA(this);
                    break;
            }
        }
//
//        /// <summary>
//        /// Extracts album art from an audio file.
//        /// </summary>
//        /// <param name="filePath">Audio file path</param>
//        /// <returns>Image (album art)</returns>
//        public static Image ExtractImageForAudioFile(string filePath)
//        {
//            Image imageCover = null;
//
//            if (!File.Exists(filePath))
//                return null;
//
//            string extension = Path.GetExtension(filePath).ToUpper();
//            if (extension == ".MP3")
//            {
//                try
//                {
//                    using (var file = new TagLib.Mpeg.AudioFile(filePath))
//                    {
//                        // Can we get the image from the ID3 tags?
//                        if (file != null && file.Tag != null && file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
//                        {
//                            // Get image from ID3 tags
//                            var ic = new ImageConverter();
//                            imageCover = (Image)ic.ConvertFrom(file.Tag.Pictures[0].Data.Data);
//                        }
//                    }
//                }
//                catch
//                {
//                    // Failed to recover album art. Do nothing.
//                }
//            }
//
//            // Check if the image was found using TagLib
//            if (imageCover == null)
//            {
//                string folderPath = Path.GetDirectoryName(filePath);
//				
//#if (!MACOSX && !LINUX)
//
//                var rootDirectoryInfo = new DirectoryInfo(folderPath);
//
//                // Try to find image files 
//                var imageFiles = new List<FileInfo>();
//                imageFiles.AddRange(rootDirectoryInfo.GetFiles("folder*.JPG").ToList());
//                imageFiles.AddRange(rootDirectoryInfo.GetFiles("folder*.PNG").ToList());
//                imageFiles.AddRange(rootDirectoryInfo.GetFiles("folder*.GIF").ToList());
//                imageFiles.AddRange(rootDirectoryInfo.GetFiles("cover*.JPG").ToList());
//                imageFiles.AddRange(rootDirectoryInfo.GetFiles("cover*.PNG").ToList());
//                imageFiles.AddRange(rootDirectoryInfo.GetFiles("cover*.GIF").ToList());
//
//                // Check if at least one image was found
//                if (imageFiles.Count > 0)
//                {
//                    try
//                    {
//                        imageCover = Image.FromFile(imageFiles[0].FullName);
//                    }
//                    catch
//                    {
//                        Tracing.Log("Error extracting image from " + imageFiles[0].FullName);
//                    }
//                }	
//			
//#endif
//				
//			}
//				
//            return imageCover;
//        }
                
        /// <summary>
        /// Extracts the album art from the audio file. Returns a byte array containing the image.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        /// <returns>Byte array (image)</returns>
        public static byte[] ExtractImageByteArrayForAudioFile(string filePath)
        {
            byte[] bytes = new byte[0];

            if (!File.Exists(filePath))
                return null;

            string extension = Path.GetExtension(filePath).ToUpper();
            if (extension == ".MP3")
            {
                try
                {
                    using (var file = new TagLib.Mpeg.AudioFile(filePath))
                    {
                        if (file != null && file.Tag != null && file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
                        {
                            bytes = file.Tag.Pictures[0].Data.Data;
                        }
                    }
                }
                catch
                {
                    // Failed to recover album art. Do nothing.
                }
            }

            // If we can't get the album art from the tags, try to use FOLDER.JPG instead
            if(bytes.Length == 0 || bytes == null)
            {
                try
                {
                    string albumArtFilePath = Path.Combine(Path.GetDirectoryName(filePath), "folder.jpg");
                    //Console.WriteLine("AudioFile - ExtractImageByteArrayForAudioFile - Trying to extract album art from FOLDER.JPG - Path: {0}", albumArtFilePath);
                    if(File.Exists(albumArtFilePath))
                    {
                        //Console.WriteLine("AudioFile - ExtractImageByteArrayForAudioFile - File exists");
                        bytes = File.ReadAllBytes(albumArtFilePath);
                        //Console.WriteLine("AudioFile - ExtractImageByteArrayForAudioFile - Read bytes length: {0}", bytes.Length);
                    }
                }
                catch
                {
                    // Failed to recover album art. Do nothing.
                }
            }
            
            return bytes;
        }

        public static void SetAlbumArtForAudioFile(string filePath, byte[] imageData)
        {
            if (!File.Exists(filePath))
                return;

            string extension = Path.GetExtension(filePath).ToUpper();
            if (extension == ".MP3")
            {
                using (var file = new TagLib.Mpeg.AudioFile(filePath))
                {
                    if (file != null && file.Tag != null)
                    {
                        file.Tag.Pictures = new TagLib.IPicture[1];
                        file.Tag.Pictures[0].Data = new TagLib.ByteVector(imageData, imageData.Length);
                    }
                }
            }
            else if (extension == ".FLAC")
            {
                // Check if it uses APE
                //var apeTag = APEMetadata.Read(filePath);
                //APEMetadata.Write(filePath, apeTag.Dictionary);

                // Or place it as folder.jpg in the same folder
                string albumArtFilePath = Path.Combine(Path.GetDirectoryName(filePath), "folder.jpg");
                //Console.WriteLine("AudioFile - SetAlbumArtForAudioFile - Writing folder.jpg... - Path: {0}", albumArtFilePath);
                File.WriteAllBytes(albumArtFilePath, imageData);
            }
        }
	}
}


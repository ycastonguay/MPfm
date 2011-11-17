using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using MPfm.Core;

namespace MPfm.Sound
{
	/// <summary>
	/// The AudioFile class contains the properties and tags
	/// of an audio file type.
	/// </summary>
	public class AudioFile
	{
        /// <summary>
        /// Private value for the Id property.
        /// </summary>
        private Guid m_id = Guid.Empty;

		/// <summary>
		/// Unique identifier for reading and writing audio file metadata to the database.
		/// </summary>
		public Guid Id
        {
            get
            {
                return m_id;
            }
        }

		#region File Information Properties
		
		/// <summary>
		/// Private value for the FilePath property.
		/// </summary>
		private string m_filePath = null;

		/// <summary>
		/// Full path to the audio file.
		/// </summary>
		public string FilePath
		{
			get
			{
				return m_filePath;
			}
		}

		/// <summary>
		/// Private value for the FileType property.
		/// </summary>
		private AudioFileType m_fileType = AudioFileType.Unknown;

		/// <summary>
		/// Type of audio file (FLAC, MP3, etc.)
		/// </summary>
		public AudioFileType FileType
		{
			get
			{
				return m_fileType;
			}
		}

		/// <summary>
		/// Private value for the FirstBlockPosition property.
		/// </summary>
		private long m_firstBlockPosition = 0;

		/// <summary>
		/// Position of the first block of data. Useful for reading the Xing header.
		/// </summary>
		public long FirstBlockPosition
		{
			get
			{
				return m_firstBlockPosition;
			}
		}

		/// <summary>
		/// Private value for the LastBlockPosition property.
		/// </summary>
		private long m_lastBlockPosition = 0;

		/// <summary>
		/// Position of the last block of data.
		/// </summary>
		public long LastBlockPosition
		{
			get
			{
				return m_lastBlockPosition;
			}
		}

		#endregion

		#region Audio Properties

		/// <summary>
		/// Private value for the Bitrate property.
		/// </summary>
		private int m_bitrate = 0;

		/// <summary>
		/// Audio bitrate. Indicates the average bitrate for VBR MP3 files.
		/// </summary>
		public int Bitrate
		{
			get
			{
				return m_bitrate;
			}
		}

		/// <summary>
		/// Private value for the BitsPerSample property.
		/// </summary>
		private int m_bitsPerSample = 0;

		/// <summary>
		/// Audio bits per sample. Usually 16-bit or 24-bit.
		/// </summary>
		public int BitsPerSample
		{
			get
			{
				return m_bitsPerSample;
			}
		}

		/// <summary>
		/// Private value for the ChannelMode property.
		/// </summary>
		private TagLib.Mpeg.ChannelMode m_channelMode;

		/// <summary>
		/// Channel mode (only for MP3 files).
		/// </summary>
		public TagLib.Mpeg.ChannelMode ChannelMode
		{
			get
			{
				return m_channelMode;
			}
		}

		/// <summary>
		/// Private value for the SampleRate property.
		/// </summary>
		private int m_sampleRate = 0;

		/// <summary>
		/// Sample rate (in Hz).
		/// </summary>
		public int SampleRate
		{
			get
			{
				return m_sampleRate;
			}
		}

		/// <summary>
		/// Private value for the AudioChannels property.
		/// </summary>
		private int m_audioChannels = 0;

		/// <summary>
		/// Number of channels.
		/// </summary>
		public int AudioChannels
		{
			get
			{
				return m_audioChannels;
			}
		}

		/// <summary>
		/// Private value for the FrameLength property.
		/// </summary>
		private int m_frameLength = 0;

		/// <summary>
		/// Frame length.
		/// </summary>
		public int FrameLength
		{
			get
			{
				return m_frameLength;
			}
		}

		/// <summary>
		/// Private value for the AudioLayer property.
		/// </summary>
		private int m_audioLayer = 0;

		/// <summary>
		/// Audio layer type.
		/// </summary>
		public int AudioLayer
		{
			get
			{
				return m_audioLayer;
			}
		}

		/// <summary>
		/// Private value for the Duration property.
		/// </summary>
		private TimeSpan m_duration;

		/// <summary>
		/// Duration of the audio file.
		/// </summary>
		public TimeSpan Duration
		{
			get
			{
				return m_duration;
			}
		}

		/// <summary>
		/// Private value for the XingInfoHeader property.
		/// </summary>
		private XingInfoHeaderData m_xingInfoHeader = null;

		/// <summary>
		/// Xing/Info header information (only for MP3 files). Null if the Xing header
		/// information doesn't exist. The Xing header appears on VBR and ABR files, and
		/// the Info header appears on the CBR files. However, they're actually the same 
		/// type of header. This data structure contains useful information for gapless playback.
		/// </summary>
		public XingInfoHeaderData XingInfoHeader
		{
			get
			{
				return m_xingInfoHeader;
			}
		}

		#endregion

        #region Other Properties
        
        /// <summary>
        /// Defines the number of times the audio file has been played.
        /// </summary>
        public int PlayCount { get; set; }

        /// <summary>
        /// Defines the last time the audio file has been played.
        /// Null if the audio file has never been played.
        /// </summary>
        public DateTime? LastPlayed { get; set; }

        /// <summary>
        /// Defines the rating of the audio file, from 1 to 5. 
        /// 0 means no rating.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Defines the audio file tempo. 
        /// 0 means no tempo found.
        /// </summary>
        public int Tempo { get; set; }        

        #endregion

        #region ID3v1/ID3v2 (MP3) and VorbisComment (FLAC, OGG) Properties

        /// <summary>
		/// Song title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Artist name.
		/// </summary>
		public string ArtistName { get; set; }

		/// <summary>
		/// Album title.
		/// </summary>
		public string AlbumTitle { get; set; }

		/// <summary>
		/// Genre.
		/// </summary>
		public string Genre { get; set; }

		/// <summary>
		/// Disc number.
		/// </summary>
		public uint DiscNumber { get; set; }

		/// <summary>
		/// Track number.
		/// </summary>
		public uint TrackNumber { get; set; }

        /// <summary>
        /// Track number.
        /// </summary>
        public uint TrackCount { get; set; }

        /// <summary>
        /// Production year.
        /// </summary>
        public uint Year { get; set; }

        /// <summary>
        /// Song lyrics.
        /// </summary>
        public string Lyrics { get; set; }

		#endregion

		/// <summary>
		/// Default constructor for AudioFile. Requires the path to the audio file.
		/// Will raise an exception if the file doesn't exists.
		/// </summary>
		/// <param name="filePath">Full path to the audio file</param>
		public AudioFile(string filePath)
		{
			Initialize(filePath, Guid.NewGuid(), true);
		}

		/// <summary>
		/// Constructor for the AudioFile class. Requires the path to the audio file.
		/// Will raise an exception if the file doesn't exists.
		/// </summary>
		/// <param name="filePath">Full path to the audio file</param>
		/// <param name="id">Unique identifier for database storage (if needed)</param>
		public AudioFile(string filePath, Guid id)
		{
			Initialize(filePath, id, true);
		}

		/// <summary>
		/// Constructor for the AudioFile class. Requires the path to the audio file.
		/// Will raise an exception if the file doesn't exists.
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
			// Set file path
			m_filePath = filePath;

			// Check if the file exists
			if (!File.Exists(filePath))
			{
				throw new Exception("The file at " + filePath + " doesn't exists!");
			}

			// Set file type based on file extension
			string fileExtension = Path.GetExtension(filePath).ToUpper();
			if (fileExtension == ".MP3")
			{
				m_fileType = AudioFileType.MP3;
			}
			else if (fileExtension == ".FLAC")
			{
				m_fileType = AudioFileType.FLAC;
			}
			else if (fileExtension == ".OGG")
			{
				m_fileType = AudioFileType.OGG;
			}
			else if (fileExtension == ".WAV")
			{
				m_fileType = AudioFileType.WAV;
			}

			// Check if the metadata needs to be fetched
			if (readMetadata)
			{
				// Read tags using TagLib# and binary reader
				RefreshMetadata();
			}
		}

		/// <summary>
		/// Refreshes the metadata of the audio file.
		/// </summary>
		public void RefreshMetadata()
		{
			// Check what is the type of the audio file
			if (m_fileType == AudioFileType.MP3)
			{
				try
				{
					// Create a more specific type of class for MP3 files
					TagLib.Mpeg.AudioFile fileMP3 = new TagLib.Mpeg.AudioFile(m_filePath);

					// Get the position of the first and last block
					m_firstBlockPosition = fileMP3.InvariantStartPosition;
					m_lastBlockPosition = fileMP3.InvariantEndPosition;

					// Copy tags
					ArtistName = fileMP3.Tag.FirstArtist;
					AlbumTitle = fileMP3.Tag.Album;
					Title = fileMP3.Tag.Title;
					Genre = fileMP3.Tag.FirstGenre;
					DiscNumber = fileMP3.Tag.Disc;
					TrackNumber = fileMP3.Tag.Track;
                    TrackCount = fileMP3.Tag.TrackCount;
                    Lyrics = fileMP3.Tag.Lyrics;
                    Year = fileMP3.Tag.Year;

					// Loop through codecs (usually just one)
					foreach (TagLib.ICodec codec in fileMP3.Properties.Codecs)
					{
						// Convert codec into a header 
						TagLib.Mpeg.AudioHeader header = (TagLib.Mpeg.AudioHeader)codec;

						// Copy properties						
						m_audioChannels = header.AudioChannels;
						m_frameLength = header.AudioFrameLength;
						m_audioLayer = header.AudioLayer;
						m_sampleRate = header.AudioSampleRate;
						m_bitsPerSample = 16; // always 16-bit
						m_channelMode = header.ChannelMode;
						m_duration = header.Duration;
						m_bitrate = header.AudioBitrate;
					}

					// Close TagLib file
					fileMP3.Dispose();

					// Check if there's a Xing header
					XingInfoHeaderData xingHeader = XingInfoHeaderReader.ReadXingInfoHeader(m_filePath, m_firstBlockPosition);

					// Check if the read was successful
					if (xingHeader.Status == XingInfoHeaderStatus.Successful)
					{
						// Set property value
						m_xingInfoHeader = xingHeader;
					}
				}
				catch (Exception ex)
				{
					throw new Exception("An error occured while reading the tags and properties of the file (" + m_filePath + ").", ex);
				}
			}
			else if(m_fileType == AudioFileType.FLAC)
			{
				// Read VorbisComment in FLAC file
				TagLib.Flac.File fileFlac = new TagLib.Flac.File(m_filePath);

				// Get the position of the first and last block
				m_firstBlockPosition = fileFlac.InvariantStartPosition;
				m_lastBlockPosition = fileFlac.InvariantEndPosition;

				// Copy tags
				ArtistName = fileFlac.Tag.FirstArtist;
				AlbumTitle = fileFlac.Tag.Album;
				Title = fileFlac.Tag.Title;
				Genre = fileFlac.Tag.FirstGenre;
				DiscNumber = fileFlac.Tag.Disc;
				TrackNumber = fileFlac.Tag.Track;
                TrackCount = fileFlac.Tag.TrackCount;
                Lyrics = fileFlac.Tag.Lyrics;
                Year = fileFlac.Tag.Year;

				// Loop through codecs (usually just one)
				foreach (TagLib.ICodec codec in fileFlac.Properties.Codecs)
				{
					// Convert codec into a header 
					TagLib.Flac.StreamHeader header = (TagLib.Flac.StreamHeader)codec;

					// Copy properties
					m_bitrate = header.AudioBitrate;
					m_audioChannels = header.AudioChannels;
					m_sampleRate = header.AudioSampleRate;
					m_bitsPerSample = header.BitsPerSample;
					m_duration = header.Duration;
				}
			}
			else if (m_fileType == AudioFileType.OGG)
			{
				// Read VorbisComment in FLAC file
				TagLib.Ogg.File fileOgg = new TagLib.Ogg.File(m_filePath);

				// Get the position of the first and last block
				m_firstBlockPosition = fileOgg.InvariantStartPosition;
				m_lastBlockPosition = fileOgg.InvariantEndPosition;

				// Copy tags
				ArtistName = fileOgg.Tag.FirstArtist;
				AlbumTitle = fileOgg.Tag.Album;
				Title = fileOgg.Tag.Title;
				Genre = fileOgg.Tag.FirstGenre;
				DiscNumber = fileOgg.Tag.Disc;
				TrackNumber = fileOgg.Tag.Track;
                TrackCount = fileOgg.Tag.TrackCount;
                Lyrics = fileOgg.Tag.Lyrics;
                Year = fileOgg.Tag.Year;                

				// Loop through codecs (usually just one)
				foreach (TagLib.ICodec codec in fileOgg.Properties.Codecs)
				{
					// Check what kind of codec is used 
					if (codec is TagLib.Ogg.Codecs.Theora)
					{
						// Do nothing, this is useless for audio.
					}
					else if (codec is TagLib.Ogg.Codecs.Vorbis)
					{
						// Convert codec into a header 
						TagLib.Ogg.Codecs.Vorbis header = (TagLib.Ogg.Codecs.Vorbis)codec;

						// Copy properties
						m_bitrate = header.AudioBitrate;
						m_audioChannels = header.AudioChannels;
						m_sampleRate = header.AudioSampleRate;
						m_bitsPerSample = 16;
						m_duration = header.Duration;
					}
				}
			}
		}

		public static Image ExtractImageForAudioFile(string filePath)
		{
			// Declare variables
			Image imageCover = null;

			// Check if the file exists
			if (!File.Exists(filePath))
			{
				return null;
			}

			// Check the file extension
			string extension = Path.GetExtension(filePath).ToUpper();
			if (extension == ".MP3")
			{
				try
				{
					// Get tags using TagLib
					TagLib.Mpeg.AudioFile file = new TagLib.Mpeg.AudioFile(filePath);

					// Can we get the image from the ID3 tags?
					if (file != null && file.Tag != null && file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
					{
						// Get image from ID3 tags
						ImageConverter ic = new ImageConverter();
						imageCover = (Image)ic.ConvertFrom(file.Tag.Pictures[0].Data.Data);
					}
				}
				catch
				{
					// Do nothing, try to get an image from another method
				}
			}
			else if (extension == ".FLAC")
			{

			}
			else if (extension == ".OGG")
			{

			}

			// Check if the image was found using TagLib
			if (imageCover == null)
			{
				// Check in the same folder for an image representing the album cover
				string folderPath = Path.GetDirectoryName(filePath);

				// Get the directory information
				DirectoryInfo rootDirectoryInfo = new DirectoryInfo(folderPath);

				// Try to find image files 
				List<FileInfo> imageFiles = new List<FileInfo>();
				imageFiles.AddRange(rootDirectoryInfo.GetFiles("folder*.JPG").ToList());
				imageFiles.AddRange(rootDirectoryInfo.GetFiles("folder*.PNG").ToList());
				imageFiles.AddRange(rootDirectoryInfo.GetFiles("folder*.GIF").ToList());
				imageFiles.AddRange(rootDirectoryInfo.GetFiles("cover*.JPG").ToList());
				imageFiles.AddRange(rootDirectoryInfo.GetFiles("cover*.PNG").ToList());
				imageFiles.AddRange(rootDirectoryInfo.GetFiles("cover*.GIF").ToList());

				// Check if at least one image was found
				if (imageFiles.Count > 0)
				{
					try
					{
						// Get image from file
						imageCover = Image.FromFile(imageFiles[0].FullName);
					}
					catch (Exception ex)
					{
						Tracing.Log("Error extracting image from " + imageFiles[0].FullName);
					}
				}
			}

			return imageCover;
		}
	}

	/// <summary>
	/// Defines the types of audio files supported by MPfm.
	/// </summary>
	public enum AudioFileType
	{
		FLAC = 0, WAV = 1, MP3 = 2, OGG = 3, Unknown = 4
	}
}


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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using MPfm.Core;
using MPfm.Core.Attributes;
using MPfm.Sound.Tags;

#if !ANDROID 
using System.Drawing;
#endif

#if (MACOSX || LINUX)
using Mono.Unix;
using Mono.Unix.Native;
#endif

namespace MPfm.Sound.AudioFiles
{
	/// <summary>
	/// The AudioFile class contains the metadata of an audio file.
	/// It can refresh the metadata by reading the tags inside the audio file or from
	/// the database (using MPfm.Library).
	/// </summary>
	public class AudioFile
	{
		/// <summary>
		/// Private value for the Id property.
		/// </summary>
		private Guid id = Guid.Empty;

		/// <summary>
		/// Unique identifier for reading and writing audio file metadata to the database.
		/// </summary>
        [Browsable(false), DatabaseField(true, "AudioFileId")]
		public Guid Id
		{
			get
			{
				return id;
			}
            set
            {
                id = value;
            }
		}

        /// <summary>
        /// Private value for the SV7Tag property.
        /// </summary>
        private SV7Tag sv7Tag = null;
        /// <summary>
        /// Defines the SV7 tag associated with this audio file. 
        /// Supported file formats: MPC (MusePack).
        /// For more information, go to http://trac.musepack.net/trac/wiki/SV7Specification.
        /// </summary>
        [Category("Tag Sources"), Browsable(true), ReadOnly(true), Description("SV7 Tag. Supported file formats: MPC (MusePack). For more information, go to http://trac.musepack.net/trac/wiki/SV7Specification")]
        public SV7Tag SV7Tag
        {
            get
            {
                return sv7Tag;
            }
        }

        /// <summary>
        /// Private value for the SV8Tag property.
        /// </summary>
        private SV8Tag sv8Tag = null;
        /// <summary>
        /// Defines the SV8 tag associated with this audio file. 
        /// Supported file formats: MPC (MusePack).
        /// For more information, go to http://trac.musepack.net/trac/wiki/SV8Specification.
        /// </summary>
        [Category("Tag Sources"), Browsable(true), ReadOnly(true), Description("SV8 Tag. Supported file formats: MPC (MusePack). For more information, go to http://trac.musepack.net/trac/wiki/SV8Specification")]
        public SV8Tag SV8Tag
        {
            get
            {
                return sv8Tag;
            }
        }

        /// <summary>
        /// Private value for the APETag property.
        /// </summary>
        private APETag apeTag = null;
        /// <summary>
        /// Defines the APEv1/APEv2 tag associated with this audio file.
        /// Supported file formats: FLAC, APE, WV, MPC, OFR, TTA.
        /// For more information go to http://wiki.hydrogenaudio.org/index.php?title=APEv2_specification.
        /// </summary>
        [Category("Tag Sources"), Browsable(true), ReadOnly(true), Description("APEv1/APEv2 Tag. Supported file formats: FLAC, APE, WV, MPC, OFR, TTA. For more information go to http://wiki.hydrogenaudio.org/index.php?title=APEv2_specification.")]
        public APETag APETag
        {
            get
            {
                return apeTag;
            }
        }

		#region File Information Properties
		
		/// <summary>
		/// Private value for the FilePath property.
		/// </summary>
		private string filePath = null;

		/// <summary>
		/// Full path to the audio file.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Full path to the audio file.")]
		public string FilePath
		{
			get
			{
				return filePath;
			}
            set
            {
                filePath = value;
            }
		}

		/// <summary>
		/// Private value for the FileType property.
		/// </summary>
        private AudioFileFormat fileType = AudioFileFormat.Unknown;

		/// <summary>
		/// Type of audio file (FLAC, MP3, OGG, WAV, WV, MPC, OFR, TTA).
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Type of audio file (FLAC, MP3, OGG, WAV, WV, MPC, OFR, TTA).")]
        public AudioFileFormat FileType
		{
			get
			{
				return fileType;
			}
            set
            {
                fileType = value;
            }
		}

		/// <summary>
		/// Private value for the FirstBlockPosition property.
		/// </summary>
		private long firstBlockPosition = 0;

		/// <summary>
		/// Position of the first block of data. Useful for reading the Xing header.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Position of the first block of data.")]
		public long FirstBlockPosition
		{
			get
			{
				return firstBlockPosition;
			}
		}

		/// <summary>
		/// Private value for the LastBlockPosition property.
		/// </summary>
		private long lastBlockPosition = 0;

		/// <summary>
		/// Position of the last block of data.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Position of the last block of data.")]
		public long LastBlockPosition
		{
			get
			{
				return lastBlockPosition;
			}
		}

		#endregion

		#region Audio Properties
	   
		/// <summary>
		/// Private value for the Bitrate property.
		/// </summary>        
		private int bitrate = 0;

		/// <summary>
		/// Audio bitrate. Indicates the average bitrate for VBR MP3 files.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio bitrate.")]
		public int Bitrate
		{
			get
			{
				return bitrate;
			}
            set
            {
                bitrate = value;
            }
		}

		/// <summary>
		/// Private value for the BitsPerSample property.
		/// </summary>
		private int bitsPerSample = 0;

		/// <summary>
		/// Audio bits per sample. Usually 16-bit or 24-bit.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio bits per sample. Usually 16-bit or 24-bit.")]
		public int BitsPerSample
		{
			get
			{
				return bitsPerSample;
			}
		}

		/// <summary>
		/// Private value for the ChannelMode property.
		/// </summary>
		private TagLib.Mpeg.ChannelMode channelMode;

		/// <summary>
		/// Channel mode (only for MP3 files).
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Channel mode (only for MP3 files).")]
		public TagLib.Mpeg.ChannelMode ChannelMode
		{
			get
			{
				return channelMode;
			}
		}

		/// <summary>
		/// Private value for the SampleRate property.
		/// </summary>
		private int sampleRate = 0;

		/// <summary>
		/// Sample rate (in Hz).
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Sample rate (in Hz).")]
		public int SampleRate
		{
			get
			{
				return sampleRate;
			}
            set
            {
                sampleRate = value;
            }
		}

		/// <summary>
		/// Private value for the AudioChannels property.
		/// </summary>
		private int audioChannels = 0;

		/// <summary>
		/// Number of audio channels.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Number of audio channels.")]
		public int AudioChannels
		{
			get
			{
				return audioChannels;
			}
		}

		/// <summary>
		/// Private value for the FrameLength property.
		/// </summary>
		private int frameLength = 0;

		/// <summary>
		/// Frame length.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Frame length.")]
		public int FrameLength
		{
			get
			{
				return frameLength;
			}
		}

		/// <summary>
		/// Private value for the AudioLayer property.
		/// </summary>
		private int audioLayer = 0;

		/// <summary>
		/// Audio layer type.
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio layer type.")]
		public int AudioLayer
		{
			get
			{
				return audioLayer;
			}
		}

		/// <summary>
		/// Private value for the Length property.
		/// </summary>
		private string length;

		/// <summary>
		/// Audio file length (in 00:00.000 format).
		/// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("Audio file length (in 00:00.000 format).")]
		public string Length
		{
			get
			{
				return length;
			}
			set
			{
				length = value;
			}
		}

        #endregion

        #region MP3 Properties

        /// <summary>
        /// Private value for the MP3HeaderType property.
        /// </summary>
        private string mp3HeaderType = string.Empty;

        /// <summary>
        /// Indicates the type of header for the MP3 file.
        /// The XING header is found on MP3 files encoded using LAME and VBR/ABR settings.
        /// The INFO header is found on MP3 files encoded using LAME and CBR settings.
        /// Both headers are in fact the same.
        /// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("MP3 header type (XING or INFO).")]
        public string MP3HeaderType
        {
            get
            {
                return mp3HeaderType;
            }
        }

        /// <summary>
        /// Private value for the MP3EncoderVersion property.
        /// </summary>
        private string mp3EncoderVersion = string.Empty;

        /// <summary>
        /// MP3 Encoder version.
        /// Ex: LAME3.98
        /// </summary>        
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("MP3 encoder version.")]
        public string MP3EncoderVersion
        {
            get
            {
                return mp3EncoderVersion;
            }
        }

        /// <summary>
        /// Private value for the MP3EncoderDelay property.
        /// </summary>
        private int? mp3EncoderDelay = null;

        /// <summary>
        /// MP3 Encoder delay.
        /// Ex: 576
        /// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("MP3 encoder delay.")]
        public int? MP3EncoderDelay
        {
            get
            {
                return mp3EncoderDelay;
            }
        }

        /// <summary>
        /// Private value for the MP3EncoderDelay property.
        /// </summary>
        private int? mp3EncoderPadding = null;

        /// <summary>
        /// MP3 Encoder padding.
        /// Ex: 1800
        /// </summary>
        [Category("Audio Properties"), Browsable(true), ReadOnly(true), Description("MP3 encoder padding.")]
        public int? MP3EncoderPadding
        {
            get
            {
                return mp3EncoderPadding;
            }
        }

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

		#region Database Properties
        
        /// <summary>
        /// Defines the number of times the audio file has been played with MPfm (information comes from the MPfm database).
        /// </summary>
        [Browsable(false)]
        public int PlayCount { get; set; }

        /// <summary>
        /// Defines the last time the audio file has been played with MPfm (information comes from the MPfm database).
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
        }

		/// <summary>
		/// Default constructor for the AudioFile class. Requires the path to the audio file.
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
			// Set properties
			this.filePath = filePath;
			this.id = id;

			// Set file type based on file extension
            AudioFileFormat audioFileFormat = AudioFileFormat.Unknown;
            string fileExtension = Path.GetExtension(filePath).ToUpper().Replace(".", "");
            if (fileExtension == "M4A" || fileExtension == "MP4" || fileExtension == "AAC")
            {
                // The format can change even though the file extensions are the same
                fileType = AudioFileFormat.AAC;
            }
            else
            {                
                // Get format by file extension
                Enum.TryParse<AudioFileFormat>(fileExtension, out audioFileFormat);
                fileType = audioFileFormat;
            }

			// Check if the metadata needs to be fetched
			if (readMetadata)
			{
                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    throw new Exception("The file at " + filePath + " doesn't exists!");
                }

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
            if (fileType == AudioFileFormat.MP3)
			{
                // Declare variables
                TagLib.Mpeg.AudioFile file = null;

                try
                {
                    // Create a more specific type of class for MP3 files
                    file = new TagLib.Mpeg.AudioFile(filePath);                    

                    // Get the position of the first and last block
                    firstBlockPosition = file.InvariantStartPosition;
                    lastBlockPosition = file.InvariantEndPosition;

                    // Copy tags
                    FillProperties(file.Tag);

                    // Loop through codecs (usually just one)
                    foreach (TagLib.ICodec codec in file.Properties.Codecs)
                    {
                        // Convert codec into a header 
                        TagLib.Mpeg.AudioHeader header = (TagLib.Mpeg.AudioHeader)codec;

                        // Copy properties						
                        audioChannels = header.AudioChannels;
                        frameLength = header.AudioFrameLength;
                        audioLayer = header.AudioLayer;
                        sampleRate = header.AudioSampleRate;
                        bitsPerSample = 16; // always 16-bit
                        channelMode = header.ChannelMode;
                        bitrate = header.AudioBitrate;
                        length = Conversion.TimeSpanToTimeString(header.Duration);
                    }
                }
                catch (Exception ex)
                {
                    // Throw exception TODO: Check if file exists when catching the exception (to make a better error description)
                    throw new Exception("An error occured while reading the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }

                try
                {
//                    // Check if there's a Xing header
//                    XingInfoHeaderData xingHeader = XingInfoHeaderReader.ReadXingInfoHeader(filePath, firstBlockPosition);
//
//                    // Check if the read was successful
//                    if (xingHeader.Status == XingInfoHeaderStatus.Successful)
//                    {
//                        // Set property value
//                        //m_xingInfoHeader = xingHeader;
//                        mp3EncoderDelay = xingHeader.EncoderDelay;
//                        mp3EncoderPadding = xingHeader.EncoderPadding;
//                        mp3EncoderVersion = xingHeader.EncoderVersion;
//                        mp3HeaderType = xingHeader.HeaderType;
//                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
			}
            else if (fileType == AudioFileFormat.FLAC)
			{
                // Declare variables 
                TagLib.Flac.File file = null;

                try
                {
                    // Read VorbisComment in FLAC file
                    file = new TagLib.Flac.File(filePath);

                    // Get the position of the first and last block
                    firstBlockPosition = file.InvariantStartPosition;
                    lastBlockPosition = file.InvariantEndPosition;

                    // Copy tags
                    FillProperties(file.Tag);

                    // Loop through codecs (usually just one)
                    foreach (TagLib.ICodec codec in file.Properties.Codecs)
                    {
                        // Convert codec into a header 
                        TagLib.Flac.StreamHeader header = (TagLib.Flac.StreamHeader)codec;

                        // Copy properties
                        bitrate = header.AudioBitrate;
                        audioChannels = header.AudioChannels;
                        sampleRate = header.AudioSampleRate;
                        bitsPerSample = header.BitsPerSample;
                        length = Conversion.TimeSpanToTimeString(header.Duration);
                    }
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
			}
            else if (fileType == AudioFileFormat.OGG)
			{
                // Declare variables 
                TagLib.Ogg.File file = null;

                try
                {
                    // Read VorbisComment in OGG file
                    file = new TagLib.Ogg.File(filePath);

                    // Get the position of the first and last block
                    firstBlockPosition = file.InvariantStartPosition;
                    lastBlockPosition = file.InvariantEndPosition;

                    // Copy tags
                    FillProperties(file.Tag);

                    // Loop through codecs (usually just one)
                    foreach (TagLib.ICodec codec in file.Properties.Codecs)
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
                            bitrate = header.AudioBitrate;
                            audioChannels = header.AudioChannels;
                            sampleRate = header.AudioSampleRate;
                            bitsPerSample = 16;
                            length = Conversion.TimeSpanToTimeString(header.Duration);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
			}
            else if (fileType == AudioFileFormat.APE)
            {
                // Monkey's Audio (APE) supports APEv2 tags.
                // http://en.wikipedia.org/wiki/Monkey's_Audio

                // Declare variables
                TagLib.Ape.File file = null;

                try
                {
                    // Read APE metadata
                    apeTag = APEMetadata.Read(filePath);

                    // Get APEv1/v2 tags from APE file
                    file = new TagLib.Ape.File(filePath);

                    // Get the position of the first and last block
                    firstBlockPosition = file.InvariantStartPosition;
                    lastBlockPosition = file.InvariantEndPosition;

                    // Copy tags
                    FillProperties(file.Tag);

                    // Loop through codecs (usually just one)
                    foreach (TagLib.ICodec codec in file.Properties.Codecs)
                    {
                        // Check what kind of codec is used 
                        if (codec is TagLib.Ape.StreamHeader)
                        {
                            // Convert codec into a header 
                            TagLib.Ape.StreamHeader header = (TagLib.Ape.StreamHeader)codec;

                            // Copy properties
                            bitrate = header.AudioBitrate;
                            audioChannels = header.AudioChannels;
                            sampleRate = header.AudioSampleRate;
                            bitsPerSample = 16;
                            length = Conversion.TimeSpanToTimeString(header.Duration);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.MPC)
            {                
                // MusePack (MPC) supports APEv2 tags.
                // http://en.wikipedia.org/wiki/Musepack
                try
                {
                    // Try to read SV8 header
                    sv8Tag = SV8Metadata.Read(filePath);

                    // Set audio properties
                    audioChannels = sv8Tag.AudioChannels;
                    sampleRate = sv8Tag.SampleRate;
                    bitsPerSample = 16;
                    length = sv8Tag.Length;
                    bitrate = sv8Tag.Bitrate;
                }
                catch (SV8TagNotFoundException exSV8)
                {
                    try
                    {
                        // Try to read the SV7 header
                        sv7Tag = SV7Metadata.Read(filePath);

                        // Set audio properties
                        audioChannels = sv7Tag.AudioChannels;
                        sampleRate = sv7Tag.SampleRate;
                        bitsPerSample = 16;
                        length = sv7Tag.Length;
                        bitrate = sv7Tag.Bitrate;
                    }
                    catch (SV7TagNotFoundException exSV7)
                    {
                        // No headers have been found!
                        SV8TagNotFoundException finalEx = new SV8TagNotFoundException(exSV8.Message, exSV7);
                        throw new Exception("Error: The file is not in SV7/MPC or SV8/MPC format!",  finalEx);
                    }
                }

                try
                {
                    // Read APE tag
                    apeTag = APEMetadata.Read(filePath);

                    // Copy tags
                    FillProperties(apeTag);
                }
                catch (Exception ex)
                {
                    // Check exception
                    if (ex is APETagNotFoundException)
                    {
                        // Skip file
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else if (fileType == AudioFileFormat.OFR)
            {
                // TagLib does not support OFR files...
                // OptimFROG (OFR) supports APEv2 tags.
                // http://en.wikipedia.org/wiki/OptimFROG                
            }
            else if (fileType == AudioFileFormat.WV)
            {
                // WavPack supports APEv2 and ID3v1 tags.
                // http://www.wavpack.com/wavpack_doc.html

                // Declare variables
                TagLib.WavPack.File file = null;

                try
                {
                    // Read WavPack tags
                    file = new TagLib.WavPack.File(filePath);

                    // Get the position of the first and last block
                    firstBlockPosition = file.InvariantStartPosition;
                    lastBlockPosition = file.InvariantEndPosition;

                    // Copy tags
                    FillProperties(file.Tag);

                    // Loop through codecs (usually just one)
                    foreach (TagLib.ICodec codec in file.Properties.Codecs)
                    {
                        // Check what kind of codec is used 
                        if (codec is TagLib.WavPack.StreamHeader)
                        {
                            // Convert codec into a header 
                            TagLib.WavPack.StreamHeader header = (TagLib.WavPack.StreamHeader)codec;

                            // Copy properties
                            bitrate = header.AudioBitrate;
                            audioChannels = header.AudioChannels;
                            sampleRate = header.AudioSampleRate;
                            bitsPerSample = 16;
                            length = Conversion.TimeSpanToTimeString(header.Duration);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.TTA)
            {
                // The True Audio (TTA) format supports ID3v1, ID3v2 and APEv2 tags.
                // http://en.wikipedia.org/wiki/TTA_(codec)

                audioChannels = 2;
                sampleRate = 44100;
                bitsPerSample = 16;

                // TagLib doesn't work.
            }
            else if (fileType == AudioFileFormat.WAV)
            {
                // Declare variables
                TagLib.Riff.File file = null;

                try
                {
                    // Get WAV file
                    file = new TagLib.Riff.File(filePath);                    

                    // Get the position of the first and last block
                    firstBlockPosition = file.InvariantStartPosition;
                    lastBlockPosition = file.InvariantEndPosition;

                    // Copy tags
                    FillProperties(file.Tag);

                    // Loop through codecs (usually just one)
                    foreach (TagLib.ICodec codec in file.Properties.Codecs)
                    {
                        // Check what kind of codec is used 
                        if (codec is TagLib.Riff.WaveFormatEx)
                        {
                            // Convert codec into a header 
                            TagLib.Riff.WaveFormatEx header = (TagLib.Riff.WaveFormatEx)codec;

                            // Copy properties
                            bitrate = header.AudioBitrate;
                            audioChannels = header.AudioChannels;
                            sampleRate = header.AudioSampleRate;
                            bitsPerSample = 16;
                            length = Conversion.TimeSpanToTimeString(header.Duration);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.WMA)
            {
                // Declare variables
                TagLib.Asf.File file = null;

                try
                {
                    // Read ASF/WMA tags
                    file = new TagLib.Asf.File(filePath);

                    // Get the position of the first and last block
                    firstBlockPosition = file.InvariantStartPosition;
                    lastBlockPosition = file.InvariantEndPosition;

                    // Copy tags
                    FillProperties(file.Tag);

                    // The right length is here, not in the codec data structure
                    length = Conversion.TimeSpanToTimeString(file.Properties.Duration);

                    // Loop through codecs (usually just one)
                    foreach (TagLib.ICodec codec in file.Properties.Codecs)
                    {
                        // Check what kind of codec is used 
                        if (codec is TagLib.Riff.WaveFormatEx)
                        {
                            // Convert codec into a header 
                            TagLib.Riff.WaveFormatEx header = (TagLib.Riff.WaveFormatEx)codec;

                            // Copy properties
                            bitrate = header.AudioBitrate;
                            audioChannels = header.AudioChannels;
                            sampleRate = header.AudioSampleRate;
                            bitsPerSample = 16;                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.AAC)
            {
                // Declare variables
                TagLib.Aac.File file = null;

                try
                {
                    // Read AAC tags
                    file = new TagLib.Aac.File(filePath);

                    // Doesn't seem to work very well...
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }              
            
			// If the song has no name, give filename as the name                
			if (String.IsNullOrEmpty(Title))
			{
				Title = Path.GetFileNameWithoutExtension(filePath);
			}

			// If the artist has no name, give it "Unknown Artist"
			if (String.IsNullOrEmpty(ArtistName))
			{
				ArtistName = "Unknown Artist";
			}

			// If the song has no album title, give it "Unknown Album"
			if (String.IsNullOrEmpty(AlbumTitle))
			{
				AlbumTitle = "Unknown Album";
			}
		}

        /// <summary>
        /// Fills the AudioFile properties from the TagLib values.
        /// </summary>
        /// <param name="tag">TagLib tag structure</param>
        private void FillProperties(TagLib.Tag tag)
        {
            // Artist name
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
        private void FillProperties(APETag tag)
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
        /// Fills the Tag properties of a TagLib.File class (including its subclasses).
        /// </summary>
        /// <param name="file">TagLib.File class</param>
        /// <returns>TagLib.File</returns>
        public TagLib.File FillTags(TagLib.File file)
        {
            // Copy tags
            file.Tag.AlbumArtists = new string[] { ArtistName };
            file.Tag.Album = AlbumTitle;
            file.Tag.Title = Title;
            file.Tag.Genres = new string[] { Genre };
            file.Tag.Disc = DiscNumber;
            file.Tag.Track = TrackNumber;
            file.Tag.TrackCount = TrackCount;
            file.Tag.Lyrics = Lyrics;
            file.Tag.Year = Year;
            file.Tag.BeatsPerMinute = (uint)Tempo;

            return file;
        }

        /// <summary>
        /// Saves the metadata associated with this audio file from its properties.
        /// </summary>
        public void SaveMetadata()
        {
            // Check what is the type of the audio file
            if (fileType == AudioFileFormat.MP3)
            {          
                // Declare variables
                TagLib.Mpeg.AudioFile file = null;

                try
                {
                    // Read tags
                    file = new TagLib.Mpeg.AudioFile(filePath);                    

                    // Copy tags
                    file = (TagLib.Mpeg.AudioFile)FillTags(file);

                    // Save metadata
                    file.Save();
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading/writing the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.FLAC)
            {
                // Declare variables
                TagLib.Flac.File file = null;

                try
                {
                    // Read tags
                    file = new TagLib.Flac.File(filePath);

                    // Copy tags
                    file = (TagLib.Flac.File)FillTags(file);

                    // Save metadata
                    file.Save();
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading/writing the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.OGG)
            {
                // Declare variables
                TagLib.Ogg.File file = null;

                try
                {
                    // Read tags
                    file = new TagLib.Ogg.File(filePath);

                    // Copy tags
                    file = (TagLib.Ogg.File)FillTags(file);

                    // Save metadata
                    file.Save();
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading/writing the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.APE)
            {
                // Declare variables
                TagLib.Ape.File file = null;

                try
                {
                    // Read tags
                    file = new TagLib.Ape.File(filePath);

                    // Copy tags
                    file = (TagLib.Ape.File)FillTags(file);

                    // Save metadata
                    file.Save();
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading/writing the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.WV)
            {
                // Declare variables
                TagLib.WavPack.File file = null;

                try
                {
                    // Read tags
                    file = new TagLib.WavPack.File(filePath);

                    // Copy tags
                    file = (TagLib.WavPack.File)FillTags(file);

                    // Save metadata
                    file.Save();
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading/writing the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
            else if (fileType == AudioFileFormat.WMA)
            {
                // Declare variables
                TagLib.Asf.File file = null;

                try
                {
                    // Read tags
                    file = new TagLib.Asf.File(filePath);

                    // Copy tags
                    file = (TagLib.Asf.File)FillTags(file);

                    // Save metadata
                    file.Save();
                }
                catch (Exception ex)
                {
                    // Throw exception
                    throw new Exception("An error occured while reading/writing the tags and properties of the file (" + filePath + ").", ex);
                }
                finally
                {
                    // Dispose file (if needed)
                    if (file != null)
                        file.Dispose();
                }
            }
        }

#if (!IOS && !ANDROID)

        /// <summary>
        /// Extracts album art from an audio file.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        /// <returns>Image (album art)</returns>
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
                    using (TagLib.Mpeg.AudioFile file = new TagLib.Mpeg.AudioFile(filePath))
                    {
                        // Can we get the image from the ID3 tags?
                        if (file != null && file.Tag != null && file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
                        {
                            // Get image from ID3 tags
                            ImageConverter ic = new ImageConverter();
                            imageCover = (Image)ic.ConvertFrom(file.Tag.Pictures[0].Data.Data);
                        }
                    }
                }
                catch
                {
                    // Failed to recover album art. Do nothing.
                }
            }

            // Check if the image was found using TagLib
            if (imageCover == null)
            {
                // Check in the same folder for an image representing the album cover
                string folderPath = Path.GetDirectoryName(filePath);
				
#if (!MACOSX && !LINUX)

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
                    catch
                    {
                        Tracing.Log("Error extracting image from " + imageFiles[0].FullName);
                    }
                }	
			
#endif
				
			}
				
            return imageCover;
        }

#endif
                
        /// <summary>
        /// Extracts the album art from the audio file. Returns a byte array containing the image.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        /// <returns>Byte array (image)</returns>
        public static byte[] ExtractImageByteArrayForAudioFile(string filePath)
        {
            byte[] bytes = new byte[0];

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
                    using (TagLib.Mpeg.AudioFile file = new TagLib.Mpeg.AudioFile(filePath))
                    {
                        // Can we get the image from the ID3 tags?
                        if (file != null && file.Tag != null && file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
                        {
                            // Get image from ID3 tags
                            bytes = file.Tag.Pictures[0].Data.Data;
                        }
                    }
                }
                catch
                {
                    // Failed to recover album art. Do nothing.
                }
            }
            
            return bytes;
        }
	}
}


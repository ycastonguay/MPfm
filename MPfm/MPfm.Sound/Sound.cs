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
using System.Drawing;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace MPfm.Sound
{
    /// <summary>
    /// This class wraps up the Sound class of the FMOD library.
    /// </summary>
    public class Sound
    {
        #region Base Properties

        private FMOD.Sound m_baseSound;
        /// <summary>
        /// Base FMOD.Sound.
        /// </summary>
        public FMOD.Sound BaseSound
        {
            get
            {
                return m_baseSound;
            }
        }

        private string m_filePath;
        /// <summary>
        /// Audio file path.
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_filePath;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for Sound. Needs a pointer to the base FMOD.Sound 
        /// and the path to the audio file.
        /// </summary>        
        /// <param name="baseSound">Base FMOD.Sound</param>
        /// <param name="filePath">Audio file path</param>
        public Sound(FMOD.Sound baseSound, string filePath)
        {
            // Set private variables            
            this.m_baseSound = baseSound;
            this.m_filePath = filePath;
        }

        #region Dynamic Properties
        
        /// <summary>
        /// Returns the absolute length of the sound, in milliseconds.
        /// </summary>
        public uint LengthAbsoluteMilliseconds
        {
            get
            {
                FMOD.RESULT result;
                uint value = 0;

                try
                {
                    result = BaseSound.getLength(ref value, FMOD.TIMEUNIT.MS);
                    System.CheckForError(result);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return value;
            }
        }

        /// <summary>
        /// Returns the length of the sound, in minutes.
        /// Use with LengthSeconds and LengthMilliseconds to make a time display.
        /// </summary>
        public double LengthMinutes
        {
            get
            {
                return LengthAbsoluteMilliseconds / 1000 / 60;
            }
        }

        /// <summary>
        /// Returns the length of the sound, in seconds.
        /// Use with LengthMinutes and LengthMilliseconds to make a time display.
        /// </summary>
        public double LengthSeconds
        {
            get
            {
                return LengthAbsoluteMilliseconds / 1000 % 60;
            }
        }

        /// <summary>
        /// Returns the length of the sound, in milliseconds.
        /// Use with LengthMinutes and LengthSeconds to make a time display.
        /// </summary>
        public double LengthMilliseconds
        {
            get
            {
                return LengthAbsoluteMilliseconds / 10 % 100;
            }
        }

        /// <summary>
        /// Returns the length of the sound, in string format.
        /// Uses the 00:00.000 format.
        /// </summary>
        public string Length
        {
            get
            {
                return LengthMinutes.ToString("0") + ":" + LengthSeconds.ToString("00") + "." + LengthMilliseconds.ToString("000");
            }
        }

        /// <summary>
        /// Returns the length of the sound, in PCM bytes format.        
        /// </summary>
        public uint LengthPCMBytes
        {
            get
            {
                FMOD.RESULT result;
                uint value = 0;

                result = BaseSound.getLength(ref value, FMOD.TIMEUNIT.PCMBYTES);
                System.CheckForError(result);

                return value;
            }
        }

        /// <summary>
        /// Returns the length of the sound, in PCM format.        
        /// </summary>
        public uint LengthPCM
        {
            get
            {
                FMOD.RESULT result;
                uint value = 0;

                result = BaseSound.getLength(ref value, FMOD.TIMEUNIT.PCM);
                System.CheckForError(result);

                return value;
            }
        }

        /// <summary>
        /// Returns the frequency of the audio file (ex: 44100 Hz).
        /// </summary>
        public double Frequency
        {
            get
            {
                float freq = 0;
                float vol = 0;
                float pan = 0;
                int priority = 0;
                BaseSound.getDefaults(ref freq, ref vol, ref pan, ref priority);

                return Convert.ToDouble(freq);
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Releases the base FMOD.Sound.
        /// </summary>
        public void Release()
        {
            FMOD.RESULT result;

            try
            {
                if (BaseSound != null)
                {
                    result = BaseSound.release();
                    System.CheckForError(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// [FMOD documentation] Retrieves the state a sound is in after FMOD_NONBLOCKING has been used to open it, or the state of the streaming buffer.
        /// </summary>
        public SoundOpenState GetOpenState()
        {
            FMOD.RESULT result;
            SoundOpenState state = new SoundOpenState();

            try
            {
                if (BaseSound != null)
                {
                    result = BaseSound.getOpenState(ref state.openState, ref state.percentBuffered, ref state.starving, ref state.diskBusy);
                    System.CheckForError(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return state;
        }

        /// <summary>
        /// Reads the PCM raw data of the sound object.        
        /// </summary>
        /// <returns>Byte array</returns>
        public byte[] ReadData(uint bufferSize, ref uint read)
        {
            FMOD.RESULT result;

            try
            {
                if (BaseSound != null)
                {
                    //uint length = 0;
                    //BaseSound.getLength(ref length, FMOD.TIMEUNIT.PCMBYTES);

                    //result = BaseSound.seekData(0);
                    //System.CheckForError(result);

                    IntPtr ptr = Marshal.AllocHGlobal((int)bufferSize);
                    //uint read = 0;
                    result = BaseSound.readData(ptr, bufferSize, ref read);

                    //System.CheckForError(result);

                    byte[] buffer = new byte[read];
                    Marshal.Copy(ptr, buffer, 0, (int)read);

                    // Very very important: this method needs to be called to prevent a memory leak
                    Marshal.FreeHGlobal(ptr);

                    return buffer;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// Reads the PCM raw data of the whole sound object.
        /// Warning: this can take a LOT memory for big files.
        /// </summary>
        /// <returns>Byte array</returns>
        public byte[] ReadAllData()
        {
            FMOD.RESULT result;

            try
            {
                if (BaseSound != null)
                {
                    uint length = 0;
                    BaseSound.getLength(ref length, FMOD.TIMEUNIT.PCMBYTES);

                    //result = BaseSound.seekData(0);
                    //System.CheckForError(result);

                    IntPtr ptr = Marshal.AllocHGlobal((int)length);
                    uint read = 0;
                    result = BaseSound.readData(ptr, length, ref read);

                    //System.CheckForError(result);

                    byte[] buffer = new byte[read];
                    Marshal.Copy(ptr, buffer, 0, (int)read);

                    // Very very important: this method needs to be called to prevent a memory leak
                    Marshal.FreeHGlobal(ptr);

                    return buffer;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        #endregion

        #region Tagging Methods

        /// <summary>
        /// Returns the number of tags associated with the sound object.
        /// </summary>
        /// <returns>Number of tags</returns>
        public int GetNumTags()
        {
            FMOD.RESULT result;

            try
            {
                if (BaseSound != null)
                {
                    int numtags = 0;
                    int numtagsupdated = 0;
                    result = BaseSound.getNumTags(ref numtags, ref numtagsupdated);
                    System.CheckForError(result);

                    return numtags;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        /// <summary>
        /// Returns a specific tag by its name.
        /// </summary>
        /// <param name="name">Name of the tag</param>
        /// <returns>Tag object</returns>
        public FMOD.TAG GetFMODTag(string name)
        {
            FMOD.RESULT result;

            try
            {
                if (BaseSound != null)
                {
                    FMOD.TAG tag = new FMOD.TAG();
                    result = BaseSound.getTag(name, 0, ref tag);
                    System.CheckForError(result);

                    return tag;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new FMOD.TAG();
        }

        /// <summary>
        /// Returns a specific tag by its index.
        /// </summary>
        /// <param name="index">Index of the tag</param>
        /// <returns>Tag object</returns>
        public FMOD.TAG GetFMODTag(int index)
        {
            FMOD.RESULT result;

            try
            {
                if (BaseSound != null)
                {
                    FMOD.TAG tag = new FMOD.TAG();
                    result = BaseSound.getTag(null, index, ref tag);
                    System.CheckForError(result);

                    return tag;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new FMOD.TAG();
        }

        /// <summary>
        /// Returns a list of all tags associated with the sound object.
        /// </summary>
        /// <returns>List of FMOD tags</returns>
        public List<FMOD.TAG> GetFMODTags()
        {
            List<FMOD.TAG> listTags = new List<FMOD.TAG>();

            int numberOfTags = GetNumTags();            
            for (int a = 0; a < numberOfTags; a++)
            {
                FMOD.TAG tag = GetFMODTag(a);
                listTags.Add(tag);                
            }

            return listTags;
        }

        /// <summary>
        /// Gets the tags of the song for every sound format. 
        /// Uses TagLib# by default. If TagLib# fails, the method uses FMOD's tagging system.
        /// </summary>
        /// <returns>List of tags</returns>
        public Tags GetTags()
        {
            // Create variables
            Tags tags = new Tags();

            // Try to get tags from TagLib
            try
            {
                // Get tags
                if (FilePath.ToUpper().Contains("MP3"))
                {                   
                    TagLib.Mpeg.AudioFile a = new TagLib.Mpeg.AudioFile(FilePath);

                    foreach(TagLib.ICodec codec in a.Properties.Codecs)
                    {
                        TagLib.Mpeg.AudioHeader header = (TagLib.Mpeg.AudioHeader)codec;
                        //if (header == null)
                        //{
                        //    break;
                        //}

                    }

                    tags = GetID3Tags();

                    //TagLib.Mpeg.VBRIHeader b = new TagLib.Mpeg.VBRIHeader();
                    //TagLib.Mpeg.AudioHeader c = new TagLib.Mpeg.AudioHeader();
                }
                
                TagLib.File songInfo = TagLib.File.Create(FilePath);

                // Get album artist if available               
                if (songInfo.Tag.AlbumArtists.Length > 0)
                {
                    tags.ArtistName = songInfo.Tag.AlbumArtists[0];
                }
                // But by default we are taking the artists
                else if (songInfo.Tag.Artists.Length > 0)
                {
                    tags.ArtistName = songInfo.Tag.Artists[0];
                }

                // Get other tags
                if (!string.IsNullOrEmpty(songInfo.Tag.Album))
                {
                    tags.AlbumTitle = songInfo.Tag.Album;
                }

                // Set other properties
                tags.Title = songInfo.Tag.Title;
                tags.TrackNumber = songInfo.Tag.Track.ToString();
                tags.DiscNumber = songInfo.Tag.Disc.ToString();
                tags.Year = songInfo.Tag.Year.ToString();
                tags.Genre = string.Empty;                

                return tags;
            }
            catch (Exception ex)
            {
                // Continue with FMOD instead
            }

            // Determine sound format
            string extension = Path.GetExtension(FilePath).ToUpper();
            if(extension.Contains("FLAC") || extension.Contains("OGG"))
            {
                // Get Vorbis Comments
                tags = GetVorbisComments();
            }
            else if (extension.Contains("MP3"))
            {
                // Get ID3 tags
                tags = GetID3Tags();
            }

            return tags;
        }

        /// <summary>
        /// Returns the list of all Vorbis Comments (tags for FLAC and OGG) related with the sound object.
        /// </summary>
        /// <returns>List of Vorbis Comments (Tags object)</returns>
        public Tags GetVorbisComments()
        {
            // Create the list of tags
            Tags tags = new Tags();

            // Get the list of tags from FMOD
            List<FMOD.TAG> fmodTags = GetFMODTags();

            // Check if the list is valid
            if (fmodTags != null)
            {
                // For each tag...
                foreach (FMOD.TAG fmodTag in fmodTags)
                {
                    // Make sure it's a vorbis comment
                    if (fmodTag.type == FMOD.TAGTYPE.VORBISCOMMENT)
                    {
                        // Detect the type of comment
                        if (fmodTag.name.ToUpper() == "ARTIST")
                        {
                            tags.ArtistName = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "ALBUM")
                        {
                            tags.AlbumTitle = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TITLE")
                        {
                            tags.Title = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "GENRE")
                        {
                            tags.Genre = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TRACKNUMBER")
                        {
                            tags.TrackNumber = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "DATE")
                        {
                            
                        }
                    }
                }
            }

            return tags;
        }

        /// <summary>
        /// Returns the list of all ID3 tag values related with the sound object.
        /// </summary>
        /// <returns>List of ID3 tag values</returns>
        public Tags GetID3Tags()
        {                        
            // Create object
            Tags tags = new Tags();

            // Get the list of tags from FMOD
            List<FMOD.TAG> fmodTags = GetFMODTags();

            // Check if the list is valid
            if (fmodTags != null)
            {
                // For each tag...
                foreach (FMOD.TAG fmodTag in fmodTags)
                {
                    // Make sure it's an ID3 tag
                    if(fmodTag.type == FMOD.TAGTYPE.ID3V1 || fmodTag.type == FMOD.TAGTYPE.ID3V2)
                    {
                        if (fmodTag.name.ToUpper() == "ARTIST")
                        {
                            tags.ArtistName = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "ALBUM")
                        {
                            tags.AlbumTitle = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TITLE")
                        {
                            tags.Title = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "YEAR")
                        {
                            tags.Year = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TRACK")
                        {
                            tags.TrackNumber = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "GENRE")
                        {
                            tags.Genre = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TIT2")
                        {
                            tags.Tit2 = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TALB")
                        {
                            tags.Talb = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TSRC")
                        {
                            tags.Tsrc = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TCON")
                        {
                            tags.Tcon = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TDRC")
                        {
                            tags.Tdrc = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TRCK")
                        {
                            tags.Trck = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TPE1")
                        {
                            tags.Tpe1 = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "TXXX")
                        {
                            tags.Txxx = Marshal.PtrToStringAnsi(fmodTag.data);
                        }
                        else if (fmodTag.name == "APIC")
                        {
                            try
                            {
                                // Load binary data and transform to bitmap
                                byte[] byteAryPic = new byte[(int)fmodTag.datalen];
                                Marshal.Copy(fmodTag.data, byteAryPic, 0, (int)fmodTag.datalen);

                                //ID3PictureFrame frame = new ID3PictureFrame(byteAryPic);

                                //MemoryStream memStream = new MemoryStream();
                                //BinaryWriter binWriter = new BinaryWriter(memStream);

                                ////Write the data 
                                //for (int i = 13; i < byteAryPic.Length; i++)
                                //{
                                //    binWriter.Write(byteAryPic[i]);
                                //}

                                //tags.Picture = new Bitmap(memStream);

                                //binWriter.Close();
                                //memStream.Close();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }

            return tags;
        }

        #endregion

        public SoundFormat GetSoundFormat()
        {
            // Create the data structure
            SoundFormat soundFormat = new SoundFormat();
            soundFormat.Format = FMOD.SOUND_FORMAT.NONE;
           
            try
            {
                // Try with FMOD
                FMOD.SOUND_TYPE type = FMOD.SOUND_TYPE.UNKNOWN;
                FMOD.SOUND_FORMAT format = FMOD.SOUND_FORMAT.NONE;
                int channels = 0;
                int bits = 0;
                BaseSound.getFormat(ref type, ref format, ref channels, ref bits);                

                float frequency = 0;
                float pan = 0;
                float volume = 0;
                int priority = 0;
                BaseSound.getDefaults(ref frequency, ref pan, ref volume, ref priority);

                // Set properties
                soundFormat.Format = format;
                soundFormat.Channels = channels;
                soundFormat.BitsPerSample = bits;
                soundFormat.Frequency = (int)frequency;
                soundFormat.Bitrate = 0;
                soundFormat.Type = type;
            }
            catch (Exception ex)
            {
                // It doesn't work using FMOD; try to detect sound format using TagLib#

                // Get file information
                TagLib.File file = TagLib.File.Create(FilePath);

                // Set sound format
                if (file.Properties.BitsPerSample == 24)
                {
                    soundFormat.Format = FMOD.SOUND_FORMAT.PCM24;
                }
                else if (file.Properties.BitsPerSample == 16)
                {
                    soundFormat.Format = FMOD.SOUND_FORMAT.PCM16;
                }

                // Set other properties
                soundFormat.BitsPerSample = file.Properties.BitsPerSample;
                soundFormat.Channels = file.Properties.AudioChannels;
                soundFormat.Frequency = file.Properties.AudioSampleRate;
                soundFormat.Bitrate = file.Properties.AudioBitrate;
            }

            return soundFormat;
        }
    }

    /// <summary>
    /// Data structure defining the sound format. Can contain information from FMOD or TagLib#.
    /// </summary>
    public class SoundFormat
    {
        public FMOD.SOUND_FORMAT Format { get; set; }
        public FMOD.SOUND_TYPE Type { get; set; }
        public int Channels { get; set; }
        public int BitsPerSample { get; set; }
        public int Frequency { get; set; }
        public int Bitrate { get; set; }
    }

    /// <summary>
    /// Defines the common tags found in audio files.
    /// </summary>
    public class Tags
    {
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }
        public string Year { get; set; }
        public string TrackNumber { get; set; }
        public string TrackCount { get; set; }
        public string DiscNumber { get; set; }
        public string Genre { get; set; }

        public Bitmap Picture { get; set; }

        // ID3 tags
        public string Tit2 { get; set; }
        public string Talb { get; set; }
        public string Tsrc { get; set; }
        public string Tcon { get; set; }
        public string Tdrc { get; set; }
        public string Trck { get; set; }
        public string Tpe1 { get; set; }
        public string Txxx { get; set; }
    }

    public class SoundOpenState
    {
        public FMOD.OPENSTATE openState;
        public uint percentBuffered;
        public bool starving;
        public bool diskBusy;
    }
}

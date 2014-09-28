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
using Sessions.Sound.Tags;

namespace Sessions.Sound.AudioFiles
{
    public static class AudioFileLoader
    {
        public static void FromMP3(AudioFile audioFile)
        {
            TagLib.Mpeg.AudioFile file = null;
            try
            {
                file = new TagLib.Mpeg.AudioFile(audioFile.FilePath);
                audioFile.FirstBlockPosition = file.InvariantStartPosition;
                audioFile.LastBlockPosition = file.InvariantEndPosition;
                audioFile.FillProperties(file.Tag);

                // Loop through codecs (usually just one)
                foreach (TagLib.ICodec codec in file.Properties.Codecs)
                {
                    // Convert codec into a header 
                    var header = (TagLib.Mpeg.AudioHeader)codec;
                    audioFile.AudioChannels = header.AudioChannels;
                    audioFile.FrameLength = header.AudioFrameLength;
                    audioFile.AudioLayer = header.AudioLayer;
                    audioFile.SampleRate = header.AudioSampleRate;
                    audioFile.BitsPerSample = 16; // always 16-bit for MP3
                    audioFile.ChannelMode = header.ChannelMode;
                    audioFile.Bitrate = header.AudioBitrate;
                    audioFile.Length = ConvertAudio.ToTimeString(header.Duration);
                }
            }
            catch (Exception ex)
            {
                // Throw exception TODO: Check if file exists when catching the exception (to make a better error description)
                throw new Exception("An error occured while reading the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
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

        public static void FromFLAC(AudioFile audioFile)
        {
            TagLib.Flac.File file = null;
            try
            {
                // Read VorbisComment in FLAC file
                file = new TagLib.Flac.File(audioFile.FilePath);
                audioFile.FirstBlockPosition = file.InvariantStartPosition;
                audioFile.LastBlockPosition = file.InvariantEndPosition;
                audioFile.FillProperties(file.Tag);

                // Loop through codecs (usually just one)
                foreach (TagLib.ICodec codec in file.Properties.Codecs)
                {
                    var header = (TagLib.Flac.StreamHeader)codec;
                    audioFile.Bitrate = header.AudioBitrate;
                    audioFile.AudioChannels = header.AudioChannels;
                    audioFile.SampleRate = header.AudioSampleRate;
                    audioFile.BitsPerSample = header.BitsPerSample;
                    audioFile.Length = ConvertAudio.ToTimeString(header.Duration);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void FromOGG(AudioFile audioFile)
        {
            TagLib.Ogg.File file = null;
            try
            {
                // Read VorbisComment in OGG file
                file = new TagLib.Ogg.File(audioFile.FilePath);
                audioFile.FirstBlockPosition = file.InvariantStartPosition;
                audioFile.LastBlockPosition = file.InvariantEndPosition;
                audioFile.FillProperties(file.Tag);

                // Loop through codecs (usually just one)
                foreach (TagLib.ICodec codec in file.Properties.Codecs)
                {
                    // Check what kind of codec is used 
                    if (codec is TagLib.Ogg.Codecs.Theora)
                    {
                        // Do nothing, this is useless for audio.
                    }
                    else
                        if (codec is TagLib.Ogg.Codecs.Vorbis)
                        {
                            var header = (TagLib.Ogg.Codecs.Vorbis)codec;
                            audioFile.Bitrate = header.AudioBitrate;
                            audioFile.AudioChannels = header.AudioChannels;
                            audioFile.SampleRate = header.AudioSampleRate;
                            audioFile.BitsPerSample = 16;
                            audioFile.Length = ConvertAudio.ToTimeString(header.Duration);
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void FromAPE(AudioFile audioFile)
        {
            // Monkey's Audio (APE) supports APEv2 tags.
            // http://en.wikipedia.org/wiki/Monkey's_Audio

            TagLib.Ape.File file = null;
            try
            {
                // Read APE metadata
                audioFile.APETag = APEMetadata.Read(audioFile.FilePath);

                // Get APEv1/v2 tags from APE file
                file = new TagLib.Ape.File(audioFile.FilePath);
                audioFile.FirstBlockPosition = file.InvariantStartPosition;
                audioFile.LastBlockPosition = file.InvariantEndPosition;
                audioFile.FillProperties(file.Tag);

                // Loop through codecs (usually just one)
                foreach (TagLib.ICodec codec in file.Properties.Codecs)
                {
                    // Check what kind of codec is used 
                    if (codec is TagLib.Ape.StreamHeader)
                    {
                        var header = (TagLib.Ape.StreamHeader)codec;
                        audioFile.Bitrate = header.AudioBitrate;
                        audioFile.AudioChannels = header.AudioChannels;
                        audioFile.SampleRate = header.AudioSampleRate;
                        audioFile.BitsPerSample = 16;
                        audioFile.Length = ConvertAudio.ToTimeString(header.Duration);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void FromMPC(AudioFile audioFile)
        {
            // MusePack (MPC) supports APEv2 tags.
            // http://en.wikipedia.org/wiki/Musepack

            try
            {
                // Try to read SV8 header
                var sv8Tag = SV8Metadata.Read(audioFile.FilePath);
                audioFile.SV8Tag = sv8Tag;
                audioFile.AudioChannels = sv8Tag.AudioChannels;
                audioFile.SampleRate = sv8Tag.SampleRate;
                audioFile.BitsPerSample = 16;
                audioFile.Length = sv8Tag.Length;
                audioFile.Bitrate = sv8Tag.Bitrate;
            }
            catch (SV8TagNotFoundException exSV8)
            {
                try
                {
                    // Try to read the SV7 header
                    var sv7Tag = SV7Metadata.Read(audioFile.FilePath);
                    audioFile.SV7Tag = sv7Tag;
                    audioFile.AudioChannels = sv7Tag.AudioChannels;
                    audioFile.SampleRate = sv7Tag.SampleRate;
                    audioFile.BitsPerSample = 16;
                    audioFile.Length = sv7Tag.Length;
                    audioFile.Bitrate = sv7Tag.Bitrate;
                }
                catch (SV7TagNotFoundException exSV7)
                {
                    // No headers have been found!
                    var finalEx = new SV8TagNotFoundException(exSV8.Message, exSV7);
                    throw new Exception("Error: The file is not in SV7/MPC or SV8/MPC format!", finalEx);
                }
            }
            try
            {
                // Read APE tag
                var apeTag = APEMetadata.Read(audioFile.FilePath);
                audioFile.APETag = apeTag;
                audioFile.FillProperties(apeTag);
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

        public static void FromWV(AudioFile audioFile)
        {
            // WavPack supports APEv2 and ID3v1 tags.
            // http://www.wavpack.com/wavpack_doc.html

            TagLib.WavPack.File file = null;

            try
            {
                // Read WavPack tags
                file = new TagLib.WavPack.File(audioFile.FilePath);
                audioFile.FirstBlockPosition = file.InvariantStartPosition;
                audioFile.LastBlockPosition = file.InvariantEndPosition;
                audioFile.FillProperties(file.Tag);

                // Loop through codecs (usually just one)
                foreach (TagLib.ICodec codec in file.Properties.Codecs)
                {
                    // Check what kind of codec is used 
                    if (codec is TagLib.WavPack.StreamHeader)
                    {
                        var header = (TagLib.WavPack.StreamHeader)codec;
                        audioFile.Bitrate = header.AudioBitrate;
                        audioFile.AudioChannels = header.AudioChannels;
                        audioFile.SampleRate = header.AudioSampleRate;
                        audioFile.BitsPerSample = 16;
                        audioFile.Length = ConvertAudio.ToTimeString(header.Duration);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void FromTTA(AudioFile audioFile)
        {
            // The True Audio (TTA) format supports ID3v1, ID3v2 and APEv2 tags.
            // http://en.wikipedia.org/wiki/TTA_(codec)
            audioFile.AudioChannels = 2;
            audioFile.SampleRate = 44100;
            audioFile.BitsPerSample = 16;
            // TagLib doesn't work.
        }

        public static void FromOFC(AudioFile audioFile)
        {
            // TagLib does not support OFR files...
            // OptimFROG (OFR) supports APEv2 tags.
            // http://en.wikipedia.org/wiki/OptimFROG                
        }

        public static void FromWAV(AudioFile audioFile)
        {
            TagLib.Riff.File file = null;
            try
            {
                file = new TagLib.Riff.File(audioFile.FilePath);
                audioFile.FirstBlockPosition = file.InvariantStartPosition;
                audioFile.LastBlockPosition = file.InvariantEndPosition;
                audioFile.FillProperties(file.Tag);

                // Loop through codecs (usually just one)
                foreach (TagLib.ICodec codec in file.Properties.Codecs)
                {
                    // Check what kind of codec is used 
                    if (codec is TagLib.Riff.WaveFormatEx)
                    {
                        var header = (TagLib.Riff.WaveFormatEx)codec;
                        audioFile.Bitrate = header.AudioBitrate;
                        audioFile.AudioChannels = header.AudioChannels;
                        audioFile.SampleRate = header.AudioSampleRate;
                        audioFile.BitsPerSample = 16;
                        audioFile.Length = ConvertAudio.ToTimeString(header.Duration);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void FromWMA(AudioFile audioFile)
        {
            TagLib.Asf.File file = null;
            try
            {
                // Read ASF/WMA tags
                file = new TagLib.Asf.File(audioFile.FilePath);
                audioFile.FirstBlockPosition = file.InvariantStartPosition;
                audioFile.LastBlockPosition = file.InvariantEndPosition;
                audioFile.FillProperties(file.Tag);

                // The right length is here, not in the codec data structure
                audioFile.Length = ConvertAudio.ToTimeString(file.Properties.Duration);

                // Loop through codecs (usually just one)
                foreach (TagLib.ICodec codec in file.Properties.Codecs)
                {
                    // Check what kind of codec is used 
                    if (codec is TagLib.Riff.WaveFormatEx)
                    {
                        var header = (TagLib.Riff.WaveFormatEx)codec;
                        audioFile.Bitrate = header.AudioBitrate;
                        audioFile.AudioChannels = header.AudioChannels;
                        audioFile.SampleRate = header.AudioSampleRate;
                        audioFile.BitsPerSample = 16;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void FromAAC(AudioFile audioFile)
        {
            TagLib.Aac.File file = null;
            try
            {
                // Read AAC tags
                file = new TagLib.Aac.File(audioFile.FilePath);
                // Doesn't seem to work very well...
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }
    }
}

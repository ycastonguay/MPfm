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
using System.Runtime.InteropServices;

namespace MPfm.Sound.BassWrapper.FX
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_APF
    {
        public float fGain;
        public float fDelay;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_APF()
        {
        }
        public BASS_BFX_APF(float Gain, float Delay)
        {
            this.fGain = Gain;
            this.fDelay = Delay;
        }
        public BASS_BFX_APF(float Gain, float Delay, BASSFXChan chans)
        {
            this.fGain = Gain;
            this.fDelay = Delay;
            this.lChannel = chans;
        }
        public void Preset_Default()
        {
            this.fGain = -0.5f;
            this.fDelay = 0.5f;
        }
        public void Preset_SmallRever()
        {
            this.fGain = 0.799f;
            this.fDelay = 0.2f;
        }
        public void Preset_RobotVoice()
        {
            this.fGain = 0.6f;
            this.fDelay = 0.05f;
        }
        public void Preset_LongReverberation()
        {
            this.fGain = 0.599f;
            this.fDelay = 1.3f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_AUTOWAH
    {
        public float fDryMix;
        public float fWetMix;
        public float fFeedback;
        public float fRate;
        public float fRange;
        public float fFreq;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_AUTOWAH()
        {
        }
        public BASS_BFX_AUTOWAH(float DryMix, float WetMix, float Feedback, float Rate, float Range, float Freq)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fRate = Rate;
            this.fRange = Range;
            this.fFreq = Freq;
        }
        public BASS_BFX_AUTOWAH(float DryMix, float WetMix, float Feedback, float Rate, float Range, float Freq, BASSFXChan chans)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fRate = Rate;
            this.fRange = Range;
            this.fFreq = Freq;
            this.lChannel = chans;
        }
        public void Preset_Default()
        {
            this.fDryMix = -1f;
            this.fWetMix = 1f;
            this.fFeedback = 0.06f;
            this.fRate = 0.2f;
            this.fRange = 6f;
            this.fFreq = 100f;
        }
        public void Preset_SlowAutoWah()
        {
            this.fDryMix = 0.5f;
            this.fWetMix = 1.5f;
            this.fFeedback = 0.5f;
            this.fRate = 2f;
            this.fRange = 4.3f;
            this.fFreq = 50f;
        }
        public void Preset_FastAutoWah()
        {
            this.fDryMix = 0.5f;
            this.fWetMix = 1.5f;
            this.fFeedback = 0.5f;
            this.fRate = 5f;
            this.fRange = 5.3f;
            this.fFreq = 50f;
        }
        public void Preset_HiFastAutoWah()
        {
            this.fDryMix = 0.5f;
            this.fWetMix = 1.5f;
            this.fFeedback = 0.5f;
            this.fRate = 5f;
            this.fRange = 4.3f;
            this.fFreq = 500f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_BQF
    {
        public BASSBFXBQF lFilter = BASSBFXBQF.BASS_BFX_BQF_ALLPASS;
        public float fCenter = 200f;
        public float fGain;
        public float fBandwidth = 1f;
        public float fQ;
        public float fS;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_BQF()
        {
        }
        public BASS_BFX_BQF(BASSBFXBQF filter, float center, float gain, float bandwidth, float q, float s, BASSFXChan chans)
        {
            this.lFilter = filter;
            this.fCenter = center;
            this.fGain = gain;
            this.fBandwidth = bandwidth;
            this.fQ = q;
            this.fS = s;
            this.lChannel = chans;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_CHORUS
    {
        public float fDryMix;
        public float fWetMix;
        public float fFeedback;
        public float fMinSweep;
        public float fMaxSweep;
        public float fRate;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_CHORUS()
        {
        }
        public BASS_BFX_CHORUS(float DryMix, float WetMix, float Feedback, float MinSweep, float MaxSweep, float Rate)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fMinSweep = MinSweep;
            this.fMaxSweep = MaxSweep;
            this.fRate = Rate;
        }
        public BASS_BFX_CHORUS(float DryMix, float WetMix, float Feedback, float MinSweep, float MaxSweep, float Rate, BASSFXChan chans)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fMinSweep = MinSweep;
            this.fMaxSweep = MaxSweep;
            this.fRate = Rate;
            this.lChannel = chans;
        }
        public void Preset_Flanger()
        {
            this.fDryMix = 1f;
            this.fWetMix = 0.35f;
            this.fFeedback = 0.5f;
            this.fMinSweep = 1f;
            this.fMaxSweep = 5f;
            this.fRate = 1f;
        }
        public void Preset_ExaggeratedChorusLTMPitchSshiftedVoices()
        {
            this.fDryMix = 0.7f;
            this.fWetMix = 0.25f;
            this.fFeedback = 0.5f;
            this.fMinSweep = 1f;
            this.fMaxSweep = 200f;
            this.fRate = 50f;
        }
        public void Preset_Motocycle()
        {
            this.fDryMix = 0.9f;
            this.fWetMix = 0.45f;
            this.fFeedback = 0.5f;
            this.fMinSweep = 1f;
            this.fMaxSweep = 100f;
            this.fRate = 25f;
        }
        public void Preset_Devil()
        {
            this.fDryMix = 0.9f;
            this.fWetMix = 0.35f;
            this.fFeedback = 0.5f;
            this.fMinSweep = 1f;
            this.fMaxSweep = 50f;
            this.fRate = 200f;
        }
        public void Preset_WhoSayTTNManyVoices()
        {
            this.fDryMix = 0.9f;
            this.fWetMix = 0.35f;
            this.fFeedback = 0.5f;
            this.fMinSweep = 1f;
            this.fMaxSweep = 400f;
            this.fRate = 200f;
        }
        public void Preset_BackChipmunk()
        {
            this.fDryMix = 0.9f;
            this.fWetMix = -0.2f;
            this.fFeedback = 0.5f;
            this.fMinSweep = 1f;
            this.fMaxSweep = 400f;
            this.fRate = 400f;
        }
        public void Preset_Water()
        {
            this.fDryMix = 0.9f;
            this.fWetMix = -0.4f;
            this.fFeedback = 0.5f;
            this.fMinSweep = 1f;
            this.fMaxSweep = 2f;
            this.fRate = 1f;
        }
        public void Preset_ThisIsTheAirplane()
        {
            this.fDryMix = 0.3f;
            this.fWetMix = 0.4f;
            this.fFeedback = 0.5f;
            this.fMinSweep = 1f;
            this.fMaxSweep = 10f;
            this.fRate = 5f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class BASS_BFX_COMPRESSOR
    {
        public float fThreshold;
        public float fAttacktime;
        public float fReleasetime;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_COMPRESSOR()
        {
        }
        public BASS_BFX_COMPRESSOR(float Threshold, float Attacktime, float Releasetime)
        {
            this.fThreshold = Threshold;
            this.fAttacktime = Attacktime;
            this.fReleasetime = Releasetime;
        }
        public BASS_BFX_COMPRESSOR(float Threshold, float Attacktime, float Releasetime, BASSFXChan chans)
        {
            this.fThreshold = Threshold;
            this.fAttacktime = Attacktime;
            this.fReleasetime = Releasetime;
            this.lChannel = chans;
        }
        public void Preset_50Attack15msRelease1sec()
        {
            this.fThreshold = 0.5f;
            this.fAttacktime = 15f;
            this.fReleasetime = 1000f;
        }
        public void Preset_80Attack1msRelease05sec()
        {
            this.fThreshold = 0.8f;
            this.fAttacktime = 1f;
            this.fReleasetime = 500f;
        }
        public void Preset_Soft()
        {
            this.fThreshold = 0.89f;
            this.fAttacktime = 20f;
            this.fReleasetime = 350f;
        }
        public void Preset_SoftHigh()
        {
            this.fThreshold = 0.7f;
            this.fAttacktime = 10f;
            this.fReleasetime = 200f;
        }
        public void Preset_Medium()
        {
            this.fThreshold = 0.5f;
            this.fAttacktime = 5f;
            this.fReleasetime = 250f;
        }
        public void Preset_Hard()
        {
            this.fThreshold = 0.25f;
            this.fAttacktime = 2.2f;
            this.fReleasetime = 400f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_COMPRESSOR2
    {
        public float fGain = 5f;
        public float fThreshold = -15f;
        public float fRatio = 3f;
        public float fAttack = 20f;
        public float fRelease = 200f;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_COMPRESSOR2()
        {
        }
        public BASS_BFX_COMPRESSOR2(float Gain, float Threshold, float Ratio, float Attack, float Release, BASSFXChan chans)
        {
            this.fGain = Gain;
            this.fThreshold = Threshold;
            this.fRatio = Ratio;
            this.fAttack = Attack;
            this.fRelease = Release;
            this.lChannel = chans;
        }
        public void Calculate0dBGain()
        {
            this.fGain = this.fThreshold / 2f * (1f / this.fRatio - 1f);
        }
        public void Preset_Default()
        {
            this.fThreshold = -15f;
            this.fRatio = 3f;
            this.fGain = 5f;
            this.fAttack = 20f;
            this.fRelease = 200f;
        }
        public void Preset_Soft()
        {
            this.fThreshold = -15f;
            this.fRatio = 2f;
            this.fGain = 3.7f;
            this.fAttack = 24f;
            this.fRelease = 800f;
        }
        public void Preset_Soft2()
        {
            this.fThreshold = -18f;
            this.fRatio = 3f;
            this.fGain = 6f;
            this.fAttack = 24f;
            this.fRelease = 800f;
        }
        public void Preset_Medium()
        {
            this.fThreshold = -20f;
            this.fRatio = 4f;
            this.fGain = 7.5f;
            this.fAttack = 16f;
            this.fRelease = 500f;
        }
        public void Preset_Hard()
        {
            this.fThreshold = -23f;
            this.fRatio = 8f;
            this.fGain = 10f;
            this.fAttack = 12f;
            this.fRelease = 400f;
        }
        public void Preset_Hard2()
        {
            this.fThreshold = -18f;
            this.fRatio = 9f;
            this.fGain = 8f;
            this.fAttack = 12f;
            this.fRelease = 200f;
        }
        public void Preset_HardCommercial()
        {
            this.fThreshold = -20f;
            this.fRatio = 10f;
            this.fGain = 9f;
            this.fAttack = 8f;
            this.fRelease = 250f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_DAMP
    {
        public float fTarget = 1f;
        public float fQuiet;
        public float fRate;
        public float fGain;
        public float fDelay;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_DAMP()
        {
        }
        public BASS_BFX_DAMP(float Target, float Quiet, float Rate, float Gain, float Delay)
        {
            this.fTarget = Target;
            this.fQuiet = Quiet;
            this.fRate = Rate;
            this.fGain = Gain;
            this.fDelay = Delay;
        }
        public BASS_BFX_DAMP(float Target, float Quiet, float Rate, float Gain, float Delay, BASSFXChan chans)
        {
            this.fTarget = Target;
            this.fQuiet = Quiet;
            this.fRate = Rate;
            this.fGain = Gain;
            this.fDelay = Delay;
            this.lChannel = chans;
        }
        public void Preset_Soft()
        {
            this.fTarget = 0.92f;
            this.fQuiet = 0.02f;
            this.fRate = 0.01f;
            this.fGain = 1f;
            this.fDelay = 0.5f;
        }
        public void Preset_Medium()
        {
            this.fTarget = 0.94f;
            this.fQuiet = 0.03f;
            this.fRate = 0.01f;
            this.fGain = 1f;
            this.fDelay = 0.35f;
        }
        public void Preset_Hard()
        {
            this.fTarget = 0.98f;
            this.fQuiet = 0.04f;
            this.fRate = 0.02f;
            this.fGain = 2f;
            this.fDelay = 0.2f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_DISTORTION
    {
        public float fDrive;
        public float fDryMix;
        public float fWetMix;
        public float fFeedback;
        public float fVolume;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_DISTORTION()
        {
        }
        public BASS_BFX_DISTORTION(float Drive, float DryMix, float WetMix, float Feedback, float Volume)
        {
            this.fDrive = Drive;
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fVolume = Volume;
        }
        public BASS_BFX_DISTORTION(float Drive, float DryMix, float WetMix, float Feedback, float Volume, BASSFXChan chans)
        {
            this.fDrive = Drive;
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fVolume = Volume;
            this.lChannel = chans;
        }
        public void Preset_HardDistortion()
        {
            this.fDrive = 1f;
            this.fDryMix = 0f;
            this.fWetMix = 1f;
            this.fFeedback = 0f;
            this.fVolume = 1f;
        }
        public void Preset_VeryHardDistortion()
        {
            this.fDrive = 5f;
            this.fDryMix = 0f;
            this.fWetMix = 1f;
            this.fFeedback = 0.1f;
            this.fVolume = 1f;
        }
        public void Preset_MediumDistortion()
        {
            this.fDrive = 0.2f;
            this.fDryMix = 1f;
            this.fWetMix = 1f;
            this.fFeedback = 0.1f;
            this.fVolume = 1f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_ECHO
    {
        public float fLevel;
        public int lDelay = 1200;
        public BASS_BFX_ECHO()
        {
        }
        public BASS_BFX_ECHO(float Level, int Delay)
        {
            this.fLevel = Level;
            this.lDelay = Delay;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_ECHO2
    {
        public float fDryMix;
        public float fWetMix;
        public float fFeedback;
        public float fDelay;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_ECHO2()
        {
        }
        public BASS_BFX_ECHO2(float DryMix, float WetMix, float Feedback, float Delay)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fDelay = Delay;
        }
        public BASS_BFX_ECHO2(float DryMix, float WetMix, float Feedback, float Delay, BASSFXChan chans)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fDelay = Delay;
            this.lChannel = chans;
        }
        public void Preset_SmallEcho()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = 0f;
            this.fDelay = 0.2f;
        }
        public void Preset_ManyEchoes()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = 0.7f;
            this.fDelay = 0.5f;
        }
        public void Preset_ReverseEchoes()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = -0.7f;
            this.fDelay = 0.8f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_ECHO3
    {
        public float fDryMix;
        public float fWetMix;
        public float fDelay;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_ECHO3()
        {
        }
        public BASS_BFX_ECHO3(float DryMix, float WetMix, float Delay)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fDelay = Delay;
        }
        public BASS_BFX_ECHO3(float DryMix, float WetMix, float Delay, BASSFXChan chans)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fDelay = Delay;
            this.lChannel = chans;
        }
        public void Preset_SmallEcho()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fDelay = 0.2f;
        }
        public void Preset_DoubleKick()
        {
            this.fDryMix = 0.5f;
            this.fWetMix = 0.599f;
            this.fDelay = 0.5f;
        }
        public void Preset_LongEcho()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.699f;
            this.fDelay = 0.9f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct BASS_BFX_ENV_NODE
    {
        public double pos;
        public float val;
        public BASS_BFX_ENV_NODE(double Pos, float Val)
        {
            this.pos = Pos;
            this.val = Val;
        }
        public override string ToString()
        {
            return string.Format("Pos={0}, Val={1}", this.pos, this.val);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_FLANGER
    {
        public float fWetDry = 1f;
        public float fSpeed = 0.01f;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_FLANGER()
        {
        }
        public BASS_BFX_FLANGER(float WetDry, float Speed)
        {
            this.fWetDry = WetDry;
            this.fSpeed = Speed;
        }
        public BASS_BFX_FLANGER(float WetDry, float Speed, BASSFXChan chans)
        {
            this.fWetDry = WetDry;
            this.fSpeed = Speed;
            this.lChannel = chans;
        }
        public void Preset_Default()
        {
            this.fWetDry = 1f;
            this.fSpeed = 0.01f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_LPF
    {
        public float fResonance = 2f;
        public float fCutOffFreq = 200f;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_LPF()
        {
        }
        public BASS_BFX_LPF(float Resonance, float CutOffFreq)
        {
            this.fResonance = Resonance;
            this.fCutOffFreq = CutOffFreq;
        }
        public BASS_BFX_LPF(float Resonance, float CutOffFreq, BASSFXChan chans)
        {
            this.fResonance = Resonance;
            this.fCutOffFreq = CutOffFreq;
            this.lChannel = chans;
        }
    }

    //[Serializable]
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    //public sealed class BASS_BFX_MIX : IDisposable
    //{
    //    private IntPtr a = IntPtr.Zero;
    //    private GCHandle b;
    //    public BASSFXChan[] lChannel;
    //    public BASS_BFX_MIX(int numChans)
    //    {
    //        this.lChannel = new BASSFXChan[numChans];
    //        for (int i = 0; i < numChans; i++)
    //        {
    //            this.lChannel[i] = (BASSFXChan)(1 << i);
    //        }
    //    }
    //    public BASS_BFX_MIX(params BASSFXChan[] channels)
    //    {
    //        this.lChannel = new BASSFXChan[channels.Length];
    //        for (int i = 0; i < channels.Length; i++)
    //        {
    //            this.lChannel[i] = channels[i];
    //        }
    //    }
    //    ~BASS_BFX_MIX()
    //    {
    //        this.Dispose();
    //    }
    //    internal void a()
    //    {
    //        if (this.b.IsAllocated)
    //        {
    //            this.b.Free();
    //            this.a = IntPtr.Zero;
    //        }
    //        int[] array = new int[this.lChannel.Length];
    //        for (int i = 0; i < this.lChannel.Length; i++)
    //        {
    //            array[i] = (int)this.lChannel[i];
    //        }
    //        this.b = GCHandle.Alloc(array, GCHandleType.Pinned);
    //        this.a = this.b.AddrOfPinnedObject();
    //    }
    //    internal void b()
    //    {
    //        if (this.a != IntPtr.Zero)
    //        {
    //            int[] array = new int[this.lChannel.Length];
    //            Marshal.Copy(this.a, array, 0, array.Length);
    //            for (int i = 0; i < array.Length; i++)
    //            {
    //                this.lChannel[i] = (BASSFXChan)array[i];
    //            }
    //        }
    //    }
    //    public void Dispose()
    //    {
    //        if (this.b.IsAllocated)
    //        {
    //            this.b.Free();
    //            this.a = IntPtr.Zero;
    //        }
    //    }
    //}

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_PEAKEQ
    {
        public int lBand;
        public float fBandwidth = 1f;
        public float fQ;
        public float fCenter = 1000f;
        public float fGain;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_PEAKEQ()
        {
        }
        public BASS_BFX_PEAKEQ(int Band, float Bandwidth, float Q, float Center, float Gain)
        {
            this.lBand = Band;
            this.fBandwidth = Bandwidth;
            this.fQ = Q;
            this.fCenter = Center;
            this.fGain = Gain;
        }
        public BASS_BFX_PEAKEQ(int Band, float Bandwidth, float Q, float Center, float Gain, BASSFXChan chans)
        {
            this.lBand = Band;
            this.fBandwidth = Bandwidth;
            this.fQ = Q;
            this.fCenter = Center;
            this.fGain = Gain;
            this.lChannel = chans;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_PHASER
    {
        public float fDryMix;
        public float fWetMix;
        public float fFeedback;
        public float fRate;
        public float fRange;
        public float fFreq;
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public BASS_BFX_PHASER()
        {
        }
        public BASS_BFX_PHASER(float DryMix, float WetMix, float Feedback, float Rate, float Range, float Freq)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fRate = Rate;
            this.fRange = Range;
            this.fFreq = Freq;
        }
        public BASS_BFX_PHASER(float DryMix, float WetMix, float Feedback, float Rate, float Range, float Freq, BASSFXChan chans)
        {
            this.fDryMix = DryMix;
            this.fWetMix = WetMix;
            this.fFeedback = Feedback;
            this.fRate = Rate;
            this.fRange = Range;
            this.fFreq = Freq;
            this.lChannel = chans;
        }
        public void Preset_Default()
        {
            this.fDryMix = -1f;
            this.fWetMix = 1f;
            this.fFeedback = 0.06f;
            this.fRate = 0.2f;
            this.fRange = 6f;
            this.fFreq = 100f;
        }
        public void Preset_PhaseShift()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = 0f;
            this.fRate = 1f;
            this.fRange = 4f;
            this.fFreq = 100f;
        }
        public void Preset_SlowInvertPhaseShiftWithFeedback()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = -0.999f;
            this.fFeedback = -0.6f;
            this.fRate = 0.2f;
            this.fRange = 6f;
            this.fFreq = 100f;
        }
        public void Preset_BasicPhase()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = 0f;
            this.fRate = 1f;
            this.fRange = 4.3f;
            this.fFreq = 50f;
        }
        public void Preset_PhaseWithFeedback()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = 0f;
            this.fRate = 1f;
            this.fRange = 4.3f;
            this.fFreq = 50f;
        }
        public void Preset_MediumPhase()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = 0f;
            this.fRate = 1f;
            this.fRange = 7f;
            this.fFreq = 100f;
        }
        public void Preset_FastPhase()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = 0f;
            this.fRate = 1f;
            this.fRange = 7f;
            this.fFreq = 400f;
        }
        public void Preset_InvertWithInvertFeedback()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = -0.999f;
            this.fFeedback = -0.2f;
            this.fRate = 1f;
            this.fRange = 7f;
            this.fFreq = 200f;
        }
        public void Preset_TremoloWah()
        {
            this.fDryMix = 0.999f;
            this.fWetMix = 0.999f;
            this.fFeedback = 0.6f;
            this.fRate = 1f;
            this.fRange = 4f;
            this.fFreq = 60f;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_REVERB
    {
        public float fLevel;
        public int lDelay = 1200;
        public BASS_BFX_REVERB()
        {
        }
        public BASS_BFX_REVERB(float Level, int Delay)
        {
            this.fLevel = Level;
            this.lDelay = Delay;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class BASS_BFX_VOLUME
    {
        public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
        public float fVolume = 1f;
        public BASS_BFX_VOLUME()
        {
        }
        public BASS_BFX_VOLUME(float Volume)
        {
            this.fVolume = Volume;
        }
        public BASS_BFX_VOLUME(float Volume, BASSFXChan chans)
        {
            this.fVolume = Volume;
            this.lChannel = chans;
        }
    }

    //[Serializable]
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    //public sealed class BASS_BFX_VOLUME_ENV : IDisposable
    //{
    //    public BASSFXChan lChannel = BASSFXChan.BASS_BFX_CHANALL;
    //    public int lNodeCount;
    //    private IntPtr a = IntPtr.Zero;
    //    [MarshalAs(UnmanagedType.Bool)]
    //    public bool bFollow = true;
    //    private GCHandle b;
    //    public BASS_BFX_ENV_NODE[] pNodes;
    //    public BASS_BFX_VOLUME_ENV()
    //    {
    //    }
    //    public BASS_BFX_VOLUME_ENV(int nodeCount)
    //    {
    //        this.lNodeCount = nodeCount;
    //        this.pNodes = new BASS_BFX_ENV_NODE[this.lNodeCount];
    //        for (int i = 0; i < this.lNodeCount; i++)
    //        {
    //            this.pNodes[i].pos = 0.0;
    //            this.pNodes[i].val = 1f;
    //        }
    //    }
    //    public BASS_BFX_VOLUME_ENV(params BASS_BFX_ENV_NODE[] nodes)
    //    {
    //        if (nodes == null)
    //        {
    //            this.lNodeCount = 0;
    //            this.pNodes = null;
    //            return;
    //        }
    //        this.lNodeCount = nodes.Length;
    //        this.pNodes = new BASS_BFX_ENV_NODE[this.lNodeCount];
    //        for (int i = 0; i < this.lNodeCount; i++)
    //        {
    //            this.pNodes[i].pos = nodes[i].pos;
    //            this.pNodes[i].val = nodes[i].val;
    //        }
    //    }
    //    ~BASS_BFX_VOLUME_ENV()
    //    {
    //        this.Dispose();
    //    }
    //    internal void a()
    //    {
    //        if (this.lNodeCount > 0)
    //        {
    //            if (this.b.IsAllocated)
    //            {
    //                this.b.Free();
    //                this.a = IntPtr.Zero;
    //            }
    //            this.b = GCHandle.Alloc(this.pNodes, GCHandleType.Pinned);
    //            this.a = this.b.AddrOfPinnedObject();
    //            return;
    //        }
    //        if (this.b.IsAllocated)
    //        {
    //            this.b.Free();
    //        }
    //        this.a = IntPtr.Zero;
    //    }
    //    internal void b()
    //    {
    //        if (this.a != IntPtr.Zero && this.lNodeCount > 0)
    //        {
    //            this.pNodes = new BASS_BFX_ENV_NODE[this.lNodeCount];
    //            this.a(this.lNodeCount, this.a);
    //            return;
    //        }
    //        this.pNodes = null;
    //    }
    //    private void a(int A_0, IntPtr A_1)
    //    {
    //        for (int i = 0; i < A_0; i++)
    //        {
    //            this.pNodes[i] = (BASS_BFX_ENV_NODE)Marshal.PtrToStructure(A_1, typeof(BASS_BFX_ENV_NODE));
    //            A_1 = new IntPtr(A_1.ToPointer() + (IntPtr)Marshal.SizeOf(this.pNodes[i]) / sizeof(void));
    //        }
    //    }
    //    public void Dispose()
    //    {
    //        if (this.b.IsAllocated)
    //        {
    //            this.b.Free();
    //            this.a = IntPtr.Zero;
    //        }
    //    }
    //}
}

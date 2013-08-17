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
using System.Linq;
using MPfm.MVP;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.Mac.Classes.Objects;

namespace MPfm.Mac
{
    public partial class EffectsWindowController : BaseWindowController, IDesktopEffectsView
    {
        public EffectsWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        public EffectsWindowController(Action<IBaseView> onViewReady) 
            : base ("EffectsWindow", onViewReady)
        {
            Initialize();
        }
        
        private void Initialize()
        {
            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);
        }
        
        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            LoadFontsAndImages();
            OnViewReady.Invoke(this);
        }

        private void LoadFontsAndImages()
        {
            viewBackground.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackground.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundPreset.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackgroundPreset.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundPreset.HeaderColor1 = GlobalTheme.PanelHeaderColor1;
            viewBackgroundPreset.HeaderColor2 = GlobalTheme.PanelHeaderColor2;
            viewBackgroundPreset.IsHeaderVisible = true;
            viewBackgroundInformation.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewBackgroundInformation.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            viewBackgroundInformation.HeaderColor1 = GlobalTheme.PanelHeaderColor1;
            viewBackgroundInformation.HeaderColor2 = GlobalTheme.PanelHeaderColor2;
            viewBackgroundInformation.IsHeaderVisible = true;

            popupPreset.Font = NSFont.FromFontName("Junction", 11f);
            lblName.Font = NSFont.FromFontName("Junction", 11f);
            lblTitleInformation.Font = NSFont.FromFontName("TitilliumText25L-800wt", 13f);
            lblTitlePreset.Font = NSFont.FromFontName("TitilliumText25L-800wt", 13f);
            lblEQOn.Font = NSFont.FromFontName("Junction", 11f);
            lblScalePlus6.Font = NSFont.FromFontName("Junction", 11f);
            lblScale0.Font = NSFont.FromFontName("Junction", 11f);
            lblScaleMinus6.Font = NSFont.FromFontName("Junction", 11f);
            txtName.Font = NSFont.FromFontName("Junction", 11f);

            btnAutoLevel.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_shape_align_middle");
            btnDelete.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_delete");
            btnSave.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_tick");
            btnReset.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_fam_exclamation");

            SetTheme();
        }

        private void SetTheme()
        {
//            // Set colors
//            viewLeftHeader.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewLeftHeader.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewRightHeader.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewRightHeader.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewLibraryBrowser.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewLibraryBrowser.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            viewNowPlaying.GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f, 1.0f);
//            viewNowPlaying.GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f, 1.0f);
//            
//            // Set label fonts
            lblEQ0.Font = NSFont.FromFontName("Junction", 11);
            lblEQ1.Font = NSFont.FromFontName("Junction", 11);
            lblEQ2.Font = NSFont.FromFontName("Junction", 11);
            lblEQ3.Font = NSFont.FromFontName("Junction", 11);
            lblEQ4.Font = NSFont.FromFontName("Junction", 11);
            lblEQ5.Font = NSFont.FromFontName("Junction", 11);
            lblEQ6.Font = NSFont.FromFontName("Junction", 11);
            lblEQ7.Font = NSFont.FromFontName("Junction", 11);
            lblEQ8.Font = NSFont.FromFontName("Junction", 11);
            lblEQ9.Font = NSFont.FromFontName("Junction", 11);
            lblEQ10.Font = NSFont.FromFontName("Junction", 11);
            lblEQ11.Font = NSFont.FromFontName("Junction", 11);
            lblEQ12.Font = NSFont.FromFontName("Junction", 11);
            lblEQ13.Font = NSFont.FromFontName("Junction", 11);
            lblEQ14.Font = NSFont.FromFontName("Junction", 11);
            lblEQ15.Font = NSFont.FromFontName("Junction", 11);
            lblEQ16.Font = NSFont.FromFontName("Junction", 11);
            lblEQ17.Font = NSFont.FromFontName("Junction", 11);

            lblEQValue0.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue1.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue2.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue3.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue4.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue5.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue6.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue7.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue8.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue9.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue10.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue11.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue12.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue13.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue14.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue15.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue16.Font = NSFont.FromFontName("Junction", 11);
            lblEQValue17.Font = NSFont.FromFontName("Junction", 11);
        }

        partial void actionPresetChange(NSObject sender)
        {
        }

        partial void actionEQOnChange(NSObject sender)
        {
        }

        partial void actionNameChanged(NSObject sender)
        {
        }

        partial void actionSave(NSObject sender)
        {
        }

        partial void actionDelete(NSObject sender)
        {
        }

        partial void actionAutoLevel(NSObject sender)
        {
        }

        partial void actionReset(NSObject sender)
        {
        }

        partial void actionSlider0ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider1ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider2ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider3ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider4ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider5ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider6ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider7ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider8ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider9ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider10ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider11ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider12ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider13ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider14ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider15ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider16ChangeValue(NSObject sender)
        {
        }

        partial void actionSlider17ChangeValue(NSObject sender)
        {
        }

        #region IEqualizerPresetDetailsView implementation

        public Action OnResetPreset { get; set; }
        public Action OnNormalizePreset { get; set; }
        public Action OnRevertPreset { get; set; }
        public Action<string> OnSavePreset { get; set; }
        public Action<string, float> OnSetFaderGain { get; set; }

        public void EqualizerPresetDetailsError(Exception ex)
        {
        }

        public void ShowMessage(string title, string message)
        {
        }

        public void RefreshPreset(EQPreset preset)
        {
        }

        #endregion

        #region IEqualizerPresetsView implementation

        public Action OnBypassEqualizer { get; set; }
        public Action<float> OnSetVolume { get; set; }
        public Action OnAddPreset { get; set; }
        public Action<Guid> OnLoadPreset { get; set; }
        public Action<Guid> OnEditPreset { get; set; }
        public Action<Guid> OnDeletePreset { get; set; }

        public void EqualizerPresetsError(Exception ex)
        {
        }

        public void RefreshPresets(IEnumerable<EQPreset> presets, Guid selectedPresetId, bool isEQBypassed)
        {
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
        }

        public void RefreshVolume(float volume)
        {
        }

        #endregion


//        #region IEffectsView implementation
//        
//        public void UpdateFader(int index, float value)
//        {
//            InvokeOnMainThread(() => {
//                string strValue = value.ToString("0.0") + " dB";
//                if (index == 0)
//                {
//                    sliderEQ0.FloatValue = value;
//                    lblEQValue0.StringValue = strValue;
//                } 
//                else if (index == 1)
//                {
//                    sliderEQ1.FloatValue = value;
//                    lblEQValue1.StringValue = strValue;
//                }
//                else if (index == 2)
//                {
//                    sliderEQ2.FloatValue = value;
//                    lblEQValue2.StringValue = strValue;
//                }
//                else if (index == 3)
//                {
//                    sliderEQ3.FloatValue = value;
//                    lblEQValue3.StringValue = strValue;
//                }
//                else if (index == 4)
//                {
//                    sliderEQ4.FloatValue = value;
//                    lblEQValue4.StringValue = strValue;
//                }
//                else if (index == 5)
//                {
//                    sliderEQ5.FloatValue = value;
//                    lblEQValue5.StringValue = strValue;
//                }
//                else if (index == 6)
//                {
//                    sliderEQ6.FloatValue = value;
//                    lblEQValue6.StringValue = strValue;
//                }
//                else if (index == 7)
//                {
//                    sliderEQ7.FloatValue = value;
//                    lblEQValue7.StringValue = strValue;
//                }
//                else if (index == 8)
//                {
//                    sliderEQ8.FloatValue = value;
//                    lblEQValue8.StringValue = strValue;
//                }
//                else if (index == 9)
//                {
//                    sliderEQ9.FloatValue = value;
//                    lblEQValue9.StringValue = strValue;
//                }
//                else if (index == 10)
//                {
//                    sliderEQ10.FloatValue = value;
//                    lblEQValue10.StringValue = strValue;
//                }
//                else if (index == 11)
//                {
//                    sliderEQ11.FloatValue = value;
//                    lblEQValue11.StringValue = strValue;
//                }
//                else if (index == 12)
//                {
//                    sliderEQ12.FloatValue = value;
//                    lblEQValue12.StringValue = strValue;
//                }
//                else if (index == 13)
//                {
//                    sliderEQ13.FloatValue = value;
//                    lblEQValue13.StringValue = strValue;
//                }
//                else if (index == 14)
//                {
//                    sliderEQ14.FloatValue = value;
//                    lblEQValue14.StringValue = strValue;
//                }
//                else if (index == 15)
//                {
//                    sliderEQ15.FloatValue = value;
//                    lblEQValue15.StringValue = strValue;
//                }
//                else if (index == 16)
//                {
//                    sliderEQ16.FloatValue = value;
//                    lblEQValue16.StringValue = strValue;
//                }
//                else if (index == 17)
//                {
//                    sliderEQ17.FloatValue = value;
//                    lblEQValue17.StringValue = strValue;
//                }
//            });
//        }
//        
//        public void UpdatePresetList(IEnumerable<string> presets)
//        {
//        }
        
        //#endregion
    }
}


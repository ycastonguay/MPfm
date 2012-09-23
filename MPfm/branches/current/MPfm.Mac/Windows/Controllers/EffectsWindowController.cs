//
// EffectsWindowController.cs: Effects window controller.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MPfm.MVP;

namespace MPfm.Mac
{
    public partial class EffectsWindowController : MonoMac.AppKit.NSWindowController, IEffectsView
    {
        readonly IEffectsPresenter effectsPresenter = null;

        #region Constructors
        
        // Called when created from unmanaged code
        public EffectsWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public EffectsWindowController(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public EffectsWindowController(IEffectsPresenter effectsPresenter) 
            : base ("EffectsWindow")
        {
            this.effectsPresenter = effectsPresenter;
            Initialize();
            this.effectsPresenter.BindView(this);
        }
        
        // Shared initialization code
        void Initialize()
        {
        }
        
        #endregion
        
        //strongly typed window accessor
        public new EffectsWindow Window
        {
            get
            {
                return (EffectsWindow)base.Window;
            }
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            viewBackground.GradientColor1 = new CGColor(0.125f, 0.125f, 0.125f, 1.0f);
            viewBackground.GradientColor2 = new CGColor(0.35f, 0.35f, 0.35f, 1.0f);
            viewBackgroundPreset.GradientColor1 = new CGColor(0.85f, 0.85f, 0.85f, 1.0f);
            viewBackgroundPreset.GradientColor2 = new CGColor(0.6f, 0.6f, 0.6f, 1.0f);
            viewBackgroundPreset.HeaderGradientColor1 = new CGColor(0.925f, 0.925f, 0.925f, 1.0f);
            viewBackgroundPreset.HeaderGradientColor2 = new CGColor(0.65f, 0.65f, 0.65f, 1.0f);
            viewBackgroundPreset.IsHeaderVisible = true;
            viewBackgroundInformation.GradientColor1 = new CGColor(0.85f, 0.85f, 0.85f, 1.0f);
            viewBackgroundInformation.GradientColor2 = new CGColor(0.6f, 0.6f, 0.6f, 1.0f);
            viewBackgroundInformation.HeaderGradientColor1 = new CGColor(0.925f, 0.925f, 0.925f, 1.0f);
            viewBackgroundInformation.HeaderGradientColor2 = new CGColor(0.65f, 0.65f, 0.65f, 1.0f);
            viewBackgroundInformation.IsHeaderVisible = true;

            popupPreset.Font = NSFont.FromFontName("Junction", 11.5f);
            lblName.Font = NSFont.FromFontName("Junction", 11f);
            lblTitleInformation.Font = NSFont.FromFontName("TitilliumText25L-800wt", 13f);
            lblTitlePreset.Font = NSFont.FromFontName("TitilliumText25L-800wt", 13f);
            lblEQOn.Font = NSFont.FromFontName("Junction", 11f);
            lblScalePlus6.Font = NSFont.FromFontName("Junction", 12f);
            lblScale0.Font = NSFont.FromFontName("Junction", 12f);
            lblScaleMinus6.Font = NSFont.FromFontName("Junction", 12f);
            btnAutoLevel.Font = NSFont.FromFontName("Junction", 11f);
            btnDelete.Font = NSFont.FromFontName("Junction", 11f);
            btnSave.Font = NSFont.FromFontName("Junction", 11f);
            btnReset.Font = NSFont.FromFontName("Junction", 11f);
            txtName.Font = NSFont.FromFontName("Junction", 11f);

            btnAutoLevel.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "fam_shape_align_middle");
            btnDelete.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "fam_delete");
            btnSave.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "fam_tick");
            btnReset.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "fam_exclamation");

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
            effectsPresenter.LoadPreset(popupPreset.StringValue);
        }

        partial void actionEQOnChange(NSObject sender)
        {
            effectsPresenter.BypassEQ();
        }

        partial void actionNameChanged(NSObject sender)
        {
        }

        partial void actionSave(NSObject sender)
        {
            effectsPresenter.SavePreset(txtName.StringValue);
        }

        partial void actionDelete(NSObject sender)
        {
            effectsPresenter.DeletePreset(txtName.StringValue);
        }

        partial void actionAutoLevel(NSObject sender)
        {
            effectsPresenter.AutoLevel();
        }

        partial void actionReset(NSObject sender)
        {
            effectsPresenter.Reset();
        }

        partial void actionSlider0ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(0, sliderEQ0.FloatValue);
        }

        partial void actionSlider1ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(1, sliderEQ1.FloatValue);
        }

        partial void actionSlider2ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(2, sliderEQ2.FloatValue);
        }

        partial void actionSlider3ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(3, sliderEQ3.FloatValue);
        }

        partial void actionSlider4ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(4, sliderEQ4.FloatValue);
        }

        partial void actionSlider5ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(5, sliderEQ5.FloatValue);
        }

        partial void actionSlider6ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(6, sliderEQ6.FloatValue);
        }

        partial void actionSlider7ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(7, sliderEQ7.FloatValue);
        }

        partial void actionSlider8ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(8, sliderEQ8.FloatValue);
        }

        partial void actionSlider9ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(9, sliderEQ9.FloatValue);
        }

        partial void actionSlider10ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(10, sliderEQ10.FloatValue);
        }

        partial void actionSlider11ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(11, sliderEQ11.FloatValue);
        }

        partial void actionSlider12ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(12, sliderEQ12.FloatValue);
        }

        partial void actionSlider13ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(13, sliderEQ13.FloatValue);
        }

        partial void actionSlider14ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(14, sliderEQ14.FloatValue);
        }

        partial void actionSlider15ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(15, sliderEQ15.FloatValue);
        }

        partial void actionSlider16ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(16, sliderEQ16.FloatValue);
        }

        partial void actionSlider17ChangeValue(NSObject sender)
        {
            effectsPresenter.SetEQParam(17, sliderEQ17.FloatValue);
        }

        #region IEffectsView implementation
        
        public void UpdateFader(int index, float value)
        {
            string strValue = value.ToString("0.0") + " dB";
            if (index == 0)
            {
                sliderEQ0.FloatValue = value;
                lblEQValue0.StringValue = strValue;
            } 
            else if (index == 1)
            {
                sliderEQ1.FloatValue = value;
                lblEQValue1.StringValue = strValue;
            }
            else if (index == 2)
            {
                sliderEQ2.FloatValue = value;
                lblEQValue2.StringValue = strValue;
            }
            else if (index == 3)
            {
                sliderEQ3.FloatValue = value;
                lblEQValue3.StringValue = strValue;
            }
            else if (index == 4)
            {
                sliderEQ4.FloatValue = value;
                lblEQValue4.StringValue = strValue;
            }
            else if (index == 5)
            {
                sliderEQ5.FloatValue = value;
                lblEQValue5.StringValue = strValue;
            }
            else if (index == 6)
            {
                sliderEQ6.FloatValue = value;
                lblEQValue6.StringValue = strValue;
            }
            else if (index == 7)
            {
                sliderEQ7.FloatValue = value;
                lblEQValue7.StringValue = strValue;
            }
            else if (index == 8)
            {
                sliderEQ8.FloatValue = value;
                lblEQValue8.StringValue = strValue;
            }
            else if (index == 9)
            {
                sliderEQ9.FloatValue = value;
                lblEQValue9.StringValue = strValue;
            }
            else if (index == 10)
            {
                sliderEQ10.FloatValue = value;
                lblEQValue10.StringValue = strValue;
            }
            else if (index == 11)
            {
                sliderEQ11.FloatValue = value;
                lblEQValue11.StringValue = strValue;
            }
            else if (index == 12)
            {
                sliderEQ12.FloatValue = value;
                lblEQValue12.StringValue = strValue;
            }
            else if (index == 13)
            {
                sliderEQ13.FloatValue = value;
                lblEQValue13.StringValue = strValue;
            }
            else if (index == 14)
            {
                sliderEQ14.FloatValue = value;
                lblEQValue14.StringValue = strValue;
            }
            else if (index == 15)
            {
                sliderEQ15.FloatValue = value;
                lblEQValue15.StringValue = strValue;
            }
            else if (index == 16)
            {
                sliderEQ16.FloatValue = value;
                lblEQValue16.StringValue = strValue;
            }
            else if (index == 17)
            {
                sliderEQ17.FloatValue = value;
                lblEQValue17.StringValue = strValue;
            }
        }
        
        public void UpdatePresetList(IEnumerable<string> presets)
        {
        }
        
        #endregion
    }
}


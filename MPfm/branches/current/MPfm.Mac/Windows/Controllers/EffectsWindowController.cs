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
using MonoMac.Foundation;
using MonoMac.AppKit;
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

        #region IEffectsView implementation
        
        public void UpdateFader(int index, float value)
        {
        }
        
        public void UpdateFaders(IEnumerable<float> values)
        {
        }
        
        public void UpdatePresetList(IEnumerable<string> presets)
        {
        }
        
        #endregion
    }
}


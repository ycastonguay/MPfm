using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Library;
using MPfm.Player;

namespace TestControls
{
    public partial class frmMain : Form
    {
        //private Player m_player = null;
        private Library m_library = null;

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load player
            //m_player = new Player(FMOD.OUTPUTTYPE.DSOUND, "", true);
            //songGridView.Library = player.Library;

            // Load library
            string databaseFilePath = "";
            m_library = new Library(databaseFilePath);

            comboDisplayType.SelectedItem = "SongGridView";

            comboCustomFontName.Items.Add("Junction");
            comboCustomFontName.Items.Add("TitilliumText22L Lt");
            comboCustomFontName.Items.Add("BPmono");
            comboCustomFontName.Items.Add("CPmono");
            comboCustomFontName.Items.Add("LeagueGothic");
            comboCustomFontName.SelectedIndex = 0;

            // Get list of installed fonts
            InstalledFontCollection fonts = new InstalledFontCollection();
            for (int a = 0; a < fonts.Families.Length; a++)
            {
                comboStandardFontName.Items.Add(fonts.Families[a].Name);
            }
            comboStandardFontName.SelectedItem = "Tahoma";

            songGridView.ImportAudioFiles(m_library.SelectAudioFiles(FilterSoundFormat.MP3));

            // Set initial query

        }

        private void trackFontSize_OnTrackBarValueChanged()
        {
            lblFontSize.Text = "Font Size: " + trackFontSize.Value.ToString() + " pt";            
            songGridView.Font = new Font(songGridView.Font.FontFamily, trackFontSize.Value, Font.Style);
        }

        private void trackPadding_OnTrackBarValueChanged()
        {
            lblPadding.Text = "Padding: " + trackPadding.Value.ToString() + " px";
            songGridView.Padding = trackPadding.Value;
            songGridView.Refresh();
        }

        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            songGridView.DisplayDebugInformation = chkDebug.Checked;
            songGridView.Refresh();
        }

        private void txtSearchArtistName_TextChanged(object sender, EventArgs e)
        {
            //List<SongDTO> songs = ConvertDTO.ConvertSongs(DataAccess.SelectSongs(txtSearchArtistName.Text, string.Empty, string.Empty, songGridView.OrderByFieldName, songGridView.OrderByAscending));
            //songGridView.ImportSongs(songs);
        }

        private void lblDisplayDebugInformation_Click(object sender, EventArgs e)
        {
            chkDebug.Checked = !chkDebug.Checked;
        }

        private void comboDisplayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboDisplayType.SelectedItem == "Albums")
            {
                //libraryGridView1.DisplayType = MPfm.WindowsControls.LibraryGridViewDisplayType.Albums;
            }
            else if (comboDisplayType.SelectedItem == "Songs")
            {
                //libraryGridView1.DisplayType = MPfm.WindowsControls.LibraryGridViewDisplayType.Songs;
            }
        }

        private void comboCustomFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReassignFont();
        }

        private void comboFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReassignFont();
        }

        private void lblUseCustomFont_Click(object sender, EventArgs e)
        {
            radioUseCustomFont.Checked = true;
        }

        private void lblUseStandardFont_Click(object sender, EventArgs e)
        {
            radioUseStandardFont.Checked = true;
        }

        private void radioUseStandardFont_CheckedChanged(object sender, EventArgs e)
        {
            comboStandardFontName.Enabled = radioUseStandardFont.Checked;
            comboCustomFontName.Enabled = radioUseCustomFont.Checked;

            ReassignFont();
        }

        private void radioUseCustomFont_CheckedChanged(object sender, EventArgs e)
        {
            comboStandardFontName.Enabled = radioUseStandardFont.Checked;
            comboCustomFontName.Enabled = radioUseCustomFont.Checked;

            ReassignFont();
        }

        private void ReassignFont()
        {
            if (radioUseCustomFont.Checked)
            {
                songGridView.CustomFontName = comboCustomFontName.SelectedItem.ToString();
            }
            else
            {
                try
                {
                    songGridView.Font = new Font(comboStandardFontName.SelectedItem.ToString(), Font.Size, Font.Style);
                }
                catch (Exception ex)
                {
                    // Replace by default font
                    songGridView.Font = new Font("Tahoma", Font.Size, Font.Style);
                }
                songGridView.CustomFontName = string.Empty;
            }

            songGridView.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = songGridView.m_startLineNumber.ToString() + " - " + songGridView.m_numberOfLinesToDraw.ToString() + " - " + songGridView.m_workerUpdateAlbumArtPile.Count.ToString();
        }

    }
}

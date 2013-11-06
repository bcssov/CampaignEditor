using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BC_Campaign_Editor
{
    public partial class MainForm : Form
    {
        #region Constructor
        public MainForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Fields
        private const string ProgName = "BC Campaign Editor";
        private CampaignBridgeDetails cbd = null;
        private CampaignBridgeDetails cbd2 = null;
        private CampaignShipDetails csd = null;
        private CampaignShipDetails csd2 = null;
        #endregion

        #region Form Events
        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            cbd = new CampaignBridgeDetails();
            cbd2 = new CampaignBridgeDetails();
            csd = new CampaignShipDetails();
            csd2 = new CampaignShipDetails();
            SetDatabindings();
        }

        /// <summary>
        /// Handles the Click event of the button1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string s = GetOpenFileDialogPath();
                if (!String.IsNullOrEmpty(s))
                    SetShipPath(csd, s);
            }
            catch
            {
                ShowError("Please select a valid file.", ProgName);
            }
        }


        /// <summary>
        /// Handles the Click event of the button2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string s = GetOpenFileDialogPath();
                if (!String.IsNullOrEmpty(s))
                    SetBridgePath(cbd, s);
            }
            catch
            {
                ShowError("Please select a valid file.", ProgName);
            }
        }

        /// <summary>
        /// Handles the Click event of the button3 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!ValidProperties())
            {
                ShowError("Not all properties have been set.", ProgName);
                return;
            }
            string s = GetFolderBrowseDialogPath();
            if (!String.IsNullOrEmpty(s) && !CampaignModified(s))
            {
                try
                {
                    GameHandler gh = new GameHandler(s);
                    if (SetEditorVariables(gh) && SetEditorImports(gh) && SetScripts(gh) && OutputToDisk(gh))
                    {
                        ShowMessage("Scripts have been rewritten.", ProgName);
                    }
                    else
                    {
                        ShowError("There was an error while rewritting the scripts.", ProgName);
                    }
                    gh.Dispose();
                }
                catch
                {
                    ShowError("There was an error while rewritting the scripts.", ProgName);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the button4 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string s = GetOpenFileDialogPath();
                if (!String.IsNullOrEmpty(s))
                    SetShipPath(csd2, s);
            }
            catch
            {
                ShowError("Please select a valid file.", ProgName);
            }
        }

        /// <summary>
        /// Handles the Click event of the button5 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string s = GetOpenFileDialogPath();
                if (!String.IsNullOrEmpty(s))
                    SetBridgePath(cbd2, s);
            }
            catch
            {
                ShowError("Please select a valid file.", ProgName);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the databindings.
        /// </summary>
        private void SetDatabindings()
        {
            SetTextBoxDatabinding(textBox1, csd, "ShipScript");
            SetTextBoxDatabinding(textBox2, cbd, "BridgeScript");
            SetTextBoxDatabinding(textBox3, csd2, "ShipScript");
            SetTextBoxDatabinding(textBox4, cbd2, "BridgeScript");
        }

        /// <summary>
        /// Sets the text box databinding.
        /// </summary>
        /// <param name="txt">The textbox control.</param>
        /// <param name="details">The class which implements the abstract GameDetails class.</param>
        /// <param name="name">The property name.</param>
        private void SetTextBoxDatabinding(TextBox txt, GameDetails details, string name)
        {
            txt.DataBindings.Clear();
            Binding bnd = new Binding("Text", details, name, false, DataSourceUpdateMode.OnPropertyChanged);
            txt.DataBindings.Add(bnd);
        }

        /// <summary>
        /// Gets the open file dialog path.
        /// </summary>
        /// <returns>Selected file path or an empty string.</returns>
        private string GetOpenFileDialogPath()
        {
            openFileDialog1.FileName = String.Empty;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets the folder browse dialog path.
        /// </summary>
        /// <returns>Selected path or an empty string.</returns>
        private string GetFolderBrowseDialogPath()
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return folderBrowserDialog1.SelectedPath;
            }
            return String.Empty;
        }

        /// <summary>
        /// Evaluates if the campaign files have already been modified.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private bool CampaignModified(string path)
        {
            bool modified = false;
            using (CampaignShipDetails ship = new CampaignShipDetails())
            using (CampaignBridgeDetails bridge = new CampaignBridgeDetails())
            {
                if (bridge.IsModified(path) || ship.IsModified(path))
                {
                    modified = true;
                }
            }
            return modified;
        }

        /// <summary>
        /// Valids the properties.
        /// </summary>
        /// <returns>
        /// true if the properties are valid, false otherwise
        /// </returns>
        private bool ValidProperties()
        {
            bool isValid = true;
            List<IValidProperty> lst = new List<IValidProperty>();
            lst.Add(cbd);
            lst.Add(cbd2);
            lst.Add(csd);
            lst.Add(csd2);
            foreach (var item in lst)
            {
                if (!item.IsPropertyValid())
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }

        /// <summary>
        /// Sets the ship path.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="path">The path.</param>
        private void SetShipPath(CampaignShipDetails details, string path)
        {
            details.ShipScript = path;
        }

        /// <summary>
        /// Sets the bridge path.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="path">The path.</param>
        private void SetBridgePath(CampaignBridgeDetails details, string path)
        {
            details.BridgeScript = path;
        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        private void ShowError(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        private void ShowMessage(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Sets the editor variables.
        /// </summary>
        /// <param name="rewrite">The rewrite.</param>
        /// <returns></returns>
        private bool SetEditorVariables(IRewriteScripts rewrite)
        {
            if (rewrite.WriteVariables(csd2.SovereignShipReplacementIdentifier, csd2.ShipScript) &&
                rewrite.WriteVariables(csd.GalaxyShipReplacementIdentifier, csd.ShipScript) &&
                rewrite.WriteVariables(cbd2.BridgeSovereignReplacementIdentifier, cbd2.BridgeScript) &&
                rewrite.WriteVariables(cbd.BridgeGalaxyReplacementIdentifier, cbd.BridgeScript))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the editor imports.
        /// </summary>
        /// <param name="rewrite">The rewrite.</param>
        /// <returns></returns>
        private bool SetEditorImports(IRewriteScripts rewrite)
        {          
            if (rewrite.WriteImports(cbd.BridgeGalaxyReplacementIdentifier, cbd2.BridgeSovereignReplacementIdentifier, csd.GalaxyShipReplacementIdentifier, csd2.SovereignShipReplacementIdentifier))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the scripts.
        /// </summary>
        /// <param name="rewrite">The rewrite.</param>
        /// <returns></returns>
        private bool SetScripts(IRewriteScripts rewrite)
        {
            if (rewrite.WriteGalaxyReplacement(cbd.type, cbd.BridgeGalaxyReplacementIdentifier) && 
                rewrite.WriteGalaxyReplacement(csd.type, csd.GalaxyShipReplacementIdentifier) && 
                rewrite.WriteSovereignReplacement(cbd2.type, cbd2.BridgeSovereignReplacementIdentifier) &&
                rewrite.WriteSovereignReplacement(csd2.type, csd2.SovereignShipReplacementIdentifier))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Outputs to disk.
        /// </summary>
        /// <param name="rewrite">The rewrite.</param>
        /// <returns></returns>
        private bool OutputToDisk(IRewriteScripts rewrite)
        {
            if (rewrite.WriteToHardDisk())
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

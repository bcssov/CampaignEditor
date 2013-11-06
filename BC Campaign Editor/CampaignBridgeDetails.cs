using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace BC_Campaign_Editor
{
    internal class CampaignBridgeDetails : GameDetails, INotifyPropertyChanged, IValidProperty, IDisposable
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the script type.
        /// </summary>
        /// <value>The type.</value>
       internal override GameDetails.ScriptType type
        {
            get
            {
                return ScriptType.Bridge;
            }
        }

       /// <summary>
       /// Gets the bridge galaxy replacement identifier.
       /// </summary>
       /// <value>The bridge galaxy replacement identifier.</value>
        internal string BridgeGalaxyReplacementIdentifier
        {
            get
            {
                return "sBridgeGalaxyReplacement";
            }
        }

        /// <summary>
        /// Gets the bridge sovereign replacement identifier.
        /// </summary>
        /// <value>The bridge sovereign replacement identifier.</value>
        internal string BridgeSovereignReplacementIdentifier
        {
            get
            {
                return "sBridgeSovereignReplacement";
            }
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        /// <summary>
        /// Gets or sets the bridge script. Can throw FileNotFoundException if the file doesn't exist.
        /// </summary>
        /// <value>The bridge script.</value>
        public string BridgeScript
        {
            get
            {
                return base.EvaluateScript(base.CustomizableProperty, MethodType.Get, this.type);
            }
            set
            {
                base.CustomizableProperty = base.EvaluateScript(value, MethodType.Set, this.type);
                OnPropertyChanged("BridgeScript");
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when Bridge Script property changes.
        /// </summary>
        /// <param name="name">The name.</param>
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
                PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Determines whether the python scripts are modified.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the python scripts are modified; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsModified(string path)
        {
            return base.IsAlreadyModified(path, this.BridgeGalaxyReplacementIdentifier, this.BridgeSovereignReplacementIdentifier);
        }

        /// <summary>
        /// Determines whether property setting is a valid one.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the property is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPropertyValid()
        {
            return base.ValidateProperties(this.BridgeScript);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
        #endregion
    }
}

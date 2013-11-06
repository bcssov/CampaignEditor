using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace BC_Campaign_Editor
{
    internal class CampaignShipDetails : GameDetails, INotifyPropertyChanged, IValidProperty, IDisposable
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
                return ScriptType.Ship;
            }
        }

        /// <summary>
        /// Gets the galaxy ship replacement identifier.
        /// </summary>
        /// <value>The galaxy ship replacement identifier.</value>
        internal string GalaxyShipReplacementIdentifier
        {
            get
            {
                return "sGalaxyShipReplacement";
            }
        }

        /// <summary>
        /// Gets the sovereign ship replacement identifier.
        /// </summary>
        /// <value>The sovereign ship replacement identifier.</value>
        internal string SovereignShipReplacementIdentifier
        {
            get
            {
                return "sSovereignShipReplacement";
            }
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        /// <summary>
        /// Gets or sets the ship script, can throw a FileNotFoundException.
        /// </summary>
        /// <value>The ship script.</value>
        public string ShipScript
        {
            get
            {
                return base.EvaluateScript(base.CustomizableProperty, MethodType.Get, this.type);
            }
            set
            {
                base.CustomizableProperty = base.EvaluateScript(value, MethodType.Set, this.type);
                OnPropertyChanged("ShipScript");
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when Ship Script property changes.
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
            return base.IsAlreadyModified(path, this.GalaxyShipReplacementIdentifier, this.SovereignShipReplacementIdentifier);
        }

        /// <summary>
        /// Determines whether property setting is a valid one.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the property is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPropertyValid()
        {
            return base.ValidateProperties(this.ShipScript);
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

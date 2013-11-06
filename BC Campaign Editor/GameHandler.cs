using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BC_Campaign_Editor
{
    internal class GameHandler : GameDetails, IDisposable, IRewriteScripts
    {
        #region Fields
        private string ScriptDir = String.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the content of the game. It holds the content of the critical game files to be edited in memory.
        /// </summary>
        /// <value>The content of the game.</value>
        internal Dictionary<string, List<string>> GameContent { get; set; }

        /// <summary>
        /// Gets the script type.
        /// </summary>
        /// <value>The type.</value>
        internal override GameDetails.ScriptType type
        {
            get
            {
                return ScriptType.NotScript;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GameHandler"/> class. Can throw a ArgumentNullException or FileNotFound Exception.
        /// </summary>
        /// <param name="path">The path.</param>
        internal GameHandler(string path)
        {
            this.GameContent = new Dictionary<string, List<string>>();
            this.ScriptDir = path;
            LoadContent();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the content.
        /// </summary>
        private void LoadContent()
        {
            if (String.IsNullOrEmpty(ScriptDir) || !Directory.Exists(ScriptDir))
            {
                throw new ArgumentNullException("Game path is null");
            }
            List<string> files = base.GetCampaignFiles(ScriptDir);
            foreach (var item in files)
            {
                if (File.Exists(item))
                {
                    using (StreamReader sr = File.OpenText(item))
                    {
                        List<string> temp = new List<string>();
                        while (sr.Peek() != -1)
                        {
                            temp.Add(sr.ReadLine());
                        }
                        GameContent.Add(item, temp);
                    }
                }
                else
                {
                    throw new FileNotFoundException("Specified file doesn't exist.");
                }
            }
        }

        /// <summary>
        /// Writes the variables.
        /// </summary>
        /// <param name="varName">Name of the var.</param>
        /// <param name="varValue">The var value.</param>
        /// <returns></returns>
        public bool WriteVariables(string varName, string varValue)
        {
            return base.WriteEditorVariables(this, varName, varValue);
        }

        /// <summary>
        /// Writes the imports.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        /// <param name="id3">The id3.</param>
        /// <param name="id4">The id4.</param>
        /// <returns></returns>
        public bool WriteImports(string id1, string id2, string id3, string id4)
        {
            return base.WriteScriptImports(this, id1, id2, id3, id4);
        }

        /// <summary>
        /// Writes the galaxy replacement.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public bool WriteGalaxyReplacement(GameDetails.ScriptType type, string replace)
        {
            return base.WriteGalaxyReplacement(this, type, replace);
        }

        /// <summary>
        /// Writes the sovereign replacement.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public bool WriteSovereignReplacement(GameDetails.ScriptType type, string replace)
        {
            return base.WriteSovereignReplacement(this, type, replace);
        }

        /// <summary>
        /// Writes to hard disk.
        /// </summary>
        /// <returns></returns>
        public bool WriteToHardDisk()
        {
            return base.WriteToDisk(GameContent);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GameContent = null;
            GC.Collect();
        }
        #endregion

        #region Enums
        /// <summary>
        /// Search State
        /// </summary>
        internal enum SearchState
        {
            /// <summary>
            /// Inited state
            /// </summary>
            Searching,
            /// <summary>
            /// Found a match
            /// </summary>
            Found,
            /// <summary>
            /// Set counter
            /// </summary>
            SetCounter
        } 
        #endregion
    }
}

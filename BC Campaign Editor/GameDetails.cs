using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BC_Campaign_Editor
{
    internal abstract class GameDetails
    {
        #region Fields
        private const string NotAvailable = "N/A";
        #endregion

        #region Properties
        /// <summary>
        /// Gets the script type.
        /// </summary>
        /// <value>The type.</value>
        internal abstract ScriptType type
        {
            get;
        }

        /// <summary>
        /// Gets the python extension signature.
        /// </summary>
        /// <value>The python extension.</value>
        private string PythonExtension
        {
            get
            {
                return ".py";
            }
        }

        private string _customProp = NotAvailable;
        /// <summary>
        /// Gets or sets the customizable property. Returns default value N/A.
        /// </summary>
        /// <value>The customizable property.</value>
        protected string CustomizableProperty
        {
            get
            {
                return _customProp;
            }
            set
            {
                _customProp = value;
            }
        }

        /// <summary>
        /// Gets the galaxy filename signatures.
        /// </summary>
        /// <value>The galaxy filename signatures.</value>
        private List<string> GalaxyFilesSignatures
        {
            get
            {
                List<string> list = new List<string>();
                list.Add("E1M1");
                list.Add("E1M2");
                list.Add("E2M0");
                list.Add("E2M1");
                list.Add("E2M2");
                list.Add("E2M6");
                return list;
            }
        }

        /// <summary>
        /// Gets the sovereign filename signatures.
        /// </summary>
        /// <value>The sovereign filename signatures.</value>
        private List<string> SovereignFilesSignatures
        {
            get
            {
                List<string> list = new List<string>();
                list.Add("E3M1");
                list.Add("E3M2");
                list.Add("E3M4");
                list.Add("E3M5");
                list.Add("E4M4");
                list.Add("E4M5");
                list.Add("E4M6");
                list.Add("E5M2");
                list.Add("E5M4");
                list.Add("E6M1");
                list.Add("E6M2");
                list.Add("E6M3");
                list.Add("E6M4");
                list.Add("E6M5");
                list.Add("E7M1");
                list.Add("E7M2");
                list.Add("E7M3");
                list.Add("E7M6");
                list.Add("E8M1");
                list.Add("E8M2");
                return list;
            }
        }

        /// <summary>
        /// Gets the main campaign file.
        /// </summary>
        /// <value>The main campaign file.</value>
        private string MainCampaignFile
        {
            get
            {
                return "Maelstrom";
            }
        }

        /// <summary>
        /// Gets the import identifier.
        /// </summary>
        /// <value>The import identifier.</value>
        private string ImportIdentifier
        {
            get
            {
                return "import";
            }
        }

        /// <summary>
        /// Gets the ship script signatures.
        /// </summary>
        /// <value>The ship script signatures.</value>
        private List<string> ShipScriptSignatures
        {
            get
            {
                List<string> list = new List<string>();
                list.Add("GetShipStats");
                list.Add("LoadModel");
                list.Add("PreLoadModel");
                return list;
            }
        }

        /// <summary>
        /// Gets the bridge script signatures.
        /// </summary>
        /// <value>The bridge script signatures.</value>
        private List<string> BridgeScriptSignatures
        {
            get
            {
                List<string> list = new List<string>();
                list.Add("CreateBridgeModel");
                list.Add("LoadSounds");
                list.Add("UnloadSounds");
                return list;
            }
        }

        /// <summary>
        /// Gets the preload assets identifier.
        /// </summary>
        /// <value>The preload assets identifier.</value>
        private string PreloadAssetsIdentifier
        {
            get
            {
                return "PreLoadAssets(pMission):";
            }
        }

        /// <summary>
        /// Gets the preload assets definition.
        /// </summary>
        /// <value>The preload assets definition.</value>
        private string PreloadAssetsDefinition
        {
            get
            {
                return "loadspacehelper.PreloadShip({0}, 1)";
            }
        }

        /// <summary>
        /// Gets the create player ship identifier.
        /// </summary>
        /// <value>The create player ship identifier.</value>
        private string CreatePlayerShipIdentifier
        {
            get
            {
                return "MissionLib.CreatePlayerShip";
            }
        }

        /// <summary>
        /// Gets the create player bridge identifier.
        /// </summary>
        /// <value>The create player bridge identifier.</value>
        private string CreatePlayerBridgeIdentifier
        {
            get
            {
                return "LoadBridge.Load";
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Evaulates the path and throws FileNotFoundException if the path is incorrect.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="mType">Type of the MethodType.</param>
        /// <param name="sType">Type of the ScriptType.</param>
        private void EvaulatePath(string path, MethodType mType, ScriptType sType)
        {
            switch (mType)
            {
                case MethodType.Set:
                    if (String.IsNullOrEmpty(path) || !File.Exists(path))
                        throw new FileNotFoundException("File specified could not be found.");
                    if (!IsValidPythonScript(path, sType))
                        throw new FileNotFoundException("File specified could not be found.");
                    break;
                case MethodType.Get:
                    if (String.IsNullOrEmpty(path))
                        throw new FileNotFoundException("File specified could not be found.");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Determines whether the input script is a valid python script. Can throw InvalidDataException if the ScriptType is NotScript.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="sType">Type of the ScriptType</param>
        /// <returns>
        /// 	<c>true</c> if loaded file contains internal bridge script or ship script signatures; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidPythonScript(string path, ScriptType sType)
        {
            bool isValid = false;
            List<string> list = new List<string>();
            using (StreamReader sr = File.OpenText(path))
            {
                while (sr.Peek() != -1)
                {
                    list.Add(sr.ReadLine());
                }
            }

            if (list != null && list.Count > 0)
            {
                short i = 0;
                switch (sType)
                {
                    case ScriptType.Ship:
                        foreach (var sig in ShipScriptSignatures)
                        {
                            foreach (var item in list)
                            {
                                if (item.StartsWith("def") && item.Contains(sig))
                                {
                                    i += 1;
                                }
                            }
                        }
                        break;
                    case ScriptType.Bridge:
                        foreach (var sig in BridgeScriptSignatures)
                        {
                            foreach (var item in list)
                            {
                                if (item.StartsWith("def") && item.Contains(sig))
                                {
                                    i += 1;
                                }
                            }
                        }
                        break;
                    case ScriptType.NotScript:
                        throw new InvalidDataException("Invalid script type defined.");
                    default:
                        break;
                }
                if (i >= 3)
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        /// <summary>
        /// Evaluates the script. Can throw FileNoutFoundException if the specified path doesn't exist or InvalidDataException if the Script type is NotScript.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="mType">Type of the MethodType</param>
        /// <param name="sType">Type of the ScriptType</param>
        /// <returns></returns>
        protected string EvaluateScript(string path, MethodType mType, ScriptType sType)
        {
            EvaulatePath(path, mType, sType);
            switch (mType)
            {
                case MethodType.Set:
                    return Path.GetFileNameWithoutExtension(path);
                case MethodType.Get:
                default:
                    return path;
            }
        }

        /// <summary>
        /// Gets the matching directory.
        /// </summary>
        /// <param name="sig">The sig.</param>
        /// <returns></returns>
        private string GetMatchingDirectory(string sig)
        {
            string s = String.Empty;
            string episode = "Episode{0}";
            string fl = "E{0}";
            for (int i = 0; i < 9; i++)
            {
                string tempEp = String.Format(episode, i);
                string tempFl = String.Format(fl, i);
                if (sig.Contains(tempFl))
                {
                    s = tempEp;
                    break;
                }
            }
            return s;
        }

        /// <summary>
        /// Determines if the specified campaign files are already modified.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="evalGalaxy">The eval galaxy.</param>
        /// <param name="evalSov">The eval sov.</param>
        /// <returns>
        /// 	<c>true</c> if campaign files are already modified; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsAlreadyModified(string path, string evalGalaxy, string evalSov)
        {
            bool isModified = false;
            List<string> files = GetCampaignFiles(path);
            int toCheck = files.Count;
            int checkedFiles = 0;
            foreach (var item in files)
            {
                if (File.Exists(item))
                {
                    checkedFiles += 1;
                    string content = String.Empty;
                    using (StreamReader sr = File.OpenText(item))
                    {
                        content = sr.ReadToEnd();
                    }
                    if (content.Contains(evalGalaxy) || content.Contains(evalSov))
                    {
                        isModified = true;
                    }
                }
                if (isModified)
                {
                    break;
                }
            }
            if (toCheck != checkedFiles)
            {
                isModified = true;
            }
            return isModified;
        }

        /// <summary>
        /// Validates the properties.
        /// </summary>
        /// <param name="props">The props.</param>
        /// <returns>
        /// true if the properties are valid, otherwise false.
        /// </returns>
        protected bool ValidateProperties(params string[] props)
        {
            bool valid = true;
            foreach (var item in props)
            {
                if (item == NotAvailable)
                {
                    valid = false;
                }
            }
            return valid;
        }

        /// <summary>
        /// Gets the campaign files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        protected List<string> GetCampaignFiles(string path)
        {
            List<string> files = new List<string>();
            List<string> combined = new List<string>();
            combined.Add(MainCampaignFile);
            combined.AddRange(GalaxyFilesSignatures);
            combined.AddRange(SovereignFilesSignatures);
            foreach (var item in combined)
            {
                string filename = Path.Combine(path, GetMatchingDirectory(item), item, String.Format("{0}{1}", item, PythonExtension));
                if (item == MainCampaignFile)
                {
                    filename = Path.Combine(path, String.Format("{0}{1}", item, PythonExtension));
                }
                files.Add(filename);
            }
            return files;
        }

        /// <summary>
        /// Formats the python variables.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string FormatPythonVariables(string variable, string value)
        {
            return String.Format("{0} = '{1}'", variable, value);
        }

        /// <summary>
        /// Formats the python imports.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        /// <param name="id3">The id3.</param>
        /// <param name="id4">The id4.</param>
        /// <returns></returns>
        private string FormatPythonImports(string id1, string id2, string id3, string id4)
        {
            return String.Format("from Maelstrom.Maelstrom import {0}, {1}, {2}, {3}", id1, id2, id3, id4);
        }

        /// <summary>
        /// Formats the bridge replacement line. It can throw ArgumentOutOfRangeException if the python line can't be parsed.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="bridgeId">The bridge id.</param>
        /// <returns></returns>
        private string FormatBridge(string line, string bridgeId)
        {
            string newText = String.Empty;
            string intendation = FormatIntendation(line);
            if (!String.IsNullOrEmpty(intendation))
            {
                newText = String.Format("{0}{1}({2})", intendation, CreatePlayerBridgeIdentifier, bridgeId);
            }

            if (String.IsNullOrEmpty(newText))
            {
                throw new ArgumentOutOfRangeException("Parsing error, please verify parameters.");
            }

            return newText;
        }

        /// <summary>
        /// Formats the ship.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="shipId">The ship id.</param>
        /// <returns></returns>
        private string FormatShip(string line, string shipId)
        {
            string newText = String.Empty;
            int bracket = line.IndexOf("(") + 1;
            int comma = line.IndexOf(",");

            if (bracket <= 0 || comma <= 0)
            {
                throw new ArgumentOutOfRangeException("Parsing error, please verify parameters.");
            }

            string pre = line.Substring(0, bracket);
            string post = line.Substring(comma);
            newText = String.Format("{0}{1}{2}", pre, shipId, post);

            if (String.IsNullOrEmpty(newText))
            {
                throw new ArgumentOutOfRangeException("Parsing error, please verify parameters.");
            }

            return newText;
        }

        /// <summary>
        /// Formats the intendation.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private string FormatIntendation(string line)
        {
            string emptySpace = String.Empty;
            foreach (char c in line)
            {
                bool stop = false;
                SpacingType type = GetSpacingType(c);
                switch (type)
                {
                    case SpacingType.Whitespace:
                        emptySpace += " ";
                        break;
                    case SpacingType.Tab:
                        emptySpace += "\t";
                        break;
                    case SpacingType.Neither:
                    default:
                        stop = true;
                        break;
                }
                if (stop)
                {
                    break;
                }
            }
            return emptySpace;
        }

        /// <summary>
        /// Gets the type of the spacing.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        private SpacingType GetSpacingType(char c)
        {
            SpacingType type = SpacingType.Neither;
            if (c.ToString().Equals("\t"))
            {
                type = SpacingType.Tab;
            }
            else if (char.IsWhiteSpace(c))
            {
                type = SpacingType.Whitespace;
            }
            return type;
        }

        /// <summary>
        /// Write editor variables
        /// </summary>
        /// <param name="gh">The gh.</param>
        /// <param name="varName">Name of the var.</param>
        /// <param name="varValue">The var value.</param>
        /// <returns></returns>
        protected bool WriteEditorVariables(GameHandler gh, string varName, string varValue)
        {
            bool written = false;
            List<string> content = null;
            string name = String.Format("{0}{1}", MainCampaignFile, PythonExtension);
            foreach (var item in gh.GameContent)
            {
                if (item.Key.EndsWith(name))
                {
                    content = item.Value;
                    break;
                }
            }

            if (content != null && content.Count > 0)
            {
                GameHandler.SearchState ss = GameHandler.SearchState.Searching;
                int counter = 0;
                int set = -1;
                foreach (var item in content)
                {
                    switch (ss)
                    {
                        case GameHandler.SearchState.Searching:
                            counter += 1;
                            if (item.Contains(ImportIdentifier))
                            {
                                ss = GameHandler.SearchState.Found;
                            }
                            break;
                        case GameHandler.SearchState.Found:
                            counter += 1;
                            if (!item.Contains(ImportIdentifier))
                            {
                                ss = GameHandler.SearchState.SetCounter;
                            }
                            break;
                        case GameHandler.SearchState.SetCounter:
                            set = counter;
                            break;
                        default:
                            break;
                    }
                }

                if (set != -1)
                {
                    content.Insert(set, FormatPythonVariables(varName, varValue));
                    string key = String.Empty;
                    foreach (var item in gh.GameContent)
                    {
                        if (item.Key.Contains(MainCampaignFile))
                        {
                            key = item.Key;
                            break;
                        }
                    }
                    if (!String.IsNullOrEmpty(key))
                    {
                        gh.GameContent[key] = content;
                        written = true;
                    }
                }
            }

            return written;
        }

        /// <summary>
        /// Writes the script imports.
        /// </summary>
        /// <param name="gh">The gh.</param>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        /// <param name="id3">The id3.</param>
        /// <param name="id4">The id4.</param>
        /// <returns></returns>
        protected bool WriteScriptImports(GameHandler gh, string id1, string id2, string id3, string id4)
        {
            bool written = false;
            List<string> keys = new List<string>();
            string name = String.Format("{0}{1}", MainCampaignFile, PythonExtension);
            foreach (var item in gh.GameContent)
            {
                if (!item.Key.Contains(name))
                {
                    keys.Add(item.Key);
                }
            }
            int counter = 0;
            int total = keys.Count;
            foreach (var item in keys)
            {
                List<string> content = gh.GameContent[item];
                if (content != null && content.Count > 0)
                {
                    content.Insert(0, FormatPythonImports(id1, id2, id3, id4));
                    gh.GameContent[item] = content;
                    counter += 1;
                }
            }
            if (counter != total)
            {
                written = false;
            }
            else
            {
                written = total != 0;
            }
            return written;
        }

        /// <summary>
        /// Writes the galaxy replacement. Can throw InvalidDataException if the script type is NotScript.
        /// </summary>
        /// <param name="gh">The gh.</param>
        /// <param name="type">The type.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        protected bool WriteGalaxyReplacement(GameHandler gh, ScriptType type, string replace)
        {
            bool written = false;
            List<string> galaxyFiles = new List<string>();
            foreach (var item in GalaxyFilesSignatures)
            {
                string name = String.Format("{0}{1}", item, PythonExtension);
                galaxyFiles.Add(name);
            }

            switch (type)
            {
                case ScriptType.Ship:
                    SetPreloadLine(gh, replace, galaxyFiles);
                    SetShipReplacement(gh, replace, galaxyFiles);
                    written = true;
                    break;
                case ScriptType.Bridge:
                    SetBridgeReplacement(gh, replace, galaxyFiles);
                    written = true;
                    break;
                case ScriptType.NotScript:
                    throw new InvalidDataException("Invalid script type defined.");
                default:
                    break;
            }

            return written;
        }

        /// <summary>
        /// Writes the sovereign replacement.
        /// </summary>
        /// <param name="gh">The gh.</param>
        /// <param name="type">The type.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        protected bool WriteSovereignReplacement(GameHandler gh, ScriptType type, string replace)
        {
            bool written = false;
            List<string> sovFiles = new List<string>();
            foreach (var item in SovereignFilesSignatures)
            {
                string name = String.Format("{0}{1}", item, PythonExtension);
                sovFiles.Add(name);
            }

            switch (type)
            {
                case ScriptType.Ship:
                    SetPreloadLine(gh, replace, sovFiles);
                    SetShipReplacement(gh, replace, sovFiles);
                    written = true;
                    break;
                case ScriptType.Bridge:
                    SetBridgeReplacement(gh, replace, sovFiles);
                    written = true;
                    break;
                case ScriptType.NotScript:
                    break;
                default:
                    break;
            }

            return written;
        }

        /// <summary>
        /// Sets the bridge replacement. It can throw ArgumentNullException if the bridge replacement cannot be set.
        /// </summary>
        /// <param name="gh">The gh.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="files">The files.</param>
        private void SetBridgeReplacement(GameHandler gh, string replace, List<string> files)
        {
            List<string> keys = GetKeys(gh, files, "Error setting bridge replacement.");
            foreach (var item in keys)
            {
                List<string> content = gh.GameContent[item];
                if (content != null && content.Count > 0)
                {
                    for (int i = 0; i < content.Count; i++)
                    {
                        string line = content[i];
                        if (line.Contains(CreatePlayerBridgeIdentifier))
                        {
                            string newLine = FormatBridge(line, replace);
                            content[i] = newLine;
                        }
                    }
                    gh.GameContent[item] = content;
                }
                else
                {
                    throw new ArgumentNullException("Error setting bridge replacement.");
                }
            }
        }

        /// <summary>
        /// Sets the preload line. It can throw ArgumentNullException if the preload line cannot be set.
        /// </summary>
        /// <param name="gh">The gh.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="files">The files.</param>
        private void SetPreloadLine(GameHandler gh, string replace, List<string> files)
        {
            List<string> keys = GetKeys(gh, files, "Error setting preload lines.");
            foreach (var item in keys)
            {
                List<string> content = gh.GameContent[item];
                if (content != null && content.Count > 0)
                {
                    int id = -1;
                    for (int i = 0; i < content.Count; i++)
                    {
                        string line = content[i];
                        if (line.Contains(PreloadAssetsIdentifier))
                        {
                            id = i;
                            break;
                        }
                    }
                    if (id == -1)
                    {
                        throw new ArgumentNullException("Error setting preload lines.");
                    }
                    int counter = 1;
                    string comparison = content[id + counter];                    
                    while (String.IsNullOrWhiteSpace(comparison))
                    {
                        counter += 1;
                        comparison = content[id + counter];
                        if (!String.IsNullOrWhiteSpace(comparison))
                        {
                            break;
                        }
                    }
                    string emptySpace = FormatIntendation(comparison);
                    if (!String.IsNullOrEmpty(emptySpace))
                    {
                        content.Insert(id + 1, String.Format("{0}{1}", emptySpace, String.Format(PreloadAssetsDefinition, replace)));
                        gh.GameContent[item] = content;
                    }
                    else
                    {
                        throw new ArgumentNullException("Error setting preload lines.");
                    }

                }
                else
                {
                    throw new ArgumentNullException("Error setting preload lines.");
                }
            }
        }

        /// <summary>
        /// Sets the ship replacement. It can throw ArgumentNullException if the ship replacement cannot be set.
        /// </summary>
        /// <param name="gh">The gh.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="files">The files.</param>
        private void SetShipReplacement(GameHandler gh, string replace, List<string> files)
        {
            List<string> keys = GetKeys(gh, files, "Error setting ship replacement.");
            foreach (var item in keys)
            {
                List<string> content = gh.GameContent[item];
                if (content != null && content.Count > 0)
                {
                    for (int i = 0; i < content.Count; i++)
                    {
                        string line = content[i];
                        if (line.Contains(CreatePlayerShipIdentifier))
                        {
                            string newLine = FormatShip(line, replace);
                            content[i] = newLine;
                        }
                        gh.GameContent[item] = content;
                    }
                }
                else
                {
                    throw new ArgumentNullException("Error setting ship replacement.");
                }
            }
        }

        /// <summary>
        /// Gets the keys. Throws ArgumentNullException if the list is empty.
        /// </summary>
        /// <param name="gh">The gh.</param>
        /// <param name="files">The files.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        private List<string> GetKeys(GameHandler gh, List<string> files, string error)
        {
            List<string> keys = new List<string>();
            foreach (var item in gh.GameContent)
            {
                foreach (var file in files)
                {
                    if (item.Key.EndsWith(file))
                    {
                        keys.Add(item.Key);
                        break;
                    }
                }
            }

            if (keys == null || keys.Count == 0)
            {
                throw new ArgumentNullException(error);
            }

            return keys;
        }

        /// <summary>
        /// Writes to disk.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        protected bool WriteToDisk(Dictionary<string, List<string>> files)
        {
            bool saved = false;

            int count = 0;
            int total = files.Count;
            foreach (var item in files)
            {
                if (File.Exists(item.Key))
                {
                    File.SetAttributes(item.Key, FileAttributes.Normal);
                    File.Delete(item.Key);
                }
                using (StreamWriter sw = File.CreateText(item.Key))
                {
                    item.Value.ForEach(p => sw.WriteLine(p));
                    count += 1;
                }
            }

            if (count != total)
            {
                saved = false;
            }
            else
            {
                saved = total != 0;
            }

            return saved;
        }
        #endregion

        #region Enums
        /// <summary>
        /// Ship script set type.
        /// </summary>
        protected enum MethodType
        {
            /// <summary>
            /// Property set method used.
            /// </summary>
            Set,
            /// <summary>
            /// Property get method used.
            /// </summary>
            Get
        }

        /// <summary>
        /// Ship script type
        /// </summary>
        internal enum ScriptType
        {
            /// <summary>
            /// Ship type
            /// </summary>
            Ship,
            /// <summary>
            /// Bridge type
            /// </summary>
            Bridge,
            /// <summary>
            /// Not a script
            /// </summary>
            NotScript
        }

        /// <summary>
        /// Spacing type
        /// </summary>
        private enum SpacingType
        {
            /// <summary>
            /// Whitespace character
            /// </summary>
            Whitespace,
            /// <summary>
            /// Tab character
            /// </summary>
            Tab,
            /// <summary>
            /// Not a spacing type
            /// </summary>
            Neither
        }
        #endregion
    }
}

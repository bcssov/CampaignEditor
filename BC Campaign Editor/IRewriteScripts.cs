using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BC_Campaign_Editor
{
    interface IRewriteScripts
    {
        bool WriteVariables(string varName, string varValue);
        bool WriteImports(string id1, string id2, string id3, string id4);
        bool WriteGalaxyReplacement(GameDetails.ScriptType type, string replace);
        bool WriteSovereignReplacement(GameDetails.ScriptType type, string replace);
        bool WriteToHardDisk();
    }
}

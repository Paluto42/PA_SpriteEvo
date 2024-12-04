using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SpriteEvo
{
    public class SPE_ModConfig : Mod
    {
        public SPE_ModConfig(ModContentPack content) : base(content)
        {

        }
        public override string SettingsCategory()
        {
            return "SpriteEvo";
        }
    }
    public class SPE_ModSettings : ModSettings
    {
        public static bool debugOverride = false;
    }
}

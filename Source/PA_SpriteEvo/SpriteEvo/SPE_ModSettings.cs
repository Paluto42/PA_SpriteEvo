using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public class SPE_ModSettings : ModSettings
    {
        //测试用
        public static bool debugOverride = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref debugOverride, "debugOverride", defaultValue: false);
        }
    }
    public class SPE_ModConfig : Mod
    {
        public SPE_ModConfig(ModContentPack content) : base(content)
        {
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new();
            list.Begin(inRect);
            list.CheckboxLabeled("SPE_DebugLog".Translate(), ref SPE_ModSettings.debugOverride, "启用完整的Debug Log");
            list.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "SpriteEvo";
        }
    }
}

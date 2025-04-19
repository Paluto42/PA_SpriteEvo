using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PA_SpriteEvo.FacialAnimation
{
    [StaticConstructorOnStartup]
    internal class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony Instance = new Harmony("paluto22.SpriteEvo.FacialAnimation.patch");
            Instance.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("PA_SpriteEvo.FacialAnimation Loaded");
        }
    }
}

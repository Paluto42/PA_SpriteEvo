using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    internal static class AssetManager
    {
        public static Shader Spine_Skeleton;

        public static Material SkeletonGraphic;

        public static Dictionary<string, Shader> SpineShaderDatabase = new();

        public static Dictionary<string, GameObject> ObjectDatabase = new();

        public static Dictionary<Thing, GameObject> ThingObjectDatabase = new();

        public static Dictionary<string, SpineAssetPack> spine38_Database = new();

        public static Dictionary<string, SpineAssetPack> spine41_Database = new();

    }
}

using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    public static class AssetManager
    {
        public static Shader Spine_Skeleton;
        public static Shader Spine_Skeleton_Straight;

        public static Material SkeletonGraphic;
        public static Material SkeletonGraphic_Straight;

        public static Dictionary<string, Shader> SpineShaderDatabase = new();

        public static Dictionary<string, AssetLoader> spine38_Database = new();
        public static Dictionary<string, AssetLoader> spine41_Database = new();
        public static Dictionary<string, AssetLoader> spine42_Database = new();
    }
}

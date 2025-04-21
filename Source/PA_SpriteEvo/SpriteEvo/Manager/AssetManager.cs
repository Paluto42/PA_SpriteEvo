using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    public static class AssetManager
    {
        public static Shader Spine_Skeleton => AssetLoadService.Spine_Skeleton;
        public static Material SkeletonGraphic => AssetLoadService.SkeletonGraphicDefault;
        public static Material SkeletonGraphic_Straight => AssetLoadService.SkeletonGraphicDefaul_Straight;

        public static Dictionary<string, Shader> SpineShaderDatabase = new();

        public static Dictionary<string, AssetLoader> spine38_Database = new();
        public static Dictionary<string, AssetLoader> spine41_Database = new();
        public static Dictionary<string, AssetLoader> spine42_Database = new();
    }
}

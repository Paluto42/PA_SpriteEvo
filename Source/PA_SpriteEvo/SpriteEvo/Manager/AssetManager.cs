using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    public static class AssetManager
    {
        public static Shader Spine_Skeleton => AssetLoadManager.Spine_Skeleton;
        
        public static Material SkeletonGraphic => AssetLoadManager.SkeletonGraphicDefault;
        public static Material SkeletonGraphic_Straight => AssetLoadManager.SkeletonGraphicDefaul_Straight;


        public static Dictionary<string, Shader> SpineShaderDatabase = new();

        public static Dictionary<string, SkeletonLoader> spine38_Database = new();

        public static Dictionary<string, SkeletonLoader> spine41_Database = new();

        public static Dictionary<string, SkeletonLoader> spine42_Database = new();

        //May be Null.
        public static T TryGetAsset<T>(this SpineAssetDef def) where T : SkeletonLoader
        {
            if (def == null) return null;
            if (def.asset.version == "3.8")
            {
                return spine38_Database.TryGetValue(def.defName) as T;
            }
            else if (def.asset.version == "4.1")
            {
                return spine41_Database.TryGetValue(def.defName) as T;
            }
            return null;
        }

    }
}

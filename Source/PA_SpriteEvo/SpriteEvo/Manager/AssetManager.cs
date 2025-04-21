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

        //May be Null.
        public static T TryGetAsset<T>(this SpineAssetDef def) where T : AssetLoader
        {
            if (def == null) return null;
            if (def.asset.version == "3.8"){
                return spine38_Database.TryGetValue(def.defName) as T;
            }
            else if (def.asset.version == "4.1"){
                return spine41_Database.TryGetValue(def.defName) as T;
            }
            else if (def.asset.version == "4.2"){
                return spine42_Database.TryGetValue(def.defName) as T;
            }
            return null;
        }

    }
}

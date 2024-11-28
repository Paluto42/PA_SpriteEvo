using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    [StaticConstructorOnStartup]
    public static class AssetManager
    {
        public static Shader Spine_Skeleton;
                
        public static Material SkeletonGraphic;

        public static Dictionary<string, Shader> SpineShaderDatabase = new();

        //设置为DontDestroyOnLoad的GameObject才能在这里引用。
        public static Dictionary<object, GameObject> ObjectDatabase = new();

        //public static Dictionary<Thing, GameObject> ThingObjectDatabase = new();

        public static Dictionary<string, SkeletonLoader> spine38_Database = new();

        public static Dictionary<string, SkeletonLoader> spine41_Database = new();

        //May be Null.
        public static SkeletonLoader TryGetSpineAsset(this SpineAssetDef def) 
        {
            if (def == null) return null;
            if (def.asset.version == "3.8")
            {
                return spine38_Database.TryGetValue(def.defName);
            }
            else if (def.asset.version == "4.1")
            {
                return spine41_Database.TryGetValue(def.defName);
            }
            return null;
        }
        public static SpineMatAsset FindSpineMatAsset(this SpineAssetDef def)
        {
            SkeletonLoader loader = TryGetSpineAsset(def);
            if (loader is SpineMatAsset matpack) 
                return matpack;
            return null;
        }
        public static SpineTexAsset FindSpineTexAsset(this SpineAssetDef def)
        {
            SkeletonLoader pack = TryGetSpineAsset(def);
            if (pack is SpineTexAsset texpack)
                return texpack;
            return null;
        }
    }
}

using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>
    ///对Spine导出文件提供封装实例,保存已创建Material实例的Spine导出文件
    ///<para>一般情况下该Material使用的Shader为Spine/Shader</para>
    ///<para>对于PMA模式问题，请确认在导出前就在Unity中为Mat设置了Straight Alpha Texture选项</para>
    ///<para>了解详细信息,请查看<a cref="SkeletonLoader"></a>类</para>
    ///</summary>
    public class Asset_Mat : SkeletonLoader
    {
        public Material[] materials;
        public bool useStraightAlpha;

        public Asset_Mat(SpineAssetDef def, TextAsset atlas, TextAsset skeleton, Material[] mats, bool usePMA = false) : base(def, atlas, skeleton) 
        {
            this.materials = mats;
            this.useStraightAlpha = usePMA;
        }
        public override Spine38.Unity.SkeletonDataAsset SkeletonDataAsset38()
        {
            if (this.def.asset.version != "3.8") { return null; }
            if (this.atlasInput == null)
            {
                AtlasException();
                return null;
            }
            if (this.skeletonInput == null)
            {
                SkeletonException();
                return null;
            }
            if (this.materials.NullOrEmpty())
            {
                Log.Error(this.def.defName + " SpineAssetPack materials为空");
                return null;
            }
            Spine38.Unity.SpineAtlasAsset atlas;
            Spine38.Unity.SkeletonDataAsset skeleton;
            atlas = Spine38.Unity.SpineAtlasAsset.CreateRuntimeInstance(this.atlasInput, this.materials, initialize: true);
            skeleton = Spine38.Unity.SkeletonDataAsset.CreateRuntimeInstance(this.skeletonInput, atlas, initialize: true);
            return skeleton;
        }

        public override Spine41.Unity.SkeletonDataAsset SkeletonDataAsset41()
        {
            if (this.def.asset.version != "4.1") { return null; }
            if (this.atlasInput == null)
            {
                AtlasException();
                return null;
            }
            if (this.skeletonInput == null)
            {
                SkeletonException();
                return null;
            }
            if (this.materials.NullOrEmpty())
            {
                Log.Error(this.def.defName + " SpineAssetPack materials为空");
                return null;
            }
            Spine41.Unity.SpineAtlasAsset atlas;
            Spine41.Unity.SkeletonDataAsset skeleton;
            atlas = Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(this.atlasInput, this.materials, initialize: true);
            skeleton = Spine41.Unity.SkeletonDataAsset.CreateRuntimeInstance(this.skeletonInput, atlas, initialize: true);
            return skeleton;
        }
    }
}

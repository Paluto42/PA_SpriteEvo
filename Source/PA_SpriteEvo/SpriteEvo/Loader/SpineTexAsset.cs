using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>
    ///对Spine导出文件提供封装实例,保存未创建Material实例的Spine导出文件
    ///<para>需要使用Spine/Shader创建Material实例后才可初始化SkeletonDataAsset</para>
    ///<para>对于PMA模式问题，请使用本框架提供的预设启用PMA的额外Spine/Skeleton Shader</para>
    ///<para>了解详细信息,请查看<a cref="SkeletonLoader"></a>类</para>
    ///</summary>
    public class SpineTexAsset : SkeletonLoader
    {
        public Texture2D[] textures;
        public Shader shader;
        public bool useStraightAlpha;

        public SpineTexAsset(SpineAssetDef def, TextAsset atlas, TextAsset skeleton, Texture2D[] texs, Shader shader, bool usePMA = false) : base(def, atlas, skeleton)
        {
            this.textures = texs;
            this.shader = shader;
            this.useStraightAlpha = usePMA;
        }

        public override Spine38.Unity.SkeletonDataAsset Create_SkeletonDataAsset38()
        {
            if (this.def.asset.version != "3.8") { return null; }
            if (this.atlasInput == null) 
            {
                AtlasErrorMessage();
                return null;
            }
            if (this.skeletonInput == null)
            {
                SkeletonErrorMessage();
                return null;
            }
            if (this.textures.NullOrEmpty())
            {
                Log.Error(def.defName + " SpineAssetPack Missing Textures");
                return null;
            }
            Spine38.Unity.SpineAtlasAsset atlas;
            Spine38.Unity.SkeletonDataAsset skeleton;
            //Is_StraightAlphaTexture = pack.useStraightAlpha;
            atlas = Spine38.Unity.SpineAtlasAsset.CreateRuntimeInstance(this.atlasInput, this.textures, this.shader, initialize: true);
            skeleton = Spine38.Unity.SkeletonDataAsset.CreateRuntimeInstance(this.skeletonInput, atlas, initialize: true);
            //Is_StraightAlphaTexture = false;
            return skeleton;
        }

        public override Spine41.Unity.SkeletonDataAsset Create_SkeletonDataAsset41()
        {
            if (this.def.asset.version != "4.1") { return null; }
            if (this.atlasInput == null)
            {
                AtlasErrorMessage();
                return null;
            }
            if (this.skeletonInput == null)
            {
                SkeletonErrorMessage();
                return null;
            }
            if (this.textures.NullOrEmpty())
            {
                Log.Error(this.def.defName + " SpineAssetPack textures为空");
                return null;
            }
            Spine41.Unity.SpineAtlasAsset atlas;
            Spine41.Unity.SkeletonDataAsset skeleton;
            //Is_StraightAlphaTexture = pack.useStraightAlpha;
            atlas = Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(this.atlasInput, this.textures, this.shader, initialize: true);
            skeleton = Spine41.Unity.SkeletonDataAsset.CreateRuntimeInstance(this.skeletonInput, atlas, initialize: true);
            //Is_StraightAlphaTexture = false;
            return skeleton;
        }
    }
}

using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>对Spine导出文件提供封装实例</summary>
    public class SpineAssetPack : SkeletonLoader
    {
        public bool useMaterial;

        public bool useStraightAlpha;

        public TextAsset atlasInput;

        public TextAsset skeletonInput;

        public Material[] materials;

        public Texture2D[] textures;

        public Shader shader;

        public SpinePackDef def;

        //JSON格式导出文件
        public SpineAssetPack(TextAsset atlas, TextAsset skeleton, Texture2D[] texs, Shader shader, SpinePackDef def, bool useAlpha = false)
        {
            this.atlasInput = atlas;
            this.skeletonInput = skeleton;
            this.textures = texs;
            this.shader = shader;
            this.def = def;
            this.useMaterial = false;
            this.useStraightAlpha = useAlpha;
        }
        //二进制格式导出文件
        public SpineAssetPack(TextAsset atlas, TextAsset skeleton, Material[] mats, SpinePackDef def, bool useMat = true, bool useAlpha = false) 
        {
            this.atlasInput = atlas;
            this.skeletonInput = skeleton;
            this.materials = mats;
            this.def = def;
            this.useMaterial = useMat;
            this.useStraightAlpha = useAlpha;
        }
        public override Spine38.Unity.SkeletonDataAsset Create_SkeletonDataAsset38()
        {
            if (this.def.props.version != "3.8") { return null; }
            if (this.atlasInput == null || this.skeletonInput == null){
                Log.Error(def.defName + " SpineAssetPack 不存在atlas/skeletonData");
                return null;
            }
            Spine38.Unity.SpineAtlasAsset atlas;
            Spine38.Unity.SkeletonDataAsset skeleton;
            if (this.useMaterial)
            {
                if (materials.NullOrEmpty()){
                    Log.Error(def.defName + " SpineAssetPack materials为空");
                    return null;
                }
                atlas = Spine38.Unity.SpineAtlasAsset.CreateRuntimeInstance(this.atlasInput, this.materials, initialize: true);
                skeleton = Spine38.Unity.SkeletonDataAsset.CreateRuntimeInstance(this.skeletonInput, atlas, initialize: true);
            }
            else
            {
                if (textures.NullOrEmpty()){
                    Log.Error(def.defName + " SpineAssetPack textures为空");
                    return null;
                }
                //Is_StraightAlphaTexture = pack.useStraightAlpha;
                atlas = Spine38.Unity.SpineAtlasAsset.CreateRuntimeInstance(this.atlasInput, this.textures, this.shader, initialize: true);
                skeleton = Spine38.Unity.SkeletonDataAsset.CreateRuntimeInstance(this.skeletonInput, atlas, initialize: true);
                //Is_StraightAlphaTexture = false;
            }
            return skeleton;
        }

        public override Spine41.Unity.SkeletonDataAsset Create_SkeletonDataAsset41()
        {
            if (this.def.props.version != "4.1") { return null; }
            if (this.atlasInput == null || this.skeletonInput == null){
                Log.Error(def.defName + " SpineAssetPack 不存在atlas/skeletonData");
                return null;
            }
            Spine41.Unity.SpineAtlasAsset atlas;
            Spine41.Unity.SkeletonDataAsset skeleton;
            if (this.useMaterial)
            {
                if (materials.NullOrEmpty()){
                    Log.Error(def.defName + " SpineAssetPack materials为空");
                    return null;
                }
                atlas = Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(this.atlasInput, this.materials, initialize: true);
                skeleton = Spine41.Unity.SkeletonDataAsset.CreateRuntimeInstance(this.skeletonInput, atlas, initialize: true);
            }
            else
            {
                if (textures.NullOrEmpty()){
                    Log.Error(def.defName + " SpineAssetPack textures为空");
                    return null;
                }
                //Is_StraightAlphaTexture = pack.useStraightAlpha;
                atlas = Spine41.Unity.SpineAtlasAsset.CreateRuntimeInstance(this.atlasInput, this.textures, this.shader, initialize: true);
                skeleton = Spine41.Unity.SkeletonDataAsset.CreateRuntimeInstance(this.skeletonInput, atlas, initialize: true);
                //Is_StraightAlphaTexture = false;
            }
            return skeleton;
        }
    }
}

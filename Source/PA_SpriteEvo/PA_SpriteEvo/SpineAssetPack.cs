using UnityEngine;

namespace PA_SpriteEvo
{
    ///<summary>对已导出的Spine数据集合提供实例封装</summary>
    public class SpineAssetPack
    {
        public bool useMaterial;

        public bool useStraightAlpha;

        public TextAsset atlasData;

        public TextAsset skeletonData;

        public Material[] materials;

        public Texture2D[] textures;

        public Shader shader;

        public SpinePackDef def;

        public SpineAssetPack(TextAsset atlas, TextAsset skeleton, Texture2D[] texs, Shader shader, SpinePackDef def, bool useMat = false, bool useAlpha = false)
        {
            this.atlasData = atlas;
            this.skeletonData = skeleton;
            this.textures = texs;
            this.shader = shader;
            this.def = def;
            this.useMaterial = useMat;
            this.useStraightAlpha = useAlpha;
        }
        public SpineAssetPack(TextAsset atlas, TextAsset skeleton, Material[] mats, SpinePackDef def, bool useMat = true, bool useAlpha = false) 
        {
            this.atlasData = atlas;
            this.skeletonData = skeleton;
            this.materials = mats;
            this.def = def;
            this.useMaterial = useMat;
            this.useStraightAlpha = useAlpha;
        }
    }
}

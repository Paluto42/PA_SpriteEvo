using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public enum SkelFormat
    {
        SkeletonBinary,
        SkeletonJSON
    }
    //相当于SkeletonAnimationRequest结构体保存一套默认初始化信息
    public class SpineAsset 
    {
        public bool StraightAlphaInput = false;
        public string version;
        public string assetBundle;
        public string folderPath;
        public string seriesname;
        public string atlas;
        public string skeleton;
        public string shader = "Spine-Skeleton.shader";
        public List<string> textures = new();
        public List<string> materials = new();
    }
    public class SpineAssetDef : Def
    {
        public SkelFormat skelFormat = SkelFormat.SkeletonBinary;
        public SpineAsset asset = new();
        //public SpineProperty props = new();
    }
}

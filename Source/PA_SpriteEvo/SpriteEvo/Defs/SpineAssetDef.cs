using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    //选择加载的文件格式
    public enum SkelFormat
    {
        SkeletonBinary,
        SkeletonJSON
    }
    //文件属性
    public class SpineAsset 
    {
        public bool StraightAlphaInput = false;//是否关闭PMA
        public string version;//Spine版本号
        public string filePath;//文件相对路径
        public string seriesname;//自动填充用的系列名
        public string atlas;//文件名
        public string skeleton;//文件名
        public string shader = "Spine-Skeleton.shader";//一般默认不用管
        public List<string> textures = new();//贴图列表
        public List<string> materials = new();//材质列表
    }
    public class SpineAssetDef : Def
    {
        public SkelFormat skelFormat = SkelFormat.SkeletonBinary;
        public SpineAsset asset = new();
    }
}

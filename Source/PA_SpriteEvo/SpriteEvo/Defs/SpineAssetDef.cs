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
    public class SpineProperty
    {
        public bool OnIMGUI = false;
        public Color color = Color.white;
        public Vector3 offset = new(0f, 0f, 0f);
        public Vector3 uioffset = new(0f, 0f, 0f);
        public Vector3 rotation = Vector3.zero;
        public Vector2 scale = new(1f, 1f);
        public string skin = "default";
        public string idleAnimationName = "Idle";
        public float timeScale = 1f;
    }
    public class SpineAssetDef : Def
    {
        public SkelFormat skelFormat = SkelFormat.SkeletonBinary;
        public SpineAsset asset = new();
        public SpineProperty props = new();
    }
}

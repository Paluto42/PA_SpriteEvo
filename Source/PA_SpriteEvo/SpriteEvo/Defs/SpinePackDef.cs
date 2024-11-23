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
    public class Properties_SpinePack
    {
        public string version;

        public string atlas;

        public string skeleton;

        public string shader = "Spine-Skeleton.shader";

        public List<string> textures = new();

        public List<string> materials = new();

        public bool StraightAlphaInput = false;

        public string skin = "default";

        public string idleAnimationName = "Idle";

        public string interactAnimationName = "Interact";

        public string specialAnimationName = "Special";
    }
    public class SpinePackDef : Def
    {
        public SkelFormat skelFormat = SkelFormat.SkeletonBinary;

        public string assetBundle;

        public string folderPath;

        public string seriesname;

        public Vector3 offset = new(0f, 0f, 0f);

        public Vector3 uioffset = new(0f, 0f, 0f);

        public Vector2 scale = new(1f, 1f);

        public Vector3 rotation = Vector3.zero;

        public Properties_SpinePack props = new();
    }
}

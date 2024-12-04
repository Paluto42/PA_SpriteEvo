using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    //动画属性
    public class SpineProperty
    {
        public bool OnIMGUI = false;
        public Vector3 uioffset = new(0f, 0f, 0f);
        public Vector2 uiDrawSize = new(1024, 1024);

        public Color color = Color.white;
        public List<SlotSettings> slotSettings = new();
        public Vector3 offset = new(0f, 0f, 0f);
        public Vector3 position = new(0f, 0f, 0f);
        public Vector3 rotation = Vector3.zero;
        public Vector2 scale = new(1f, 1f);
        public string skin = "default";
        public string idleAnimationName = "Idle";
        public float timeScale = 1f;
    }
    /// <summary>
    /// 使用已加载的SpineAsset创建不同的Animation实例, 可填入attachment进行合并，仅支持填入Spine4.1 Skeleton JSON格式Asset
    /// </summary>
    public class AnimationDef : Def
    {
        public string version;
        public SpineAssetDef mainAsset;
        public List<SpineAssetDef> attachments = new();
        public SpineProperty props = new();
        public AnimationScriptDef script;
    }
}

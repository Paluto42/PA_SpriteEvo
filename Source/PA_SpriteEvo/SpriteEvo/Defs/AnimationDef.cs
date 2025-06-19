using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>用于确定一个slot的颜色</summary>
    //[Serializable]
    public class SlotSetting
    {
        public string slot = string.Empty;
        public Color32 color = Color.white;
    }
    //[Serializable]
    public class CustomMixSetting
    {
        public string fromAnimation = string.Empty;
        public string toAnimation = string.Empty;
        public float duration = 0;
    }
    //动画属性,相当于一个Request结构体保存一套默认初始化信息
    public class SpineProperty
    {
        public int renderQueue = 3200;

        //Canvas参数
        public bool OnIMGUI = false;//是否产生贴图用于UI渲染
        public Vector3 uioffset = new(0f, 0f, -100f);//调整贴图偏移(摄像机)
        public Vector2 uiDrawSize = new(1024, 1024);//产生的贴图大小

        public Vector3 offset = Vector3.zero;//暂时没用
        public Vector3 position = Vector3.zero;//游戏内坐标位置 
        public Vector3 rotation = Vector3.zero;//旋转量
        public Vector2 scale = new(1f, 1f);//尺寸

        public Color? skeletonColor;//骨架整体颜色,可以用(255,255,255,A)的方式改变整体透明度
        public List<SlotSetting> slotSettings = new();//每个槽位的具体属性

        public string skin = "default";//默认皮肤
        public string idleAnimation = "Idle";//默认播放的动画
        public float timeScale = 1f;//动画播放的速度 0停止
    }

    /// <summary>使用已加载的SpineAsset创建不同的Animation实例</summary>
    public class AnimationDef : Def
    {
        public string version;
        public SpineAssetDef mainAsset;
        public List<ScriptProperties> scripts = new();
        public List<SpineAssetDef> attachments = new();
        public SpineProperty props = new();
    }
}

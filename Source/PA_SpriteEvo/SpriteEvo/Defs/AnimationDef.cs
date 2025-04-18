using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>用于确定一个slot的颜色</summary>
    [Serializable]
    public class SlotSettings
    {
        public string slot = string.Empty;
        public Color32 color = Color.white;
    }
    //动画属性,相当于一个Request结构体保存一套默认初始化信息
    public class SpineProperty
    {
        [Obsolete]
        public bool OnIMGUI = false;//是否产生贴图用于UI渲染
        public bool OnUGUI = false;

        //public bool uiAutoAlign = false; //是否让摄像机自己自动对齐
        //public string alignBone; //如果自动对齐 选择一个Bone节点的名称

        public Vector3 uioffset = new(0f, 0f, -100f);//调整贴图偏移(摄像机)
        //public Vector3 uiPosition = new(0f, 0f, 0f); //调整贴图位置(摄像机)
        public Vector2 uiDrawSize = new(1024, 1024);//产生的贴图大小

        public Color color = Color.white;//骨架整体颜色,可以用(255,255,255,A)的方式改变整体透明度
        public List<SlotSettings> slotSettings = new();//每个槽位的具体属性

        public Vector3 offset = new(0f, 0f, 0f);//暂时没用
        public Vector3 position = new(0f, 0f, 0f);//游戏内坐标位置
        public Vector3 rotation = Vector3.zero;//旋转量
        public Vector2 scale = new(1f, 1f);//尺寸

        public string skin = "default";//默认皮肤
        public string idleAnimation = "Idle";//默认播放的动画
        public float timeScale = 1f;//动画播放的速度 0停止
    }
    /// <summary>使用已加载的SpineAsset创建不同的Animation实例, 可填入attachment进行合并，仅支持填入Spine4.1 Skeleton JSON格式Asset</summary>
    public class AnimationDef : Def
    {
        public string version;
        public List<ScriptProperties> scripts = new();
        public SpineAssetDef mainAsset;
        public List<SpineAssetDef> attachments = new();
        public SpineProperty props = new();
    }
}

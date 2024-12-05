using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteEvo
{  
    /// <summary>用于生成初始化Spine动画提供的参数集
    /// <para>必须调用<a cref="GenAnimation.GetSkeletonParams(AnimationDef, bool)"></a>进行值的初始化</para>
    /// </summary>
    public struct AnimationParams
    {
        public string name;
        public Color32 color;
        public List<SlotSettings> slotSettings;
        public Vector3 offset;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string skin;
        public string defaultAnimation;
        public bool PremultipliedAlpha;
        public bool loop;
        public float timeScale;
    }

}

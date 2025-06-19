using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    /// <summary>用于生成初始化Spine动画提供的参数集
    /// <para>必须调用<a cref="SkeletonAnimationUtility.GetSkeletonParams(AnimationDef, bool)"></a>进行值的初始化</para>
    /// </summary>
    public struct AnimationParams
    {
        public Color32? skeletonColor;
        public List<SlotSetting> slotSettings;
        public Vector3 offset;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string skin;
        public string defaultAnimation;
        public bool loop;
        public float timeScale;
    }
    public enum MixBlendInternal
    {
        Setup,
        First,
        Replace,
        Add
    }
    public static class Utilities
    {
        /// <summary>从AnimationDef中读取动画属性作为参数结构体返回</summary>
        public static AnimationParams GetSkeletonParams(this AnimationDef def, bool loop = true)
        {
            AnimationParams @params = default;
            @params.position = def.props.position;
            @params.rotation = def.props.rotation;
            @params.scale = def.props.scale;
            @params.timeScale = def.props.timeScale;
            @params.skin = def.props.skin;
            @params.defaultAnimation = def.props.idleAnimation;
            @params.loop = loop;
            @params.skeletonColor = def.props.skeletonColor;
            @params.slotSettings = def.props.slotSettings;
            return @params;
        }

        public static T ParseTo<T>(this MixBlendInternal mixBlend) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), mixBlend.ToString());
        }

        public static string GetJobName(Pawn pawn)
        {
            return pawn?.CurJobDef?.defName ?? "";
        }

        public static float GetMood(Pawn pawn)
        {
            return pawn?.needs?.mood?.CurLevelPercentage ?? 0.5f;
        }

        public static float GetPain(Pawn pawn)
        {
            return (pawn?.health?.hediffSet?.PainTotal).GetValueOrDefault();
        }

        public static List<Thought> GetThoughts(Pawn pawn)
        {
            List<Thought> list = new();
            pawn?.needs?.mood?.thoughts?.GetAllMoodThoughts(list);
            return list;
        }
    }
}

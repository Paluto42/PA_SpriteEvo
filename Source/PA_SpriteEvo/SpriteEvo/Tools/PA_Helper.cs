using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public static class PA_Helper
    {
        /// <summary>从AnimationDef中读取动画属性作为参数结构体返回</summary>
        public static AnimationParams GetSkeletonParams(this AnimationDef def, bool loop = true)
        {
            AnimationParams @params = default;
            @params.color = def.props.color;
            @params.slotSettings = def.props.slotSettings;
            @params.skin = def.props.skin;
            @params.defaultAnimation = def.props.idleAnimation;
            @params.rotation = def.props.rotation;
            @params.scale = def.props.scale;
            @params.timeScale = def.props.timeScale;
            @params.position = def.props.position;
            @params.loop = loop;
            return @params;
        }

        /*internal static void DoFlipX(this Spine41.Skeleton skeleton, float x) 
        {
            if (x == 1f || x == -1f)
                skeleton.ScaleX = x;
        }*/

        /*internal static void DestoryInNextFrame(this GameObject obj) 
        {
            if (obj != null) 
            {
                GameObject.Destroy(obj);
            }
        }*/

    }
}

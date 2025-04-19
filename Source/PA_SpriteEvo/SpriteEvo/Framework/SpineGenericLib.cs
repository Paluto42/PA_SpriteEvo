using System;
using UnityEngine;

namespace SpriteEvo
{
    public static class SpineGenericLib
    {
        public static TSkeleton GetSkeletonDataFrom<TSkeleton>(AnimationDef animationDef) where TSkeleton : ScriptableObject 
        {
            SkeletonLoader loader = animationDef.mainAsset.TryGetAsset<SkeletonLoader>();
            if (loader == null){
                throw new NullReferenceException($"SpriteEvo. Main Asset Not Found In {animationDef.defName}");
            }
            return loader.GetSkeletonDataAsset<TSkeleton>();
        }
    }
}

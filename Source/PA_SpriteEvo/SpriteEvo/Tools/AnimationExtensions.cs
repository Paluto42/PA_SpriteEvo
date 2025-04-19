using System.Collections.Generic;
using UnityEngine;

namespace SpriteEvo
{
    public static class AnimationExtensions
    {
        //public static bool Is_StraightAlphaTexture = false;
        public static Shader Spine_Skeleton => AssetLoadManager.Spine_Skeleton;
        //public static Material Spine_SkeletonGraphic => AssetLoadManager.SkeletonGraphic;
        #region 一些小拓展方法
        ///<summary>获取一个模型实例的定位点Bone</summary>
        public static Spine38.Bone RootBone(this Spine38.Unity.SkeletonAnimation instance)
        {
            return instance.skeleton.RootBone;
        }
        public static Spine41.Bone RootBone(this Spine41.Unity.SkeletonAnimation instance)
        {
            return instance.skeleton.RootBone;
        }
        ///<summary>获取一个模型实例里的Bone (可能为Null)</summary>
        public static Spine38.Bone GetBone(this Spine38.Unity.SkeletonAnimation instance, string name)
        {
            return instance.skeleton.FindBone(name);
        }
        public static Spine41.Bone GetBone(this Spine41.Unity.SkeletonAnimation instance, string name)
        {
            return instance.skeleton.FindBone(name);
        }
        ///<summary>获取一个模型实例里的Bone的实际坐标 (不推荐在动画运动时使用)</summary>
        public static Vector3 GetBonePositon(this Spine38.Unity.SkeletonAnimation instance, string name)
        {
            Spine38.Bone bone = instance.skeleton.FindBone(name);
            if (bone == null)
            {
                return Vector3.zero;
            }
            return Spine38.Unity.SkeletonExtensions.GetWorldPosition(bone, instance.transform);
        }
        public static Vector3 GetBonePositon(this Spine41.Unity.SkeletonAnimation instance, string name)
        {
            Spine41.Bone bone = instance.skeleton.FindBone(name);
            if (bone == null)
            {
                return Vector3.zero;
            }
            return Spine41.Unity.SkeletonExtensions.GetWorldPosition(bone, instance.transform);
        }
        ///<summary>访问SkeletonAnimation实例的SkeletonDataAsset 获取指定名称的Spine动画数据对象</summary>
        public static Spine38.Animation GetAnimation(this Spine38.Unity.SkeletonAnimation instance, string name)
        {
            return instance?.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(name);
        }
        ///<summary>访问SkeletonAnimation实例的SkeletonDataAsset 返回包含该骨骼文件的全部动画对象的列表</summary>
        public static List<Spine38.Animation> GetAllAnimations(this Spine38.Unity.SkeletonAnimation instance)
        {
            Spine38.ExposedList<Spine38.Animation> animations = instance?.SkeletonDataAsset.GetSkeletonData(false).Animations;
            List<Spine38.Animation> IList = new();
            for (int i = 0, n = animations.Count; i < n; i++)
            {
                Spine38.Animation animation = animations.Items[i];
                IList.Add(animation);
            }
            return IList;
        }
        #endregion
        public static Spine38.Unity.SkeletonAnimation GetSkeletonAnimation38(GameObject instance) 
        {
            return instance?.GetComponent<Spine38.Unity.SkeletonAnimation>();
        }
        public static Spine41.Unity.SkeletonAnimation GetSkeletonAnimation41(GameObject instance)
        {
            return instance?.GetComponent<Spine41.Unity.SkeletonAnimation>();
        }
        //对整个骨架染色。在激活后调用,应在实例级别的骨架上修改
        /*public static void SetColor(this Spine38.Unity.SkeletonAnimation instance, Color color, List<SlotSettings> slotSettings) 
        {
            if (instance == null) return;
            Spine38.Unity.ISkeletonComponent skeletonComponent = instance.gameObject.GetComponent<Spine38.Unity.ISkeletonComponent>();
            if (skeletonComponent != null)
            {
                Spine38.Skeleton skeleton = skeletonComponent.Skeleton;
                Spine38.Unity.SkeletonExtensions.SetColor(skeleton, color);
                foreach (SlotSettings s in slotSettings)
                {
                    Spine38.Slot slot = skeleton.FindSlot(s.slot);
                    if (slot != null) 
                        Spine38.Unity.SkeletonExtensions.SetColor(slot, color);
                }
            }
        }*/

    }
}

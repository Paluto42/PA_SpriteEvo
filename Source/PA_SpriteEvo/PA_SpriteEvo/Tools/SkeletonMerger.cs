using System;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public class SkeletonMerger
    {
        public SkeletonMerger(Spine41.Unity.SkeletonDataAsset parent, Spine41.Unity.SkeletonDataAsset child)
        {
            try
            {
                skeletonData = MergeSkeleton_Spine41(parent, child);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Spine41.SkeletonData SkeletonData => skeletonData;

        internal Spine41.SkeletonData skeletonData;
        public Spine38.SkeletonData GetSkeletonData38(Spine38.Unity.SkeletonDataAsset skeletonDataAsset)
        {
            return skeletonDataAsset.skeletonDataInternal();
        }
        public Spine41.SkeletonData GetSkeletonData41(Spine41.Unity.SkeletonDataAsset skeletonDataAsset) 
        {
            return skeletonDataAsset.skeletonDataInternal();
        }
        //用这个方法会修改原来的skeletonDataAsset实例内的SkeletonData，而不是创建一个副本，使用原数据需要重新创建一个SkeletonDataAsset运行时实例
        public Spine41.SkeletonData MergeSkeleton_Spine41(Spine41.Unity.SkeletonDataAsset PARENTAsset, Spine41.Unity.SkeletonDataAsset CHILDAsset)
        {
            var PARENT = PARENTAsset.skeletonDataInternal();
            var CHILD = CHILDAsset.skeletonDataInternal();
            //禁止同骨架合并
            if (PARENT.hash == CHILD.hash) { return PARENT; throw new Exception("invild hash conflict: " + CHILD.Name); }
            var P_bones = PARENT.bones;
            var P_slots = PARENT.slots;
            var P_skins = PARENT.skins;
            var P_Animations = PARENT.animations;
            // Bones.
            foreach (Spine41.BoneData cbone in CHILD.bones) 
            {
                //重名直接认为是重复副本，跳过，但是考虑也要检查是否为命名冲突（有不同x,y值）
                if (P_bones.Exists(pb => pb.Name == cbone.Name))
                {
                    /*Spine41.BoneData p = PARENT.bones.Find(b => b.name == c.name);
                    if (c.x != p.x || c.y != p.y)
                    {
                    }*/
                    continue; 
                }
                else if(P_bones.Exists(pb => pb == cbone.parent))
                { P_bones.Add(cbone); }
            }
            // Slots.
            foreach (Spine41.SlotData cslot in CHILD.slots) 
            {
                //同插槽拒绝合并
                if (P_slots.Exists(pslot => pslot.Name == cslot.Name))
                {//#Pending
                    continue; 
                }
                else if (P_bones.Exists(pb => pb == cslot.boneData))
                { P_slots.Add(cslot); }
            }
            // IK constraints. #Pending

            // Transform constraints. #Pending

            // Path constraints. #Pending

            // Skins.
            foreach (Spine41.Skin cskin in CHILD.Skins) 
            {
                //default skin
                var Pskin = P_skins.Find(pskin => pskin.Name == cskin.Name);
                if (Pskin != null)
                {
                    //Attachments .
                    foreach (var KVpairs in cskin.AttachmentsInternal())
                    {
                        string slotname = KVpairs.Key.name;
                        string attachmentName = KVpairs.Value.Name;
                        if (P_slots.Exists(pslot => pslot.attachmentName == attachmentName))
                        {
                            int slotIndex = FindSlotIndex(PARENT, slotname);
                            Pskin.SetAttachment(slotIndex, slotname, KVpairs.Value.attachment);
                        }
                    }
                    continue;
                }
                P_skins.Add(cskin);
                //else continue;
            }
            // Linked meshes. #Pending
            // Events #Pending
            // Animations. #Pending
            foreach (Spine41.Animation canimation in CHILD.animations) 
            {
                //拒绝同名动画合并
                if (P_Animations.Exists(panimation => panimation.Name == canimation.Name))
                {
                    continue;
                }
                //感谢Spine的一堆泛型类，写个Bone时间轴的先
                Spine41.ExposedList<Spine41.Timeline> timelines = new Spine41.ExposedList<Spine41.Timeline>();
                foreach (Spine41.Timeline timeline in canimation.timelines) 
                {
                    //一堆代码然而最终只为了一个对象呃呃呃
                    int boneIndex;
                    switch (timeline) {
                        //--main--
                        case Spine41.RotateTimeline rotateTimeline:
                            boneIndex = FindBoneIndex(PARENT, FindBoneName(CHILD, rotateTimeline.BoneIndex));
                            rotateTimeline.propertyIds = new string[] { (int)Spine41.Property.Rotate + "|" + boneIndex };
                            rotateTimeline.boneIndex = boneIndex;break;
                        case Spine41.TranslateTimeline translateTimeline:
                            boneIndex = FindBoneIndex(PARENT, FindBoneName(CHILD, translateTimeline.BoneIndex));
                            translateTimeline.propertyIds = new string[] { (int)Spine41.Property.Rotate + "|" + boneIndex };
                            translateTimeline.boneIndex = boneIndex;break;
                        //--------
                        case Spine41.TranslateXTimeline translateXTimeline:
                            boneIndex = FindBoneIndex(PARENT, FindBoneName(CHILD, translateXTimeline.BoneIndex));
                            translateXTimeline.propertyIds = new string[] { (int)Spine41.Property.Rotate + "|" + boneIndex };
                            translateXTimeline.boneIndex = boneIndex;break;
                        case Spine41.TranslateYTimeline translateYTimeline:
                            boneIndex = FindBoneIndex(PARENT, FindBoneName(CHILD, translateYTimeline.BoneIndex));
                            translateYTimeline.propertyIds = new string[] { (int)Spine41.Property.Rotate + "|" + boneIndex };
                            translateYTimeline.boneIndex = boneIndex;break;
                        case Spine41.ScaleXTimeline scaleXTimeline:
                            boneIndex = FindBoneIndex(PARENT, FindBoneName(CHILD, scaleXTimeline.BoneIndex));
                            scaleXTimeline.propertyIds = new string[] { (int)Spine41.Property.Rotate + "|" + boneIndex };
                            scaleXTimeline.boneIndex = boneIndex;break;
                        case Spine41.ScaleYTimeline scaleYTimeline:
                            boneIndex = FindBoneIndex(PARENT, FindBoneName(CHILD, scaleYTimeline.BoneIndex));
                            scaleYTimeline.propertyIds = new string[] { (int)Spine41.Property.Rotate + "|" + boneIndex };
                            scaleYTimeline.boneIndex = boneIndex;break;
                        case Spine41.ShearXTimeline shearXTimeline:
                            boneIndex = FindBoneIndex(PARENT, FindBoneName(CHILD, shearXTimeline.BoneIndex));
                            shearXTimeline.propertyIds = new string[] { (int)Spine41.Property.Rotate + "|" + boneIndex };
                            shearXTimeline.boneIndex = boneIndex;break;
                        case Spine41.ShearYTimeline shearYTimeline:
                            boneIndex = FindBoneIndex(PARENT, FindBoneName(CHILD, shearYTimeline.BoneIndex));
                            shearYTimeline.propertyIds = new string[] { (int)Spine41.Property.Rotate + "|" + boneIndex };
                            shearYTimeline.boneIndex = boneIndex;break;
                    }
                    timelines.Add(timeline);
                }
                P_Animations.Add(new Spine41.Animation(canimation.Name, timelines, canimation.duration));

            }
            P_bones.TrimExcess();
            P_slots.TrimExcess();
            P_skins.TrimExcess();
            P_Animations.TrimExcess();
            return PARENT;
        }

        private int FindSlotIndex(Spine41.SkeletonData skeletonData, string slotName)
        {
            Spine41.SlotData[] slots = skeletonData.slots.Items;
            for (int i = 0, n = skeletonData.slots.Count; i < n; i++) 
            {
                if (slots[i].name == slotName) return i;
            }
            throw new Exception("Slot not found: " + slotName);
        }

        public int FindBoneIndex(Spine41.SkeletonData skeletonData, string boneName) 
        {
            int boneIndex = -1;
            Spine41.BoneData[] bones = skeletonData.bones.Items;
            for (int i = 0, n = skeletonData.bones.Count; i < n; i++) 
            {
                if (bones[i].name == boneName) return boneIndex = i;
            }
            throw new Exception("Bone not found: " + boneName);
        }

        public string FindBoneName(Spine41.SkeletonData skeletonData, int boneIndex)
        {
            Spine41.BoneData[] bones = skeletonData.bones.Items;
            return bones[boneIndex].Name;
            throw new Exception("Bone not found: " + skeletonData.Name);
        }
    }
}

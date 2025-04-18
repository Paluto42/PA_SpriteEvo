using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SpriteEvo.Extensions
{
    //从内部对象修改SkeletonData的尝试，失败
    /*public class SkeletonMerger
    {
        
            public SkeletonMerger(Spine41.Unity.SkeletonDataAsset parent, Spine41.Unity.SkeletonDataAsset child)
            {
                try{
                    skeletonData = MergeSkeleton_Spine41(parent, child);
                    scale = parent.scale;
                }
                catch (Exception ex){
                    throw ex;
                }
            }
            internal Spine41.SkeletonData skeletonData;

            internal float scale;
            public Spine41.SkeletonData SkeletonData => skeletonData;
            public Spine38.SkeletonData GetSkeletonData38(Spine38.Unity.SkeletonDataAsset skeletonDataAsset)
            {
                return skeletonDataAsset.skeletonDataInternal();
            }
            public Spine41.SkeletonData GetSkeletonData41(Spine41.Unity.SkeletonDataAsset skeletonDataAsset) 
            {
                return skeletonDataAsset.skeletonDataInternal();
            }
            //用这个方法会修改原来的skeletonDataAsset实例内的SkeletonData，而不是创建一个副本，使用原数据需要重新创建一个新的SkeletonDataAsset运行时实例
            //使用这个方法的前提是已经使用AppendToAtlasText和AppendToTextureArray合并了新的SpineAtlasAsset，
            //并与原来的skeleton导出文件一起生成新的SkeletonDataAsset(这行得通)
            public Spine41.SkeletonData MergeSkeleton_Spine41(Spine41.Unity.SkeletonDataAsset PARENTAsset, Spine41.Unity.SkeletonDataAsset CHILDAsset)
            {
                var PARENT = PARENTAsset.skeletonDataInternal();
                var CHILD = CHILDAsset.skeletonDataInternal();
                //禁止同骨架合并
                if (PARENT.hash == CHILD.hash) { return PARENT; throw new Exception("invild hash conflict: " + CHILD.Name); }
                //Clone
                Spine41.SkeletonData newSkeletonData = new Spine41.SkeletonData{
                    hash = PARENT.hash,
                    version = PARENT.version,
                    x = PARENT.x,
                    y = PARENT.y,
                    width = PARENT.width,
                    height = PARENT.height,
                    fps = PARENT.fps,
                    imagesPath = PARENT.imagesPath,
                    audioPath = PARENT.audioPath
                };
                foreach (var bone in PARENT.bones) {
                    newSkeletonData.bones.Add(bone);
                }
                foreach (var slot in PARENT.slots) {
                    newSkeletonData.slots.Add(slot);
                }
                foreach (var skin in PARENT.Skins) {
                    newSkeletonData.skins.Add(skin);
                    if (skin.name == "default") newSkeletonData.defaultSkin = skin;
                }
                foreach(var animation in PARENT.Animations)
                {
                    newSkeletonData.animations.Add(animation);
                }
                //Clone
                var P_bones = newSkeletonData.bones;
                var P_slots = newSkeletonData.slots;
                var P_skins = newSkeletonData.Skins;
                var P_Animations = newSkeletonData.Animations;
                // Bones.
                foreach (Spine41.BoneData cbone in CHILD.bones) 
                {
                    //重名直接认为是重复副本，跳过，但是考虑也要检查是否为命名冲突（有不同x,y值）
                    if (P_bones.Exists(pb => pb.Name == cbone.Name))
                    {
                        /*Spine41.BoneData p = PARENT.bones.Find(b => b.name == c.name);
                        if (c.x != p.x || c.y != p.y)
                        {
                        }
                        continue;
                    }
                    //合并的硬性要求：bone的parent要存在于原本的骨架中
                    else if(P_bones.Exists(pb => pb == cbone.parent))
                    { P_bones.Add(new Spine41.BoneData(P_bones.Count, cbone.name, cbone.parent)); }
                }
                // Slots.
                foreach (Spine41.SlotData cslot in CHILD.slots) 
                {
                    //同名插槽拒绝合并
                    if (P_slots.Exists(pslot => pslot.Name == cslot.Name))
                    {//#Pending
                        continue; 
                    }
                    else if (P_bones.Exists(pb => pb == cslot.boneData))
                    {
                        P_slots.Add(new Spine41.SlotData(P_slots.Count, cslot.name, cslot.boneData));
                    }
                }
                // IK constraints. #Pending

                // Transform constraints. #Pending

                // Path constraints. #Pending

                // Skins.
                foreach (Spine41.Skin cskin in CHILD.Skins) 
                {
                    //default skin 进行同名skin和默认skin的合并
                    var Pskin = P_skins.Find(pskin => pskin.Name == cskin.Name);
                    if (Pskin != null)
                    {
                        //Attachments .
                        foreach (var KVpairs in cskin.AttachmentsInternal())
                        {
                            string slotname = KVpairs.Key.name;//插槽名
                            string attachmentName = KVpairs.Value.Name;//附件名
                            if (P_slots.Exists(pslot => pslot.Name == slotname && pslot.AttachmentName == attachmentName))
                            {
                                int slotIndex = FindSlotIndex(newSkeletonData, slotname);
                                PARENTAsset.
                                Pskin.SetAttachment(slotIndex, slotname, KVpairs.Value.attachment);//为当前skin添加附件
                            }
                        }
                        continue;
                    }
                    else {//最好需要检查记得补全
                        P_skins.Add(cskin);
                    }
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
                    //感谢Spine的一堆泛型类，暂先写个Bone时间轴的先
                    Spine41.ExposedList<Spine41.Timeline> timelines = new Spine41.ExposedList<Spine41.Timeline>();
                    foreach (Spine41.Timeline timeline in canimation.timelines) 
                    {
                        //一堆代码呃呃呃
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
            private Spine41.Attachment ReadAttachment(Dictionary<string, object> map, Spine41.Skin skin, int slotIndex, string name, Spine41.SkeletonData skeletonData)
            {
                float scale = this.scale;
                name = Spine41.SkeletonJson.GetString(map, "name", name);

                string typeName = Spine41.SkeletonJson.GetString(map, "type", "region");
                Spine41.AttachmentType type = (Spine41.AttachmentType)Enum.Parse(typeof(Spine41.AttachmentType), typeName, true);

                switch (type)
                {
                    case Spine41.AttachmentType.Region:
                        {
                            string path = Spine41.SkeletonJson.GetString(map, "path", name);
                            object sequenceJson;
                            map.TryGetValue("sequence", out sequenceJson);
                            Spine41.Sequence sequence = Spine41.SkeletonJson.ReadSequence(sequenceJson);
                            Spine41.RegionAttachment region = attachmentLoader.NewRegionAttachment(skin, name, path, sequence);
                            if (region == null) return null;
                            region.Path = path;
                            region.x = GetFloat(map, "x", 0) * scale;
                            region.y = GetFloat(map, "y", 0) * scale;
                            region.scaleX = GetFloat(map, "scaleX", 1);
                            region.scaleY = GetFloat(map, "scaleY", 1);
                            region.rotation = GetFloat(map, "rotation", 0);
                            region.width = GetFloat(map, "width", 32) * scale;
                            region.height = GetFloat(map, "height", 32) * scale;
                            region.sequence = sequence;

                            if (map.ContainsKey("color"))
                            {
                                string color = (string)map["color"];
                                region.r = ToColor(color, 0);
                                region.g = ToColor(color, 1);
                                region.b = ToColor(color, 2);
                                region.a = ToColor(color, 3);
                            }

                            if (region.Region != null) region.UpdateRegion();
                            return region;
                        }
                    case AttachmentType.Boundingbox:
                        BoundingBoxAttachment box = attachmentLoader.NewBoundingBoxAttachment(skin, name);
                        if (box == null) return null;
                        ReadVertices(map, box, GetInt(map, "vertexCount", 0) << 1);
                        return box;
                    case AttachmentType.Mesh:
                    case AttachmentType.Linkedmesh:
                        {
                            string path = GetString(map, "path", name);
                            object sequenceJson;
                            map.TryGetValue("sequence", out sequenceJson);
                            Sequence sequence = ReadSequence(sequenceJson);
                            MeshAttachment mesh = attachmentLoader.NewMeshAttachment(skin, name, path, sequence);
                            if (mesh == null) return null;
                            mesh.Path = path;

                            if (map.ContainsKey("color"))
                            {
                                string color = (string)map["color"];
                                mesh.r = ToColor(color, 0);
                                mesh.g = ToColor(color, 1);
                                mesh.b = ToColor(color, 2);
                                mesh.a = ToColor(color, 3);
                            }

                            mesh.Width = GetFloat(map, "width", 0) * scale;
                            mesh.Height = GetFloat(map, "height", 0) * scale;
                            mesh.Sequence = sequence;

                            string parent = GetString(map, "parent", null);
                            if (parent != null)
                            {
                                linkedMeshes.Add(new LinkedMesh(mesh, GetString(map, "skin", null), slotIndex, parent, GetBoolean(map, "timelines", true)));
                                return mesh;
                            }

                            float[] uvs = GetFloatArray(map, "uvs", 1);
                            ReadVertices(map, mesh, uvs.Length);
                            mesh.triangles = GetIntArray(map, "triangles");
                            mesh.regionUVs = uvs;
                            if (mesh.Region != null) mesh.UpdateRegion();

                            if (map.ContainsKey("hull")) mesh.HullLength = GetInt(map, "hull", 0) << 1;
                            if (map.ContainsKey("edges")) mesh.Edges = GetIntArray(map, "edges");
                            return mesh;
                        }
                    case AttachmentType.Path:
                        {
                            PathAttachment pathAttachment = attachmentLoader.NewPathAttachment(skin, name);
                            if (pathAttachment == null) return null;
                            pathAttachment.closed = GetBoolean(map, "closed", false);
                            pathAttachment.constantSpeed = GetBoolean(map, "constantSpeed", true);

                            int vertexCount = GetInt(map, "vertexCount", 0);
                            ReadVertices(map, pathAttachment, vertexCount << 1);

                            // potential BOZO see Java impl
                            pathAttachment.lengths = GetFloatArray(map, "lengths", scale);
                            return pathAttachment;
                        }
                    case AttachmentType.Point:
                        {
                            PointAttachment point = attachmentLoader.NewPointAttachment(skin, name);
                            if (point == null) return null;
                            point.x = GetFloat(map, "x", 0) * scale;
                            point.y = GetFloat(map, "y", 0) * scale;
                            point.rotation = GetFloat(map, "rotation", 0);

                            //string color = GetString(map, "color", null);
                            //if (color != null) point.color = color;
                            return point;
                        }
                    case AttachmentType.Clipping:
                        {
                            ClippingAttachment clip = attachmentLoader.NewClippingAttachment(skin, name);
                            if (clip == null) return null;

                            string end = GetString(map, "end", null);
                            if (end != null)
                            {
                                SlotData slot = skeletonData.FindSlot(end);
                                if (slot == null) throw new Exception("Clipping end slot not found: " + end);
                                clip.EndSlot = slot;
                            }

                            ReadVertices(map, clip, GetInt(map, "vertexCount", 0) << 1);

                            //string color = GetString(map, "color", null);
                            // if (color != null) clip.color = color;
                            return clip;
                        }
                }
                return null;
            }
    }*/
}

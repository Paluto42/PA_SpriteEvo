namespace SpriteEvo.Extensions
{
#if OLD_BUILD
    public class SkeletonJsonMerge42 : SkeletonJson
    {
        public SkeletonJsonMerge42(AttachmentLoader attachmentLoader)
            : base(attachmentLoader){
        }

        public SkeletonJsonMerge42(params Atlas[] atlasArray)
            : base(atlasArray){
        }

        private void ProcessBones(SkeletonData skeletonData, Dictionary<string, object> root, bool isSubSkeleton = false)
        {
            foreach (Dictionary<string, Object> boneMap in (List<Object>)root["bones"])
            {
                BoneData parent = null;
                if (boneMap.ContainsKey("parent"))
                {
                    parent = skeletonData.FindBone((string)boneMap["parent"]);
                    if (parent == null)
                        throw new Exception("Parent bone not found: " + boneMap["parent"]);
                }
                BoneData data = new BoneData(skeletonData.Bones.Count, (string)boneMap["name"], parent);
                data.length = GetFloat(boneMap, "length", 0) * scale;
                data.x = GetFloat(boneMap, "x", 0) * scale;
                data.y = GetFloat(boneMap, "y", 0) * scale;
                data.rotation = GetFloat(boneMap, "rotation", 0);
                data.scaleX = GetFloat(boneMap, "scaleX", 1);
                data.scaleY = GetFloat(boneMap, "scaleY", 1);
                data.shearX = GetFloat(boneMap, "shearX", 0);
                data.shearY = GetFloat(boneMap, "shearY", 0);

                string inheritString = GetString(boneMap, "inherit", Inherit.Normal.ToString());
                data.inherit = (Inherit)Enum.Parse(typeof(Inherit), inheritString, true);
                data.skinRequired = GetBoolean(boneMap, "skin", false);
                if (isSubSkeleton && skeletonData.bones.Exists(pb => pb.name == data.name))
                {
                    continue;
                }
                skeletonData.bones.Add(data);
            }
        }

        private void ProcessSlots(SkeletonData skeletonData, Dictionary<string, object> root)
        {
            foreach (Dictionary<string, Object> slotMap in (List<Object>)root["slots"])
            {
                string slotName = (string)slotMap["name"];
                string boneName = (string)slotMap["bone"];
                BoneData boneData = skeletonData.FindBone(boneName);
                if (boneData == null) throw new Exception("Slot bone not found: " + boneName);
                SlotData data = new SlotData(skeletonData.Slots.Count, slotName, boneData);

                if (slotMap.ContainsKey("color"))
                {
                    string color = (string)slotMap["color"];
                    data.r = ToColor(color, 0);
                    data.g = ToColor(color, 1);
                    data.b = ToColor(color, 2);
                    data.a = ToColor(color, 3);
                }

                if (slotMap.ContainsKey("dark"))
                {
                    string color2 = (string)slotMap["dark"];
                    data.r2 = ToColor(color2, 0, 6); // expectedLength = 6. ie. "RRGGBB"
                    data.g2 = ToColor(color2, 1, 6);
                    data.b2 = ToColor(color2, 2, 6);
                    data.hasSecondColor = true;
                }

                data.attachmentName = GetString(slotMap, "attachment", null);
                if (slotMap.ContainsKey("blend"))
                    data.blendMode = (BlendMode)Enum.Parse(typeof(BlendMode), (string)slotMap["blend"], true);
                else
                    data.blendMode = BlendMode.Normal;
                //data.visible = slotMap.getBoolean("visible", true);
                skeletonData.slots.Add(data);
            }
        }

        private void ProcessSkins(SkeletonData skeletonData, Dictionary<string, object> root)
        {
            foreach (Dictionary<string, object> skinMap in (List<object>)root["skins"])
            {
                Skin skin = new Skin((string)skinMap["name"]);
                if (skinMap.ContainsKey("bones"))
                {
                    foreach (string entryName in (List<Object>)skinMap["bones"])
                    {
                        BoneData bone = skeletonData.FindBone(entryName);
                        if (bone == null) throw new Exception("Skin bone not found: " + entryName);
                        skin.bones.Add(bone);
                    }
                }
                skin.bones.TrimExcess();
                if (skinMap.ContainsKey("ik"))
                {
                    foreach (string entryName in (List<Object>)skinMap["ik"])
                    {
                        IkConstraintData constraint = skeletonData.FindIkConstraint(entryName);
                        if (constraint == null) throw new Exception("Skin IK constraint not found: " + entryName);
                        skin.constraints.Add(constraint);
                    }
                }
                if (skinMap.ContainsKey("transform"))
                {
                    foreach (string entryName in (List<Object>)skinMap["transform"])
                    {
                        TransformConstraintData constraint = skeletonData.FindTransformConstraint(entryName);
                        if (constraint == null) throw new Exception("Skin transform constraint not found: " + entryName);
                        skin.constraints.Add(constraint);
                    }
                }
                if (skinMap.ContainsKey("path"))
                {
                    foreach (string entryName in (List<Object>)skinMap["path"])
                    {
                        PathConstraintData constraint = skeletonData.FindPathConstraint(entryName);
                        if (constraint == null) throw new Exception("Skin path constraint not found: " + entryName);
                        skin.constraints.Add(constraint);
                    }
                }
                if (skinMap.ContainsKey("physics"))
                {
                    foreach (string entryName in (List<Object>)skinMap["physics"])
                    {
                        PhysicsConstraintData constraint = skeletonData.FindPhysicsConstraint(entryName);
                        if (constraint == null) throw new Exception("Skin physics constraint not found: " + entryName);
                        skin.constraints.Add(constraint);
                    }
                }
                skin.constraints.TrimExcess();
                if (skinMap.ContainsKey("attachments"))
                {
                    foreach (KeyValuePair<string, Object> slotEntry in (Dictionary<string, Object>)skinMap["attachments"])
                    {
                        int slotIndex = FindSlotIndex(skeletonData, slotEntry.Key);
                        foreach (KeyValuePair<string, Object> entry in ((Dictionary<string, Object>)slotEntry.Value))
                        {
                            try
                            {
                                Attachment attachment = ReadAttachment((Dictionary<string, Object>)entry.Value, skin, slotIndex, entry.Key, skeletonData);
                                if (attachment != null) skin.SetAttachment(slotIndex, entry.Key, attachment);
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Error reading attachment: " + entry.Key + ", skin: " + skin, e);
                            }
                        }
                    }
                }
                skeletonData.skins.Add(skin);
                if (skin.name == "default") skeletonData.defaultSkin = skin;
            }
        }

        public SkeletonData ReadSkeletonDatasToMerge(TextReader reader1, TextReader[] reader2)
        {
            if (reader1 == null || reader2.NullOrEmpty()) throw new ArgumentNullException("reader", "reader cannot be null.");

            float scale = this.scale;
            SkeletonData skeletonData = new SkeletonData();

            Dictionary<string, object> root1 = Json.Deserialize(reader1) as Dictionary<string, Object>;
            List<Dictionary<string, object>> roots = new();
            foreach (TextReader reader in reader2)
            {
                Dictionary<string, object> newroot = Json.Deserialize(reader) as Dictionary<string, Object>;
                roots.Add(newroot);
            }

            if (root1 == null || roots.NullOrEmpty()) throw new Exception("Invalid JSON.");

            // Skeleton.
            if (root1.ContainsKey("skeleton"))
            {
                Dictionary<string, object> skeletonMap = (Dictionary<string, Object>)root1["skeleton"];
                skeletonData.hash = (string)skeletonMap["hash"];
                skeletonData.version = (string)skeletonMap["spine"];
                skeletonData.x = GetFloat(skeletonMap, "x", 0);
                skeletonData.y = GetFloat(skeletonMap, "y", 0);
                skeletonData.width = GetFloat(skeletonMap, "width", 0);
                skeletonData.height = GetFloat(skeletonMap, "height", 0);
                skeletonData.referenceScale = GetFloat(skeletonMap, "referenceScale", 100) * scale;
                skeletonData.fps = GetFloat(skeletonMap, "fps", 30);
                skeletonData.imagesPath = GetString(skeletonMap, "images", null);
                skeletonData.audioPath = GetString(skeletonMap, "audio", null);
            }

            // Bones.
            if (root1.ContainsKey("bones"))
            {
                ProcessBones(skeletonData, root1);
            }
            foreach (var root2 in roots)
            {
                if (root2.ContainsKey("bones"))
                {
                    ProcessBones(skeletonData, root2);
                }
            }

            // Slots.
            if (root1.ContainsKey("slots"))
            {
                ProcessSlots(skeletonData, root1);
            }
            foreach (var root2 in roots)
            {
                if (root2.ContainsKey("slots"))
                {
                    ProcessSlots(skeletonData, root2);
                }
            }

            // IK constraints.
            if (root1.ContainsKey("ik"))
            {
                foreach (Dictionary<string, Object> constraintMap in (List<Object>)root1["ik"])
                {
                    IkConstraintData data = new IkConstraintData((string)constraintMap["name"]);
                    data.order = GetInt(constraintMap, "order", 0);
                    data.skinRequired = GetBoolean(constraintMap, "skin", false);

                    if (constraintMap.ContainsKey("bones"))
                    {
                        foreach (string boneName in (List<Object>)constraintMap["bones"])
                        {
                            BoneData bone = skeletonData.FindBone(boneName);
                            if (bone == null) throw new Exception("IK bone not found: " + boneName);
                            data.bones.Add(bone);
                        }
                    }

                    string targetName = (string)constraintMap["target"];
                    data.target = skeletonData.FindBone(targetName);
                    if (data.target == null) throw new Exception("IK target bone not found: " + targetName);
                    data.mix = GetFloat(constraintMap, "mix", 1);
                    data.softness = GetFloat(constraintMap, "softness", 0) * scale;
                    data.bendDirection = GetBoolean(constraintMap, "bendPositive", true) ? 1 : -1;
                    data.compress = GetBoolean(constraintMap, "compress", false);
                    data.stretch = GetBoolean(constraintMap, "stretch", false);
                    data.uniform = GetBoolean(constraintMap, "uniform", false);

                    skeletonData.ikConstraints.Add(data);
                }
            }

            // Transform constraints.
            if (root1.ContainsKey("transform"))
            {
                foreach (Dictionary<string, Object> constraintMap in (List<Object>)root1["transform"])
                {
                    TransformConstraintData data = new TransformConstraintData((string)constraintMap["name"]);
                    data.order = GetInt(constraintMap, "order", 0);
                    data.skinRequired = GetBoolean(constraintMap, "skin", false);

                    if (constraintMap.ContainsKey("bones"))
                    {
                        foreach (string boneName in (List<Object>)constraintMap["bones"])
                        {
                            BoneData bone = skeletonData.FindBone(boneName);
                            if (bone == null) throw new Exception("Transform constraint bone not found: " + boneName);
                            data.bones.Add(bone);
                        }
                    }

                    string targetName = (string)constraintMap["target"];
                    data.target = skeletonData.FindBone(targetName);
                    if (data.target == null) throw new Exception("Transform constraint target bone not found: " + targetName);

                    data.local = GetBoolean(constraintMap, "local", false);
                    data.relative = GetBoolean(constraintMap, "relative", false);

                    data.offsetRotation = GetFloat(constraintMap, "rotation", 0);
                    data.offsetX = GetFloat(constraintMap, "x", 0) * scale;
                    data.offsetY = GetFloat(constraintMap, "y", 0) * scale;
                    data.offsetScaleX = GetFloat(constraintMap, "scaleX", 0);
                    data.offsetScaleY = GetFloat(constraintMap, "scaleY", 0);
                    data.offsetShearY = GetFloat(constraintMap, "shearY", 0);

                    data.mixRotate = GetFloat(constraintMap, "mixRotate", 1);
                    data.mixX = GetFloat(constraintMap, "mixX", 1);
                    data.mixY = GetFloat(constraintMap, "mixY", data.mixX);
                    data.mixScaleX = GetFloat(constraintMap, "mixScaleX", 1);
                    data.mixScaleY = GetFloat(constraintMap, "mixScaleY", data.mixScaleX);
                    data.mixShearY = GetFloat(constraintMap, "mixShearY", 1);

                    skeletonData.transformConstraints.Add(data);
                }
            }

            // Path constraints.
            if (root1.ContainsKey("path"))
            {
                foreach (Dictionary<string, Object> constraintMap in (List<Object>)root1["path"])
                {
                    PathConstraintData data = new PathConstraintData((string)constraintMap["name"]);
                    data.order = GetInt(constraintMap, "order", 0);
                    data.skinRequired = GetBoolean(constraintMap, "skin", false);

                    if (constraintMap.ContainsKey("bones"))
                    {
                        foreach (string boneName in (List<Object>)constraintMap["bones"])
                        {
                            BoneData bone = skeletonData.FindBone(boneName);
                            if (bone == null) throw new Exception("Path bone not found: " + boneName);
                            data.bones.Add(bone);
                        }
                    }

                    string targetName = (string)constraintMap["target"];
                    data.target = skeletonData.FindSlot(targetName);
                    if (data.target == null) throw new Exception("Path target slot not found: " + targetName);

                    data.positionMode = (PositionMode)Enum.Parse(typeof(PositionMode), GetString(constraintMap, "positionMode", "percent"), true);
                    data.spacingMode = (SpacingMode)Enum.Parse(typeof(SpacingMode), GetString(constraintMap, "spacingMode", "length"), true);
                    data.rotateMode = (RotateMode)Enum.Parse(typeof(RotateMode), GetString(constraintMap, "rotateMode", "tangent"), true);
                    data.offsetRotation = GetFloat(constraintMap, "rotation", 0);
                    data.position = GetFloat(constraintMap, "position", 0);
                    if (data.positionMode == PositionMode.Fixed) data.position *= scale;
                    data.spacing = GetFloat(constraintMap, "spacing", 0);
                    if (data.spacingMode == SpacingMode.Length || data.spacingMode == SpacingMode.Fixed) data.spacing *= scale;
                    data.mixRotate = GetFloat(constraintMap, "mixRotate", 1);
                    data.mixX = GetFloat(constraintMap, "mixX", 1);
                    data.mixY = GetFloat(constraintMap, "mixY", data.mixX);

                    skeletonData.pathConstraints.Add(data);
                }
            }

            // Physics constraints.
            if (root1.ContainsKey("physics"))
            {
                foreach (Dictionary<string, Object> constraintMap in (List<Object>)root1["physics"])
                {
                    PhysicsConstraintData data = new PhysicsConstraintData((string)constraintMap["name"]);
                    data.order = GetInt(constraintMap, "order", 0);
                    data.skinRequired = GetBoolean(constraintMap, "skin", false);

                    string boneName = (string)constraintMap["bone"];
                    data.bone = skeletonData.FindBone(boneName);
                    if (data.bone == null) throw new Exception("Physics bone not found: " + boneName);

                    data.x = GetFloat(constraintMap, "x", 0);
                    data.y = GetFloat(constraintMap, "y", 0);
                    data.rotate = GetFloat(constraintMap, "rotate", 0);
                    data.scaleX = GetFloat(constraintMap, "scaleX", 0);
                    data.shearX = GetFloat(constraintMap, "shearX", 0);
                    data.limit = GetFloat(constraintMap, "limit", 5000) * scale;
                    data.step = 1f / GetInt(constraintMap, "fps", 60);
                    data.inertia = GetFloat(constraintMap, "inertia", 1);
                    data.strength = GetFloat(constraintMap, "strength", 100);
                    data.damping = GetFloat(constraintMap, "damping", 1);
                    data.massInverse = 1f / GetFloat(constraintMap, "mass", 1);
                    data.wind = GetFloat(constraintMap, "wind", 0);
                    data.gravity = GetFloat(constraintMap, "gravity", 0);
                    data.mix = GetFloat(constraintMap, "mix", 1);
                    data.inertiaGlobal = GetBoolean(constraintMap, "inertiaGlobal", false);
                    data.strengthGlobal = GetBoolean(constraintMap, "strengthGlobal", false);
                    data.dampingGlobal = GetBoolean(constraintMap, "dampingGlobal", false);
                    data.massGlobal = GetBoolean(constraintMap, "massGlobal", false);
                    data.windGlobal = GetBoolean(constraintMap, "windGlobal", false);
                    data.gravityGlobal = GetBoolean(constraintMap, "gravityGlobal", false);
                    data.mixGlobal = GetBoolean(constraintMap, "mixGlobal", false);

                    skeletonData.physicsConstraints.Add(data);
                }
            }

            // Skins.
            if (root1.ContainsKey("skins"))
            {
                if (root1.ContainsKey("skins"))
                {
                    ProcessSkins(skeletonData, root1);
                }
                foreach (var root2 in roots)
                {
                    foreach (Dictionary<string, object> skinMap in (List<object>)root2["skins"])
                    {
                        Skin skin = new Skin((string)skinMap["name"]);
                        Skin pskin = skeletonData.skins.Find(ps => ps.name == skin.name);
                        bool Duplicated = pskin != null;
                        if (Duplicated)
                        {
                            skin = pskin;
                        }
                        if (skinMap.ContainsKey("bones"))
                        {
                            foreach (string entryName in (List<Object>)skinMap["bones"])
                            {
                                BoneData bone = skeletonData.FindBone(entryName);
                                if (bone == null) throw new Exception("Skin bone not found: " + entryName);
                                skin.bones.Add(bone);
                            }
                        }
                        skin.bones.TrimExcess();
                        if (skinMap.ContainsKey("ik"))
                        {
                            foreach (string entryName in (List<Object>)skinMap["ik"])
                            {
                                IkConstraintData constraint = skeletonData.FindIkConstraint(entryName);
                                if (constraint == null) throw new Exception("Skin IK constraint not found: " + entryName);
                                skin.constraints.Add(constraint);
                            }
                        }
                        if (skinMap.ContainsKey("transform"))
                        {
                            foreach (string entryName in (List<Object>)skinMap["transform"])
                            {
                                TransformConstraintData constraint = skeletonData.FindTransformConstraint(entryName);
                                if (constraint == null) throw new Exception("Skin transform constraint not found: " + entryName);
                                skin.constraints.Add(constraint);
                            }
                        }
                        if (skinMap.ContainsKey("path"))
                        {
                            foreach (string entryName in (List<Object>)skinMap["path"])
                            {
                                PathConstraintData constraint = skeletonData.FindPathConstraint(entryName);
                                if (constraint == null) throw new Exception("Skin path constraint not found: " + entryName);
                                skin.constraints.Add(constraint);
                            }
                        }
                        if (skinMap.ContainsKey("physics"))
                        {
                            foreach (string entryName in (List<Object>)skinMap["physics"])
                            {
                                PhysicsConstraintData constraint = skeletonData.FindPhysicsConstraint(entryName);
                                if (constraint == null) throw new Exception("Skin physics constraint not found: " + entryName);
                                skin.constraints.Add(constraint);
                            }
                        }
                        skin.constraints.TrimExcess();
                        if (skinMap.ContainsKey("attachments"))
                        {
                            foreach (KeyValuePair<string, Object> slotEntry in (Dictionary<string, Object>)skinMap["attachments"])
                            {
                                int slotIndex = FindSlotIndex(skeletonData, slotEntry.Key);
                                foreach (KeyValuePair<string, Object> entry in ((Dictionary<string, Object>)slotEntry.Value))
                                {
                                    try
                                    {
                                        Attachment attachment = ReadAttachment((Dictionary<string, Object>)entry.Value, skin, slotIndex, entry.Key, skeletonData);
                                        if (attachment != null) skin.SetAttachment(slotIndex, entry.Key, attachment);
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception("Error reading attachment: " + entry.Key + ", skin: " + skin, e);
                                    }
                                }
                            }
                        }
                        if (!Duplicated)
                        {
                            skeletonData.skins.Add(skin);
                        }
                        //if (skin.name == "default") skeletonData.defaultSkin = skin;
                    }
                }
            }

            // Linked meshes.
            for (int i = 0, n = linkedMeshes.Count; i < n; i++)
            {
                LinkedMesh linkedMesh = linkedMeshes[i];
                Skin skin = linkedMesh.skin == null ? skeletonData.defaultSkin : skeletonData.FindSkin(linkedMesh.skin);
                if (skin == null) throw new Exception("Slot not found: " + linkedMesh.skin);
                Attachment parent = skin.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
                if (parent == null) throw new Exception("Parent mesh not found: " + linkedMesh.parent);
                linkedMesh.mesh.TimelineAttachment = linkedMesh.inheritTimelines ? (VertexAttachment)parent : linkedMesh.mesh;
                linkedMesh.mesh.ParentMesh = (MeshAttachment)parent;
                if (linkedMesh.mesh.Region != null) linkedMesh.mesh.UpdateRegion();
            }
            linkedMeshes.Clear();

            // Events.
            if (root1.ContainsKey("events"))
            {
                foreach (KeyValuePair<string, Object> entry in (Dictionary<string, Object>)root1["events"])
                {
                    Dictionary<string, object> entryMap = (Dictionary<string, Object>)entry.Value;
                    EventData data = new EventData(entry.Key);
                    data.Int = GetInt(entryMap, "int", 0);
                    data.Float = GetFloat(entryMap, "float", 0);
                    data.String = GetString(entryMap, "string", string.Empty);
                    data.AudioPath = GetString(entryMap, "audio", null);
                    if (data.AudioPath != null)
                    {
                        data.Volume = GetFloat(entryMap, "volume", 1);
                        data.Balance = GetFloat(entryMap, "balance", 0);
                    }
                    skeletonData.events.Add(data);
                }
            }

            // Animations.
            if (root1.ContainsKey("animations"))
            {
                foreach (KeyValuePair<string, Object> entry in (Dictionary<string, Object>)root1["animations"])
                {
                    try
                    {
                        ReadAnimation((Dictionary<string, Object>)entry.Value, entry.Key, skeletonData);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error reading animation: " + entry.Key + "\n" + e.Message, e);
                    }
                }
            }
            foreach (var root2 in roots)
            {
                foreach (KeyValuePair<string, Object> entry in (Dictionary<string, Object>)root2["animations"])
                {
                    try
                    {
                        ReadAnimation((Dictionary<string, Object>)entry.Value, entry.Key, skeletonData);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error reading animation: " + entry.Key + "\n" + e.Message, e);
                    }
                }
            }

            skeletonData.bones.TrimExcess();
            skeletonData.slots.TrimExcess();
            skeletonData.skins.TrimExcess();
            skeletonData.events.TrimExcess();
            skeletonData.animations.TrimExcess();
            skeletonData.ikConstraints.TrimExcess();
            return skeletonData;
        }
    }
#endif
}

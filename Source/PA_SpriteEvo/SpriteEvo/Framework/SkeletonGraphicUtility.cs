using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace SpriteEvo
{
    public static class SkeletonGraphicUtility
    {
        public static bool currentlyGenerating = false;
        public static Material SkeletonGraphicDefault => AssetLoadManager.SkeletonGraphicDefault;
        public static Material SkeletonGraphicDefaul_Straight => AssetLoadManager.SkeletonGraphicDefaul_Straight;
        public static Dictionary<object, GameObject> DynamicObjectDatabase => ObjectManager.NeverDestoryObjectDatabase;

        public static Material EnsureInitializedMaterialProperySource(bool StraightAlphaInput = false) 
        {
            return StraightAlphaInput ? SkeletonGraphicDefaul_Straight : SkeletonGraphicDefault;
        }

        public static GameObject InstantiateSpineByDefname(string defname, string key = null, int layer = 2, bool loop = true, bool active = true, bool docuSaved = true, List<ProgramState> programStates = null)
        {
            AnimationDef def = DefDatabase<AnimationDef>.GetNamed(defname);
            if (def == null)
            {
                Log.Error($"[SpriteEvo] 无{defname}的AnimationDef");
                return null;
            }
            key ??= defname;
            ProgramStateFlags flag = (ProgramStateFlags)0;
            if (programStates == null) flag |= (ProgramStateFlags)ProgramState.Playing;
            else
            {
                foreach (ProgramState stat in programStates)
                {
                    flag |= (ProgramStateFlags)stat;
                }
            }
            return InstantiateSpine(def, key, layer, loop, active, docuSaved, flag);
        }

        public static GameObject InstantiateSpine(AnimationDef def, object key, int layer = 2, bool loop = true, bool active = true, bool docuSaved = true, ProgramStateFlags allowProgramStates = ProgramStateFlags.Playing)
        {
            if (((ProgramStateFlags)Current.ProgramState & allowProgramStates) == 0) return null; //游戏状况不允许
            //if (Current.ProgramState != ProgramState.Playing) return null;
            if (key == null) return null; //任何情况不允许空key
            if (docuSaved && ObjectManager.CurrentGameObjectDataBase.TryGetValue(key, out GameObject res))
            {
                res.SetActive(true);
                return res;
            }
            GameObject instance = Instantiate(def, layer, loop, active, DontDestroyOnLoad: false);
            if (instance != null && docuSaved)
            {
                ObjectManager.TryAddToCurrentGame(instance, key);
            }
            return instance;
        }

        public static GameObject Instantiate(AnimationDef def, int layer = 2, bool loop = true, bool active = true, bool DontDestroyOnLoad = false)
        {
            if (def == null) return null;
            GameObject instance = null;
            //Material material = SkeletonGraphicDefault;
            bool usePMA = def.mainAsset.asset.StraightAlphaInput;
            Material material = EnsureInitializedMaterialProperySource( usePMA);
            if (def.version == "3.8")
            {
                instance = Spine38Lib.NewSkeletonGraphic(def, material, layer, loop, active, DontDestroyOnLoad);
            }
            else if (def.version == "4.1")
            {
                instance = Spine41Lib.NewSkeletonGraphic(def, material, layer, loop, active, DontDestroyOnLoad);
            }
            else if (def.version == "4.2")
            {
                instance = Spine42Lib.NewSkeletonGraphic(def, material, layer, loop, active, DontDestroyOnLoad);
            }
            if (def.props.OnIMGUI || def.props.OnUGUI)
            {
                /*Transform transform;
                if (def.version == "3.8")
                {
                    transform = instance.Get
                }
                else if (def.version == "4.1")
                {
                    
                }
                if (def.props.uiAutoAlign && def.props.alignBone != null)
                {
                    Vector3 focus = 
                }*/
                instance.AddRenderCameraToCanvas(def.props.uioffset, (int)def.props.uiDrawSize.x, (int)def.props.uiDrawSize.y);
            }
            return instance;
        }

        public static Camera AddRenderCameraToCanvas(this GameObject instance, Vector3 uioffset, int width = 2048, int height = 2048)
        {
            //添加Camera
            if (instance == null) return null;
            instance.layer = 5;
            GameObject myGO = new GameObject("RenderCamera", new Type[] { typeof(Camera) });
            myGO.transform.SetParent(instance.transform);
            myGO.transform.localRotation = Quaternion.identity;
            myGO.transform.localPosition = uioffset; // X:0, Y:10, Z:-15

            Camera cam = myGO.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Color; //设置清除标志
            cam.cullingMask = 1 << 5; //剔除遮罩: UI层
            cam.fieldOfView = 60f;
            cam.backgroundColor = Color.clear;
            cam.useOcclusionCulling = true;
            cam.renderingPath = RenderingPath.Forward;
            cam.depth = Current.Camera.depth - 1;
            cam.targetTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32, 0);

            Canvas canvas = instance.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            CanvasScaler canvasScaler = instance.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = 1;
            canvas.referencePixelsPerUnit = 100;
            return cam;
        }

        ///<summary>[Pending] 在Canvas上渲染Spine动画 </summary>
        [Obsolete]
        public static GameObject Instantiate(AnimationDef def, Pawn pawn, bool loop = true)
        {
            /*
            Vector3 offset = new Vector3(def.props.offset.x, 0, pack.def.props.offset.y);
            Vector3 scale = new Vector3(pack.def.props.scale.x * 0.1f, 1f, pack.def.props.scale.y * 0.1f);
            GameObject obj = DynamicObjectDatabase.TryGetValue(pack.def.defName);
            if (obj != null)
            {
                Log.Error("[PA]. Duplicate Call :  Animation Instance  \"" + pack.def.defName + "\"  Existed in Hierarchy");
                return null;
            }
            GameObject myGO = new GameObject
            {
                name = "myGO"
            };
            Canvas myCanvas = myGO.AddComponent<Canvas>();
            Transform parent = myCanvas.transform;
            parent.position += offset;
            parent.localScale = scale;

            Material SkeletonGraphic_alpha = Spine_SkeletonGraphic;
            if (!pack.useStraightAlpha)
            {
                SkeletonGraphic_alpha.SetFloat("_StraightAlphaInput", 0);
                SkeletonGraphic_alpha.DisableKeyword("_STRAIGHT_ALPHA_INPUT");
                Log.Message("_STRAIGHT_ALPHA_INPUT OFF");
            }
            Spine38.Unity.SkeletonDataAsset skeleton = pack.Create_SkeletonDataAsset38();
            Spine38.Unity.SkeletonGraphic graphic = Spine38.Unity.SkeletonGraphic.NewSkeletonGraphicGameObject(skeleton, parent, SkeletonGraphic_alpha);

            graphic.allowMultipleCanvasRenderers = true;
            graphic.gameObject.layer = 5;
            //graphic.rectTransform.position += offset;
            //graphic.rectTransform.localScale = scale;
            graphic.rectTransform.rotation = Quaternion.Euler(90f, 0f, 0f);
            graphic.rectTransform.position = pawn.DrawPos + Vector3.up; ;
            graphic.AnimationState.SetAnimation(0, "Idle", loop);
            graphic.Initialize(overwrite: false);
            graphic.gameObject.SetActive(false);
            DynamicObjectDatabase.Add(pack.def.defName, graphic.gameObject);
            */
            return null;
        }
    }
}

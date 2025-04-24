using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>整合了初始化单个Spine骨骼动画对象的必要信息</summary>
    /*public class ThingDocument : IExposable, ILoadReferenceable
    {
        public string animationID;
        public SpineAssetDef spineassetdef;
        public Thing user;
        public Color32 color;
        public List<SlotSettings> slotSettings;
        public Vector3 offset;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string skin;
        public string currentAnimation;
        public bool loop;
        public float timeScale;
        public ThingDocument(string defName, Thing t, SpineAssetDef spineassetdef)
        {
            this.animationID = defName;
            this.user = t;
            this.spineassetdef = spineassetdef;
        }
        public void RecordSlotSettings() 
        {

        }
        public void ClearSlotSettings() 
        {
            if (slotSettings.NullOrEmpty()) return;
            foreach (SlotSettings setting in slotSettings) 
            {

            }
        }
        //Color32=> Value=RGBA(255, 255, 255, 255)
        public void ExposeData()
        {
            //Scribe_Values.Look(ref this.animationID, "animationID");
            Scribe_References.Look<Thing>(ref this.user, "user", true);

            Scribe_Values.Look<Color32>(ref this.color, "color", Color.white, true);
            Scribe_Collections.Look<SlotSettings>(ref this.slotSettings, "slotSettings", LookMode.Value);

            Scribe_Values.Look<Vector3>(ref this.offset, "localOffset", Vector3.zero, true);
            Scribe_Values.Look<Vector3>(ref this.position, "localPosition", Vector3.zero, true);
            Scribe_Values.Look<Vector3>(ref this.rotation, "localRotation", Vector3.zero, true);
            Scribe_Values.Look<Vector3>(ref this.scale, "scale", Vector3.one, true);
            Scribe_Values.Look(ref this.skin, "skin");
            Scribe_Values.Look(ref this.currentAnimation, "currentAnimation");
            Scribe_Values.Look(ref this.loop, "loop");
            Scribe_Values.Look(ref this.timeScale, "timeScale", 1f, true);

        }

        public string GetUniqueLoadID()
        {
            return this.animationID + "SpAnimationDoc";
        }
    }*/

    ///<summary>用来缓存动画信息的GC,在游戏中才可以使用</summary>
    public class GC_AnimationDocument : GameComponent
    {
        private bool isRegisterUpdated = false;

        public static GC_AnimationDocument instance;

        //key没有其他强引用时，这里面的key的键值对会在gc时被自动移除。但是gc不是实时的。
        //public ConditionalWeakTable<object, GameObject> ObjectDataBase = new();
        public ConditionalWeakTable<object, AnimationTracker> animationTrackerDocument = new();

        // 使用动画的小人的缓存
        private HashSet<Pawn> registedPawns = new();
        public Dictionary<Pawn, AnimationTracker> pawnTrackerDocument = new();


        public GC_AnimationDocument(Game game)
        {
            instance = this;
        }

        public override void GameComponentTick()
        {
            if (isRegisterUpdated == false) return;

            foreach (var pawn in registedPawns)
            {
                if (pawnTrackerDocument.ContainsKey(pawn) == false)
                {
                    if (animationTrackerDocument.TryGetValue(pawn, out AnimationTracker res))
                    {
                        CacheToDocument(pawn, res);
                    }
                    else
                    {
                        Instantiate(pawn);
                    }
                }
                //tracker?.Tick();
            }
            isRegisterUpdated = false;
        }

        public bool Contains(Pawn pawn) 
        {
            return registedPawns.Contains(pawn);
        }

        //必须通过这里来刷新状态
        public void Register(Pawn pawn) 
        {
            if (pawn == null) return;
            registedPawns.Add(pawn);
            isRegisterUpdated = true;
        }

        private void CacheToDocument(Pawn pawn, AnimationTracker tracker)
        {
            if (pawnTrackerDocument.ContainsKey(pawn)) return;
            pawnTrackerDocument.Add(pawn, tracker);
        }

        //test
        private void Instantiate(Pawn pawn, string defName = "Chang_An_Test")
        {
            AnimationDef def = DefDatabase<AnimationDef>.GetNamed(defName);
            if (def == null) return;
            ProgramStateFlags flag = (ProgramStateFlags)0;
            flag |= (ProgramStateFlags)ProgramState.Playing;
            GameObject obj = SkeletonAnimationUtility.InstantiateSpine(def, pawn, allowProgramStates: flag);
            obj.transform.position = pawn.DrawPos + Vector3.up;//debug
            obj.SetActive(true);
        }
    }
}

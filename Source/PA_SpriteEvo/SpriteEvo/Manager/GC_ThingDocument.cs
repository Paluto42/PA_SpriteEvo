using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    ///<summary>整合了初始化单个Spine骨骼动画对象的必要信息</summary>
    public class ThingDocument : IExposable, ILoadReferenceable
    {
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
            return this.user.GetHashCode() + "SpAnimationDoc";
        }
    }
    public class GC_ThingDocument : GameComponent
    {
        public static Dictionary<object, GameObject> ObjectDataBase;
        public static Dictionary<string, ThingDocument> animationDataBase;

        //internal static HashSet<Thing> cachedThings = new HashSet<Thing>();

        public GC_ThingDocument(Game game)
        {
            ObjectDataBase = new Dictionary<object, GameObject>();
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            if (ModLister.GetActiveModWithIdentifier("PA.SpriteEvo") != null) 
            {
                Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter(Translator.Translate("AK_StartLabel"), Translator.Translate("AK_StartDesc"), LetterDefOf.NeutralEvent, null, null));
            }
            ObjectDataBase ??= new Dictionary<object, GameObject>();
        }

        public override void FinalizeInit()
        {
            List<string> key = new List<string>();
            List<ThingDocument> value = new List<ThingDocument>();
            Scribe.mode = LoadSaveMode.ResolvingCrossRefs;
            try
            {
                Scribe_Collections.Look(ref animationDataBase, "animationDocoment", LookMode.Value, LookMode.Deep, ref key, ref value);
            }
            catch 
            { Log.Error("Failed to save AnimationDoc"); }
            //复原
            Scribe.mode = LoadSaveMode.Inactive;
        }
        public override void LoadedGame()
        {
            base.LoadedGame();
            ObjectDataBase ??= new Dictionary<object, GameObject>();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode != LoadSaveMode.ResolvingCrossRefs)
            {
                List<string> key = new List<string>();
                List<ThingDocument> value = new List<ThingDocument>();
                try 
                {
                    Scribe_Collections.Look(ref animationDataBase, "animationDocoment", LookMode.Value, LookMode.Deep, ref key, ref value);
                }
                catch { Log.Error("Failed to save AnimationDoc"); }
            }
        }
        public static void Add(object key, GameObject value)
        {
            if (ObjectDataBase.ContainsKey(key)) 
            {
                Log.Error("SpriteEvo. Error while Adding new Value: The same Key already exists in ObjectDatabase"); 
                return; 
            }
            else
            {
                ObjectDataBase.Add(key, value);
            }
        }
        //May be Null
        public static GameObject TryGetRecord(object key)
        {
            if (ObjectDataBase.ContainsKey(key)) 
            {
                return ObjectDataBase[key]; 
            }
            else 
            {
                return null;
            }

        }
    }
}

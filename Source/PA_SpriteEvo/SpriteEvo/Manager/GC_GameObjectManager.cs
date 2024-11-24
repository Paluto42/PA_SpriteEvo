using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    //暂时好像没用
    public class GameObjectDocument : IExposable, ILoadReferenceable
    {
        public string objectID;
        public bool currentExist;
        public GameObject document;


        public void ExposeData()
        {
        }

        public string GetUniqueLoadID()
        {
            return this.objectID + "ObjDoc";
        }
    }
    public class GC_GameObjectManager : GameComponent
    {
        public static Dictionary<object, GameObject> ObjectDatabase;

        //internal static HashSet<Thing> cachedThings = new HashSet<Thing>();

        public GC_GameObjectManager(Game game)
        {
            ObjectDatabase = new Dictionary<object, GameObject>();
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            if (ModLister.GetActiveModWithIdentifier("PA.SpriteEvo") != null) 
            {
                Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter(Translator.Translate("AK_StartLabel"), Translator.Translate("AK_StartDesc"), LetterDefOf.NeutralEvent, null, null));
            }
            ObjectDatabase ??= new Dictionary<object, GameObject>();
        }

        public override void FinalizeInit()
        {
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            ObjectDatabase ??= new Dictionary<object, GameObject>();
        }
        public override void GameComponentTick() { }

        public override void ExposeData()
        {
            base.ExposeData();
        }
        public static void Add(object key, GameObject value)
        {
            if (ObjectDatabase.ContainsKey(key)) 
            {
                Log.Error("SpriteEvo. Error while Adding new Value: The same Key already exists in ObjectDatabase"); 
                return; 
            }
            else
            {
                ObjectDatabase.Add(key, value);
            }
        }
        //May be Null
        public static GameObject TryGetRecord(object key)
        {
            if (ObjectDatabase.ContainsKey(key)) 
            {
                return ObjectDatabase[key]; 
            }
            else 
            {
                return null;
            }

        }
    }
}

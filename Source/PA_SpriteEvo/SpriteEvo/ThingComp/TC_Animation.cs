using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public class TCP_Animation : CompProperties
    {
        public AnimationDef animation;

        public string skin = "default";
        public string idleAnimation = "Idle";
        public float timeScale = 1f;

        public TCP_Animation() 
        {
            this.compClass = typeof(TC_Animation);
        }
    }
    public class TC_Animation : ThingComp
    {
        public TCP_Animation Props => props as TCP_Animation;
        public AnimationDef Animation => Props.animation;
        public string Skin => Props.skin;
        public string IdleAnimation => Props.idleAnimation;
        public float TimeScale => Props.timeScale;

        private GameObject instance;
        public virtual bool ShouldInstantiate
        {
            get
            {
                return parent is Pawn user && user.Spawned && !user.Destroyed && !user.Dead && !user.Downed;
            }
        }
        public virtual void InstantiateAnimation() 
        {
            if (!ShouldInstantiate) return;
            if (Props == null) return;
            if (Animation == null || Skin == null || IdleAnimation == null) return;
            instance ??=SkeletonAnimationUtility.InstantiateSpine(Animation, parent);
        }
        public virtual void InitializeAnimation() 
        {
            if (instance == null) return;
            instance.transform.rotation = Quaternion.Euler(90f, 0, 0);
            instance.SetActive(true);
        }
        public override void Initialize(CompProperties props) 
        {
            base.Initialize(props);
        }
        public override void PostPostMake()
        {
            InstantiateAnimation();
            InitializeAnimation();
        }
        public override void DrawGUIOverlay()
        {
            if (instance == null || parent == null) return;
            instance.transform.position = parent.DrawPos;
        }
        public override void Notify_BecameInvisible()
        {
            instance?.SetActive(false);
        }
        public override void Notify_BecameVisible()
        {
            instance?.SetActive(true);
        }
        public override void CompDrawWornExtras()
        {
        }
        public override void PostDraw()
        {
            instance?.SetActive(true);
            instance.transform.rotation = Quaternion.Euler(90f, 0 , 0);
            instance.transform.position = parent.DrawPos;
        }
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (previousMap != null) 
            {
                GameObject.Destroy(instance);
            }
        }
        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            base.Notify_Killed(prevMap, dinfo);
        }
    }
}

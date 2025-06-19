using Spine42;
using Spine42.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteEvo
{
#if !RELEASE_BUILD
    public enum SlotRotation
    {
        None,
        Front,
        Back,
        Side
    }
    public class ACP_PawnController : ScriptProperties
    {
        public List<TrackSet> trackSets = new();
        public List<AnimationDriverDef> animations = new();
        //animations.OrderByDescending(driver => driver.priority).ToList();

        public ACP_PawnController()
        {
            scriptClass = typeof(AC_PawnController);
        }
    }

    public class AC_PawnController : PawnBaseController<SkeletonAnimation>, IPawnTransform
    {
        ACP_PawnController Props => props as ACP_PawnController;
        List<TrackSet> TrackSets => Props.trackSets;

        // 用于存储不同轨道的动画队列
        private readonly Dictionary<int, TrackSet> trackQueues = new();
        public ISkeletonComponent ISkeletonComponent => SkeletonInstanceInt;
        public IAnimationStateComponent IAnimationStateComponent => SkeletonInstanceInt;

        // 槽位属性
        #region
        private SlotRotation LastRotation = SlotRotation.None;
        private readonly ExposedList<Slot> frontSlotInt = new();
        private readonly ExposedList<Slot> backSlotInt = new();
        private readonly ExposedList<Slot> sideSlotInt = new();
        #endregion

        protected override void Awake()
        {
            base.Awake();
            InitializeSlotInfo();
            InitializeQueueInfo();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (var queueSet in trackQueues)
            {
                StartCoroutine(AnimationQueueHandlerTest(queueSet));
            }
        }

        public override void DoMove()
        {
            UpdateRotation();
            UpdatePosition();
        }

        public virtual void UpdatePosition()
        {
            transform.position = Owner.DrawPos + (Vector3.up * drawDepth);
        }

        public virtual void UpdateRotation()
        {
            switch (Owner.Rotation.AsInt)
            {
                case 0: FaceNorth(); break;
                case 1: FaceEast(); break;
                case 2: FaceSouth(); break;
                case 3: FaceWest(); break;
            }
        }

        //旋转
        #region
        public virtual void FaceNorth()
        {
            if (LastRotation == SlotRotation.Back) return;
            SetSlotVisible(frontSlotInt, false);
            SetSlotVisible(backSlotInt, true);
            SetSlotVisible(sideSlotInt, false);
            LastRotation = SlotRotation.Back;
        }

        public virtual void FaceEast()
        {
            if (LastRotation != SlotRotation.Side)
            {
                SetSlotVisible(frontSlotInt, false);
                SetSlotVisible(backSlotInt, false);
                SetSlotVisible(sideSlotInt, true);
            }
            ISkeletonComponent.DoFlipX(false);
            LastRotation = SlotRotation.Side;
        }

        public virtual void FaceSouth()
        {
            if (LastRotation == SlotRotation.Front) return;
            SetSlotVisible(frontSlotInt, true);
            SetSlotVisible(backSlotInt, false);
            SetSlotVisible(sideSlotInt, false);
            LastRotation = SlotRotation.Front;
        }

        public virtual void FaceWest()
        {
            if (LastRotation != SlotRotation.Side)
            {
                SetSlotVisible(frontSlotInt, false);
                SetSlotVisible(backSlotInt, false);
                SetSlotVisible(sideSlotInt, true);
            }
            ISkeletonComponent.DoFlipX(true);
            LastRotation = SlotRotation.Side;
        }
        #endregion

        //槽位
        #region
        protected virtual void InitializeSlotInfo()
        {
            if (ISkeletonComponent == null) return;
            var list = ISkeletonComponent.Skeleton.Slots;
            foreach (var slot in list)
            {
                string slotName = slot.Data.Name;
                if (slotName.StartsWith("F_")) frontSlotInt.Add(slot);
                else if (slotName.StartsWith("B_")) backSlotInt.Add(slot);
                else if (slotName.StartsWith("S_")) sideSlotInt.Add(slot);
            }
        }

        public virtual void SetSlotVisible(ExposedList<Slot> targetSlots, bool visible = true)
        {
            float alpha = visible ? 1 : 0;
            targetSlots.ForEach(slot => slot.A = alpha);
        }
        #endregion

        public void InitializeQueueInfo()
        {
            foreach (TrackSet item in TrackSets)
            {
                trackQueues.Add(item.index, item);
            }
        }

        public virtual IEnumerator AnimationQueueHandlerTest(KeyValuePair<int, TrackSet> set)
        {
            int index = set.Key;
            TrackSet queueSet = set.Value;
            //大量的new Yield实例会导致内存性能问题
            WaitForSeconds waitForSeconds = new(queueSet.interval);
            bool shouldRefresh = queueSet.interval > 0;
            do
            {
                if (shouldRefresh)
                {
                    yield return waitForSeconds;
                    IAnimationStateComponent.AnimationState.SetEmptyAnimation(index, 0.2f);
                }
                foreach (TrackEntry item in TestQueueTrack(queueSet))
                {
                    yield return item;
                }
            }
            while (shouldRefresh);
        }

        public IEnumerable<TrackEntry> TestQueueTrack(TrackSet set)
        {
            int index = set.index;
            List<TrackSet.TrackQueue> queues = set.queues;
            foreach (TrackSet.TrackQueue item in queues)
            {
                TrackEntry track = IAnimationStateComponent.AnimationState.AddAnimation(index, item.animation, item.loop, item.delay);
                track.MixBlend = item.blendMode.ParseTo<MixBlend>();
                yield return track;
            }
        }
    }
#endif
}

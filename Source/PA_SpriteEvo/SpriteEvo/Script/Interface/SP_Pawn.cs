using Spine42;
using Spine42.Unity;
using UnityEngine;
using Verse;

namespace SpriteEvo
{
    public enum SlotRotation
    {
        None,
        Front,
        Back,
        Side
    }

    public class ASP_PawnBase : ScriptProperties
    {
        public ASP_PawnBase()
        {
            scriptClass = typeof(AS_PawnBase);
        }
    }

    public class AS_PawnBase : ScriptBase, ITransform
    {
        #region Unity
        Color clearColor = Color.clear;

        ISkeletonComponent skeletonComp;
        IAnimationStateComponent animationStateComp;

        ExposedList<Slot> frontSlotInt = new();
        ExposedList<Slot> backSlotInt = new();
        ExposedList<Slot> sideSlotInt = new();

        Pawn ownerInt;
        #endregion
        SlotRotation lastRot = SlotRotation.None;
        Pawn Owner
        {
            get
            {
                if (ownerInt != null){
                    return ownerInt;
                }
                ownerInt = referenceKey as Pawn;
                return ownerInt;
            }
        }

        int count = 0;
        float blinkMixIn = 0.4f;
        float blinkMixOut = 0.833f;

        public override void OnEnable()
        {
            skeletonComp ??= GetComponent<ISkeletonComponent>();
            animationStateComp ??= GetComponent<IAnimationStateComponent>();
            InitializeRotationSlots();
            /*var list = skeletonComp.Skeleton.Slots.FindAll(slot => slot.Data.Name.StartsWith("S_") || slot.Data.Name.StartsWith("B_"));
            foreach (var slot in list)
            {
                slot.SetColor(clearColor);
                Debug.Log(slot.Data.Name);
            }*/
            if (animationStateComp == null) return;
            Log.Message("可以播放动画");
            TrackEntry track0 = animationStateComp.AnimationState.SetAnimation(0, "idle", false);
            track0.Complete += CompleteEventHandler;
        }

        public override void Update()
        {
            if (Owner == null) return;
            Rot4 rot = Owner.Rotation;
            switch (rot.AsInt)
            {
                case 0:
                    PoseNorth();
                    break;
                //右 east
                case 1:
                    PoseEast();
                    break;
                case 2:
                    PoseSouth();
                    break;
                case 3:
                    PoseWest();
                    break;
            }
            Move();
        }

        public void PoseSouth()
        {
            if (lastRot == SlotRotation.Front) return;
            SetRotationVisibility(frontSlotInt, true);
            SetRotationVisibility(backSlotInt, false);
            SetRotationVisibility(sideSlotInt, false);
            lastRot = SlotRotation.Front;
        }

        public void PoseNorth()
        {
            if (lastRot == SlotRotation.Back) return;
            SetRotationVisibility(frontSlotInt, false);
            SetRotationVisibility(backSlotInt, true);
            SetRotationVisibility(sideSlotInt, false);
            lastRot = SlotRotation.Back;
        }

        public void PoseWest()
        {
            if (lastRot != SlotRotation.Side)
            {
                SetRotationVisibility(frontSlotInt, false);
                SetRotationVisibility(backSlotInt, false);
                SetRotationVisibility(sideSlotInt, true);
            }
            skeletonComp.DoFlipX(true);
            lastRot = SlotRotation.Side;
        }

        public void PoseEast()
        {
            if (lastRot != SlotRotation.Side)
            {
                SetRotationVisibility(frontSlotInt, false);
                SetRotationVisibility(backSlotInt, false);
                SetRotationVisibility(sideSlotInt, true);
            }
            skeletonComp.DoFlipX(false);
            lastRot = SlotRotation.Side;
        }

        public void Move()
        {
            if (Owner != null){
                transform.position = Owner.DrawPos;
            }
        }

        private void InitializeRotationSlots()
        {
            if (animationStateComp == null) return;
            var list = skeletonComp.Skeleton.Slots;
            foreach (var slot in list)
            {
                string slotName = slot.Data.Name;
                if (slotName.StartsWith("F_")) frontSlotInt.Add(slot);
                else if (slotName.StartsWith("B_")) backSlotInt.Add(slot);
                else if (slotName.StartsWith("S_")) sideSlotInt.Add(slot);
            }
        }

        private void SetRotationVisibility(ExposedList<Slot> targetSlots, bool visible)
        {
            Color showColor = visible ? Color.white : clearColor;
            targetSlots.ForEach(s => s.SetColor(showColor));
        }

        private void CompleteEventHandler(TrackEntry trackEntry)
        {
            count++;
            if (count == 1)
            {
                TrackEntry track1 = animationStateComp.AnimationState.AddAnimation(1, "blink", false, blinkMixIn);
                track1.Complete += CompleteEventHandler;
                return;
            }
            if (count >= 2)
            {
                count = 0;
                TrackEntry track1 = animationStateComp.AnimationState.AddAnimation(0, "idle", false, 0);
                track1.Complete += CompleteEventHandler;
                TrackEntry track2 = animationStateComp.AnimationState.AddAnimation(1, "idle", false, blinkMixOut);
            }
        }
    }
}

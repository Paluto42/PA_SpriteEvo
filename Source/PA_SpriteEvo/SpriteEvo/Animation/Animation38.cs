using Spine38;
using Spine38.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteEvo
{
    //写这个类是为了更加便捷地访问动画的属性,全是Getter
    /*public class Animation38 : ITransform, ISkeletonConfig
    {
        private readonly SkeletonAnimation _skeletonAnimation;
        private readonly Camera renderCam;
        private GameObject RootObject => _skeletonAnimation?.gameObject;
        public bool Active 
        { 
            get => RootObject.activeInHierarchy; 
            set => RootObject.SetActive(value); 
        }
        public Skeleton Skeleton => _skeletonAnimation?.Skeleton;
        public RenderTexture RenderTexture => renderCam?.targetTexture;

        public Animation38(SkeletonAnimation instance)
        {
            _skeletonAnimation = instance ?? throw new ArgumentNullException("Failed to Instantiate Animation38");
        }
        public Animation38(SkeletonAnimation instance, Vector3 camoffset, Vector2 drawSize)
        {
            _skeletonAnimation = instance ?? throw new ArgumentNullException("Failed to Instantiate Animation38");
            renderCam = _skeletonAnimation.gameObject.AddRenderCameraToSkeletonAnimation(camoffset, (int)drawSize.x, (int)drawSize.y);
        }
        public Vector3 Position => RootObject.transform.position;
        public Quaternion Rotation => RootObject.transform.rotation;
        public Vector3 Scale => RootObject.transform.localScale;
        public void SetPositon(Vector3 pos) 
        { 
            RootObject.transform.position = pos;
        }
        public void SetRotation(Vector3 rot) 
        {
            RootObject.transform.rotation = Quaternion.Euler(rot); 
        }
        public void SetScale(Vector3 s) 
        {
            RootObject.transform.localScale = s; 
        }
        public void DoFlipX(bool boolean) 
        {
            Skeleton.ScaleX = boolean ? -1f : 1f;
        }
        public void DoFlipY(bool boolean) 
        {
            Skeleton.ScaleY = boolean ? -1f : 1f;
        }
        public void ApplyColor(Color color, List<SlotSettings> slotSettings)
        {
            if (_skeletonAnimation == null) return;
            ISkeletonComponent skeletonComponent = _skeletonAnimation;
            if (skeletonComponent != null)
            {
                Skeleton skeleton = skeletonComponent.Skeleton;
                SkeletonExtensions.SetColor(skeleton, color);
                foreach (SlotSettings s in slotSettings)
                {
                    Slot slot = skeleton.FindSlot(s.slot);
                    slot?.SetColor(color);
                }
            }
        }
        public void SetTimeScale(float scale)
        {
            if (_skeletonAnimation == null) return;
            _skeletonAnimation.timeScale = scale;
        }
        public void ReloadSkeleton(string initialSkinName)
        {
            if (_skeletonAnimation == null) return;
            _skeletonAnimation.initialSkinName = initialSkinName;
            _skeletonAnimation.Initialize(overwrite: true);
        }
        public void UpdateSkin(string skinName, bool resetBones = true)
        {
            if (_skeletonAnimation?.SkeletonDataAsset == null)
                return;
            this.Skeleton.SetSkin(skinName);
            if (resetBones)
                this.Skeleton.SetBonesToSetupPose();
            this.Skeleton.SetSlotsToSetupPose();
        }

    }*/
}

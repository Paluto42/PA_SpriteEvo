using UnityEngine;

namespace SpriteEvo
{
    /// <summary>
    /// 相当于ThingComp的用法
    /// </summary>
    public class ScriptBase : MonoBehaviour
    {
        public object referenceKey;
        public GameObject Parent => base.gameObject;

        public ScriptProperties props;

        /***** 初始化函数 *****/
        // 挂载脚本实例时调用Awake 适合拿来初始化引用
        public virtual void Awake() { }
        // 当对象已启用并处于活动状态时调用OnEnable
        public virtual void OnEnable() { }
        // 仅在首次调用Update之前调用Start
        public virtual void Start() { }

        /***** 更新函数 *****/
        // 如果启用MonoBehavioir,则每个固定帧速率的帧都将调用FixedUpdate
        public virtual void FixedUpdate() { }
        // 如果启用MonoBehavioir,则在每一帧都调用Update
        public virtual void Update() { }
        // 如果启用MonoBehavioir,则在每一帧都调用LateUpdate
        public virtual void LateUpdate() { }

        /***** 渲染函数 *****/
        // 在照相机裁剪场景前调用OnPreCull 裁剪操作将确定摄像机可以看到哪些对象
        public virtual void OnPreCull()  { }
        // 对象变为对任何摄像机可见/不可见时调用
        public virtual void OnBecameVisible() { }
        public virtual void OnBecameInvisible() { }
        // 如果对象可见，则为每个摄像机调用一次
        public virtual void OnWillRenderObject() { }
        // 在摄像机开始渲染场景之前调用
        public virtual void OnPreRender() { }
        // 所有常规场景渲染完成之后调用,此时，可以使用 GL 类或 Graphics.DrawMeshNow 来绘制自定义几何形状
        public virtual void OnRenderObject() { }
        //在摄像机完成场景渲染后调用
        public virtual void OnPostRender() { }
        // 在场景渲染完成后调用以允许对图像进行后处理
        //public virtual void OnRenderImage() { }
        // 每帧调用多次以响应 GUI 事件。首先处理布局和重新绘制事件，然后为每个输入事件处理布局和键盘/鼠标事件
        public virtual void OnGUI() { }
        // 用于在场景视图中始终绘制gizmos以实现可视化
        public virtual void OnDrawGizmos() { }

        /***** 退出函数 *****/
        // 在退出应用程序之前在所有游戏对象上调用此函数
        public virtual void OnApplicationPause() { }
        public virtual void OnApplicationQuit() { }
        //行为被禁用时调用
        public virtual void OnDisable() { }

        // 被销毁时调用
        public virtual void OnDestroy() { }
    }
}

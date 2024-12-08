using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpriteEvo
{
    /// <summary>
    /// 相当于ThingComp的用法
    /// </summary>
    public class CompatibleMonoBehaviour : MonoBehaviour
    {
        public GameObject Parent => base.gameObject;

        public CompatibleMonoBehaviourProperties props;
        public void Enable() { this.enabled = true; }
        public void Disable() { this.enabled = false; }
        //添加后调用
        public virtual void Awake()
        {
        }
        //物件被激活时调用
        public virtual void OnEnable()
        {
        }
        // Start is called before the first frame update
        //这玩意只有第一次激活调用 适合拿来初始化对象
        public virtual void Start()
        {
        }
        public virtual void FixedUpdate()
        {
        }
        // Update is called once per frame
        public virtual void Update()
        {
        }
        public virtual void LateUpdate()
        {
        }
        public virtual void OnGUI()
        {
        }
        //物件被禁用时调用
        public virtual void OnDisable()
        {
        }
        public virtual void OnDestory()
        {
        }
    }
}

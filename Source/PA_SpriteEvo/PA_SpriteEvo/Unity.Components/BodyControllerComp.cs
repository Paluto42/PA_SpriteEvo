using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PA_SpriteEvo
{
    public class BodyControllerComp : BaseControllerComp
    {
        //在Unity编辑器里直接使用需要把属性换成字段
        #region Inspector
        public Rot4 rot;
        public GameObject FrontClothes { get; set; }
        public GameObject BackClothes { get; set; }
        #endregion

        public override void Awake()
        {
        }
        public override void OnEnable()
        {
            FrontClothes?.SetActive(true);
            BackClothes?.SetActive(true);
        }
        // Start is called before the first frame update
        public override void Start()
        {
        }
        // Update is called once per frame
        public override void Update()
        {
        }
        public override void OnDisable()
        {
            FrontClothes?.SetActive(false);
            BackClothes?.SetActive(false);
        }
    }
}

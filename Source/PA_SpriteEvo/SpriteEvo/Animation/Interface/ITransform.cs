using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpriteEvo
{
    public interface ITransform
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        Vector3 Scale { get; }
        void SetPositon(Vector3 pos);
        void SetRotation(Vector3 rot);
        void SetScale(Vector3 s);
    }
}

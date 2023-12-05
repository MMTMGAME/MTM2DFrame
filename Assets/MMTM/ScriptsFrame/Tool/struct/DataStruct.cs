using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMTM
{
    public struct TransformData
    {
        public Vector3 localposition;
        public Quaternion localrotation;
        public Vector3 localscale;
        public Vector3 position;
        public Quaternion rotation;
        
        public TransformData(Transform transform)
        {
            localposition = transform.localPosition;
            localrotation = transform.localRotation;
            localscale = transform.localScale;
            position = transform.position;
            rotation = transform.localRotation;
        }
    }
}
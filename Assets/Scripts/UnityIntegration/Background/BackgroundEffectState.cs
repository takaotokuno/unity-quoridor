using UnityEngine;

namespace Quoridor
{
    [System.Serializable]
    public struct BackgroundEffectState
    {
        public Color BaseColorA;
        public Color BaseColorB;
        public float NoiseScale;
        public float DistortionStrength;
        public float FlowSpeed;
        public float EmissionStrength;
        public float Alpha;
    }
}

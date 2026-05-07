using UnityEngine;

namespace VivifyBaker.Baker.Scripts.Bakers.MaterialBaker
{
    public struct MaterialBakeSettings
    {
        public AnimationClip Clip;
        public string MaterialName;
        public string[] PropertyNames;
        public string ObjectName;
        public float BPM;
        public float StartBeatOffset;
    }
}
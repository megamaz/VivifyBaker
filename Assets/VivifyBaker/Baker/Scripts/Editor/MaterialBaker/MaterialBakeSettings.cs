using UnityEngine;

namespace VivifyBaker.Baker.Scripts.Editor.MaterialBaker
{
    public struct MaterialBakeSettings
    {
        public AnimationClip Clip;
        public string MaterialName;
        public string[] PropertyNames;
        public string ObjectName;
        public int SamplesPerSecond;
        public float BPM;
        public float StartBeatOffset;
    }
}
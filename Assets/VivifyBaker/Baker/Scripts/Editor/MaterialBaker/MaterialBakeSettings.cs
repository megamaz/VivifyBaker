using UnityEngine;

namespace VivifyBaker.Baker.Scripts.Editor.MaterialBaker
{
    public class MaterialBakeSettings
    {
        public AnimationClip Clip;
        public string MaterialName;
        public string[] PropertyNames;
        public int SamplesPerSecond;
        public float BPM;
        public float StartBeatOffset;
    }
}
using UnityEditor;
using UnityEngine;

namespace VivifyBaker.Baker.Scripts.Editor.Utility
{
    public static class AnimationSampler
    {
        public static float GetPropertyValueAtFrame(AnimationClip clip, string propertyName, string relativePath, System.Type componentType, int frame)
        {
            float time = frame / clip.frameRate;

            EditorCurveBinding binding = EditorCurveBinding.FloatCurve(relativePath, componentType, propertyName);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            if (curve == null)
                return 0f;

            return curve.Evaluate(time);
        }
    }
}
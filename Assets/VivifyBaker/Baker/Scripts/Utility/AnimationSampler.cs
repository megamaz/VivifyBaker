using UnityEditor;
using UnityEngine;

namespace VivifyBaker.Baker.Scripts.Utility
{
    public static class AnimationSampler
    {
        public static float GetPropertyValueAtFrame(AnimationClip clip, string propertyName, string relativePath, System.Type componentType, int frame)
        {
            // does not work when outside Unity Editor, causing errors on VivifyTemplate build.
            #if UNITY_EDITOR
            float time = frame / clip.frameRate;

            EditorCurveBinding binding = EditorCurveBinding.FloatCurve(relativePath, componentType, propertyName);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            if (curve == null)
                return 0f;

            return curve.Evaluate(time);
            
            #else
            return 0;
            #endif
        }
    }
}
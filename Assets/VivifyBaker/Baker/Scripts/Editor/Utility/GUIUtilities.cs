using System;
using JetBrains.Annotations;
using UnityEngine;

namespace VivifyBaker.Baker.Scripts.Editor.Utility
{
    public static class GUIUtilities
    {
        // reused pretty often
        public static void GUIBake([CanBeNull] Action callback)
        {
            if (GUILayout.Button("Bake", GUILayout.Height(40)) &&  callback != null)
            {
                callback.DynamicInvoke();
            }
        }
    }
}
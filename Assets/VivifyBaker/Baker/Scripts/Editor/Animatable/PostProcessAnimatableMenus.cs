using UnityEditor;
using UnityEngine;
using VivifyBaker.Baker.Scripts.Components.Animatable;

namespace VivifyBaker.Baker.Scripts.Editor.Animatable
{
    [CustomEditor(typeof(PostProcessAnimationController))]
    public class PostProcessAnimationControllerDraw : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            PostProcessAnimationController script =  (PostProcessAnimationController)target;

            if (GUILayout.Button("Add Layer"))
            {
                script.AddLayer();
            }
        }
    }

    public class CustomActionMenu
    {
        [MenuItem("GameObject/VivifyBaker/Create Post Process Controller", false, 10)]
        private static void CreatePostProcessController(MenuCommand menuCommand)
        {
            GameObject click_obj = menuCommand.context as GameObject;
            GameObject new_controller = new GameObject("PostProcessController");
            new_controller.transform.SetParent(click_obj.transform);
            new_controller.AddComponent<PostProcessAnimationController>();
            Selection.activeGameObject = new_controller;
        }
    }
}
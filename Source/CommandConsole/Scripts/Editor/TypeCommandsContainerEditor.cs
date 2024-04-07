#if UNITY_EDITOR && DevConsole
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace Game.DevConsole
{
    [CustomEditor(typeof(TypeCommandsContainer))]
    internal sealed class TypeCommandsContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TypeCommandsContainer yourComponent = (TypeCommandsContainer)target;

            if (GUILayout.Button("Compile Commands"))
            {
                MethodInfo methodInfo = typeof(TypeCommandsContainer).GetMethod("CompileCommands", BindingFlags.NonPublic | BindingFlags.Instance);

                methodInfo.Invoke(yourComponent, null);
            }
        }
    }
}
#endif
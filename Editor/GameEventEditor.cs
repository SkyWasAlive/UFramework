using UFramework.GameEvents;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GameEvent gameEvent = (GameEvent)target;
            if (GUILayout.Button("Raise Event"))
            {
                gameEvent.Raise();
            }
        }
    }
}

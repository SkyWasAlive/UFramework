using System.Collections.Generic;
using UFramework.GameEvents;
using UnityEditor;
using UnityEngine;

public class EventListnnerInspectorWindow : EditorWindow
{

    private GameObject selectedObject;
    private int pageIndex = 0;
    private Dictionary<string, bool> responseFoldoutStates = new Dictionary<string, bool>();
    private Dictionary<string, bool> selectedEvents = new Dictionary<string, bool>(); 
    private bool[] showRenameField; // Array to store visibility state of each rename field
    private string[] newEventNames;  // Array to store new names for each GameEvent
    
    
    [MenuItem("Window/Game Event/Game Event Listener Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EventListnnerInspectorWindow));
    }
    

    void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }

    void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    void OnGUI()
    {
        titleContent.text = "Game Event Editor";
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Game Event Listeners", GUI.skin.button))
        {
            pageIndex = 0;
        };
        if (GUILayout.Button("Game Events", GUI.skin.button))
        {
            pageIndex = 1;
        };
        GUILayout.EndHorizontal();
        
        if (pageIndex == 0)
        {
            GUILayout.Space(10);
            if (selectedObject == null)
            {
                GUILayout.Label("No GameObject Selected!");
            }
            else
            {
                DisplayEventListeners(selectedObject);
            }
        }

        if (pageIndex == 1)
        {
            DisplayAllGameEvents();
        }
    }

   private void DisplayAllGameEvents()
    {
        GameEvent[] gameEvents = Resources.LoadAll<GameEvent>("Game Events");

        if (showRenameField == null || showRenameField.Length != gameEvents.Length)
        {
            showRenameField = new bool[gameEvents.Length];
            newEventNames = new string[gameEvents.Length];
        }

        // Display a label if no game events are found
        if (gameEvents.Length == 0)
        {
            GUILayout.Label("No Game Events found.");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); 
            if (GUILayout.Button("Create New Event", GUILayout.Width(150)))
            {
                CreateNewGameEvent();
            }
            GUILayout.EndHorizontal();
            }
        else
        {
            for (int i = 0; i < gameEvents.Length; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(gameEvents[i].name);
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    RemoveGameEvent(gameEvents[i]);
                }
                if (GUILayout.Button("Rename", GUILayout.Width(80)))
                {
                    showRenameField[i] = !showRenameField[i];
                    newEventNames[i] = gameEvents[i].name;
                }
                GUILayout.EndHorizontal();
                if (showRenameField[i])
                {
                    GUILayout.BeginHorizontal();
                    newEventNames[i] = GUILayout.TextField(newEventNames[i]);
                    if (GUILayout.Button("Apply", GUILayout.Width(80)))
                    {
                        RenameGameEvent(gameEvents[i], newEventNames[i]);
                        showRenameField[i] = false;
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); 
        if (GUILayout.Button("Create New Event", GUILayout.Width(150)))
        {
            CreateNewGameEvent();
        }
        GUILayout.EndHorizontal();
    }

    private void CreateNewGameEvent()
    {
        GameEvent newGameEvent = CreateInstance<GameEvent>();
        string path = "Assets/Resources/Game Events/NewGameEvent.asset"; 
        AssetDatabase.CreateAsset(newGameEvent, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void RenameGameEvent(GameEvent gameEvent, string newName)
    {
        // Ensure the new name is not empty
        if (!string.IsNullOrEmpty(newName))
        {
            // Check if the new name is different from the current name
            if (newName != gameEvent.name)
            {
                // Rename the GameEvent asset
                string oldAssetPath = "Assets/Resources/Game Events/" + gameEvent.name + ".asset";
                string newAssetPath = "Assets/Resources/Game Events/" + newName + ".asset";
                AssetDatabase.RenameAsset(oldAssetPath, newName);
                AssetDatabase.SaveAssets();

                // Update the name of the GameEvent object
                gameEvent.name = newName;
            }
            else
            {
                Debug.LogWarning("New name is same as old name.");
            }
        }
        else
        {
            Debug.LogWarning("New name cannot be empty.");
        }
    }


    private void RemoveGameEvent(GameEvent gameEvent)
    {
        // Remove the GameEvent asset from the Resources directory
        string assetPath = "Assets/Resources/Game Events/" + gameEvent.name + ".asset";
        AssetDatabase.DeleteAsset(assetPath);
    }

    private void DisplayEventListeners(GameObject gameObject)
    {
        GameEventListener[] eventListeners = gameObject.GetComponents<GameEventListener>();

        if (eventListeners.Length > 0)
        {
            // Begin grouping the event listeners
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Event Listeners Registered on the Object", EditorStyles.boldLabel);
            GUILayout.Space(20);
            SerializedObject serializedObject;
            foreach (GameEventListener gameEventListener in eventListeners)
            {
                serializedObject = new SerializedObject(gameEventListener);
                serializedObject.Update();

                SerializedProperty iterator = serializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    
                    // Skip properties named "m_Script" and "m_Name"
                    if (iterator.name == "m_Script" || iterator.name == "m_Name")
                    {
                        continue;
                    }

                    // Draw collapsible element for "Response" property
                    if (iterator.name == "Response")
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(30);
                        string responsePropertyKey = serializedObject.targetObject.GetInstanceID() + "_Response";

                        bool isExpanded = false;
                        if (!responseFoldoutStates.TryGetValue(responsePropertyKey, out isExpanded))
                        {
                            responseFoldoutStates[responsePropertyKey] = false;
                        }

                        responseFoldoutStates[responsePropertyKey] = EditorGUILayout.Foldout(responseFoldoutStates[responsePropertyKey], "Response");

                        GUILayout.EndHorizontal();

                        if (responseFoldoutStates[responsePropertyKey])
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUI.indentLevel++;
                            GUILayout.Space(30);
                            EditorGUILayout.PropertyField(iterator, true);
                            EditorGUI.indentLevel--;
                            GUILayout.Space(30);
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(30);
                        if (iterator.name == "Event")
                        {
                            string eventKey = gameEventListener.GetInstanceID().ToString();
                            if (!selectedEvents.ContainsKey(eventKey))
                            {
                                selectedEvents[eventKey] = false;
                            }
                            selectedEvents[eventKey] = EditorGUILayout.Toggle(selectedEvents[eventKey], GUILayout.Width(20));
                        }
                        EditorGUILayout.PropertyField(iterator, true);
                        GUILayout.Space(30);
                        GUILayout.EndHorizontal();
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove Selected Event Listeners", GUILayout.Width(250)))
            {
                foreach (var eventListenerKey in selectedEvents)
                {
                    if (eventListenerKey.Value)
                    {
                        int instanceId = int.Parse(eventListenerKey.Key);
                        GameEventListener[] eventListenersComponents = gameObject.GetComponents<GameEventListener>();
                        foreach (var eventListener in eventListenersComponents)
                        {
                            if (eventListener.GetInstanceID() == instanceId)
                            {
                                Undo.DestroyObjectImmediate(eventListener);
                                break;
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button("Add Event Listener", GUILayout.Width(150)))
            {
                gameObject.AddComponent<GameEventListener>();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        else
        {
            GUILayout.Label("No Game Event Listeners found on this GameObject.");
        }
    }

    

    void OnSelectionChanged()
    {
        selectedObject = Selection.activeGameObject;
        Repaint();
    }
}

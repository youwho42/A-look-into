using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

using Klaxon.ConversationSystem;

public class DialogueEditorWindow : EditorWindow
{
    private DialogueObject dialogue;
    private Vector2 leftScroll;
    private Vector2 rightScroll;
    private int selectedNodeIndex = -1;
    Color32 neutralBackgroundStyle;
    Color32 highlightBackgroundStyle;
    SerializedObject serializedDialogue;

    private void OnEnable()
    {
        neutralBackgroundStyle = new Color32(180, 180, 180, 255);
        highlightBackgroundStyle = new Color32(255, 255, 255, 255);

        Selection.selectionChanged += OnSelectionChanged;

        if (dialogue != null)
            serializedDialogue = new SerializedObject(dialogue);

    }
    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }
    private void OnSelectionChanged()
    {
        if (Selection.activeObject is DialogueObject selectedDialogue)
        {
            dialogue = selectedDialogue;
            serializedDialogue = new SerializedObject(dialogue);
            selectedNodeIndex = -1; // reset node selection
            Repaint(); // refresh the window
        }
    }



    [MenuItem("Window/Klaxon/Dialogue Editor")]
    public static void OpenWindow()
    {
        GetWindow<DialogueEditorWindow>("Dialogue Editor");
    }

    private void OnGUI()
    {
        DrawDialoguePicker();

        if (dialogue == null)
            return;

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        DrawConversationDetails();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        DrawNodeList();
        DrawNodeDetails();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDialoguePicker()
    {
        dialogue = (DialogueObject)EditorGUILayout.ObjectField("Dialogue", dialogue, typeof(DialogueObject), false);
        if (dialogue != null)
        {
            serializedDialogue = new SerializedObject(dialogue);
        }
    }

    void DrawConversationDetails()
    {
        if (dialogue == null)
            return;

        if (serializedDialogue == null || serializedDialogue.targetObject != dialogue)
            serializedDialogue = new SerializedObject(dialogue);

        serializedDialogue.Update();

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Conversation Details", EditorStyles.boldLabel);
        GUILayout.Space(8);
        EditorGUI.indentLevel++; 
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        // Conversation Name
        SerializedProperty nameProp = serializedDialogue.FindProperty("ConversationName");
        EditorGUILayout.PropertyField(nameProp, new GUIContent("Conversation Name"));

        GUILayout.Space(10);

        // Conditions Needed
        SerializedProperty conditionsProp = serializedDialogue.FindProperty("ConditionsNeeded");
        EditorGUILayout.PropertyField(conditionsProp, new GUIContent("Required Conditions"), true);

        GUILayout.Space(10);

        // Undertakings
        SerializedProperty tiedProp = serializedDialogue.FindProperty("isTiedToUndertaking");
        SerializedProperty undertakingProp = serializedDialogue.FindProperty("undertaking");
        SerializedProperty undertakingStateProp = serializedDialogue.FindProperty("undertakingState");

        EditorGUILayout.PropertyField(tiedProp, new GUIContent("Tie To Undertaking?"));
        if (tiedProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(undertakingProp, new GUIContent("Undertaking"));
            EditorGUILayout.PropertyField(undertakingStateProp, new GUIContent("Required State"));
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        

        // Start Node
        //SerializedProperty startIDProp = serializedDialogue.FindProperty("startPhraseID");

        //if (dialogue.dialogueNodes.Count == 0)
        //{
        //    EditorGUILayout.HelpBox("No nodes available. Add at least one node.", MessageType.Warning);
        //    startIDProp.intValue = 0;
        //}
        //else
        //{
        //    // Build dropdown list of existing nodes
        //    string[] options = new string[dialogue.dialogueNodes.Count];
        //    for (int i = 0; i < dialogue.dialogueNodes.Count; i++)
        //        options[i] = $"Node {i}";

        //    // Clamp start index in case nodes were deleted
        //    int currentIndex = Mathf.Clamp(startIDProp.intValue, 0, dialogue.dialogueNodes.Count - 1);

        //    int selected = EditorGUILayout.Popup("Start Phrase", currentIndex, options);

        //    startIDProp.intValue = selected;
        //}

        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        serializedDialogue.ApplyModifiedProperties();
    }




    private void DrawNodeList()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(200));

        leftScroll = EditorGUILayout.BeginScrollView(leftScroll);

        if (dialogue.dialogueNodes != null)
        {
            for (int i = 0; i < dialogue.dialogueNodes.Count; i++)
            {
                GUI.color = selectedNodeIndex == i ? highlightBackgroundStyle : neutralBackgroundStyle;
                if (GUILayout.Button($"Node {i}"))
                {
                    selectedNodeIndex = i;
                }
            }
        }

        EditorGUILayout.EndScrollView();
        GUI.color = neutralBackgroundStyle;

        if (GUILayout.Button("Add Node"))
        {
            dialogue.dialogueNodes.Add(new DialogueNode());
            EditorUtility.SetDirty(dialogue);
            selectedNodeIndex = dialogue.dialogueNodes.Count - 1;
        }

        if (selectedNodeIndex >= 0)
        {
            if (GUILayout.Button("Remove Node"))
            {
                RemoveNode(selectedNodeIndex);
                if (selectedNodeIndex >= dialogue.dialogueNodes.Count)
                    selectedNodeIndex = dialogue.dialogueNodes.Count - 1;
                EditorUtility.SetDirty(dialogue);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawNodeDetails()
    {
        EditorGUILayout.BeginVertical("box");

        rightScroll = EditorGUILayout.BeginScrollView(rightScroll);

        if (dialogue == null || selectedNodeIndex < 0 || selectedNodeIndex >= dialogue.dialogueNodes.Count)
        {
            GUILayout.Label("Select a node to edit.");
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            return;
        }

        if (serializedDialogue == null || serializedDialogue.targetObject != dialogue)
            serializedDialogue = new SerializedObject(dialogue);

        serializedDialogue.Update();

        SerializedProperty nodesProp = serializedDialogue.FindProperty("dialogueNodes");
        SerializedProperty nodeProp = nodesProp.GetArrayElementAtIndex(selectedNodeIndex);


        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 16;
        GUILayout.Label($"Dialogue Node {selectedNodeIndex}", headerStyle);
        GUILayout.Space(20);
        EditorGUI.indentLevel++;


        // NPC Phrase
        SerializedProperty phraseProp = nodeProp.FindPropertyRelative("LocalizedPhrase");
        EditorGUILayout.PropertyField(phraseProp, new GUIContent("NPC Phrase"), true);

        GUILayout.Space(20);

        // Player Choices
        SerializedProperty choicesProp = nodeProp.FindPropertyRelative("Choices");
        EditorGUILayout.LabelField("Player Choices", EditorStyles.boldLabel);
        int removeIndex = -1;

        for (int i = 0; i < choicesProp.arraySize; i++)
        {
            SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(i);
            SerializedProperty replyProp = choiceProp.FindPropertyRelative("LocalizedChoice");
            SerializedProperty nextNodeProp = choiceProp.FindPropertyRelative("NextNodeID");

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(replyProp, new GUIContent("Reply Text"), true);

            // Next Node dropdown with None option
            string[] nodeOptions = GetNodeDropdownOptionsWithNone(selectedNodeIndex);
            int selectedDropdownIndex = GetDropdownIndexWithNone(nextNodeProp.intValue, selectedNodeIndex);
            selectedDropdownIndex = EditorGUILayout.Popup("Next Node", selectedDropdownIndex, nodeOptions);
            nextNodeProp.intValue = GetNodeIDFromDropdownWithNone(selectedDropdownIndex, selectedNodeIndex);

            if (GUILayout.Button("Remove Choice"))
            {
                removeIndex = i;
            }

            EditorGUILayout.EndVertical();
        }

        if (removeIndex >= 0)
            choicesProp.DeleteArrayElementAtIndex(removeIndex);

        if (GUILayout.Button("Add Choice"))
        {
            choicesProp.arraySize++;
            SerializedProperty newChoice = choicesProp.GetArrayElementAtIndex(choicesProp.arraySize - 1);
            newChoice.FindPropertyRelative("NextNodeID").intValue = -1;
        }

        // AutoNextNode dropdown only if there are no player choices
        if (choicesProp.arraySize == 0)
        {
            SerializedProperty autoNextProp = nodeProp.FindPropertyRelative("AutoNextNodeID");
            int autoNextID = autoNextProp.intValue;
            string[] autoNextOptions = GetNodeDropdownOptionsWithNone(selectedNodeIndex);
            int dropdownIndex = GetDropdownIndexWithNone(autoNextID, selectedNodeIndex);
            dropdownIndex = EditorGUILayout.Popup("Auto Next Node", dropdownIndex, autoNextOptions);
            autoNextProp.intValue = GetNodeIDFromDropdownWithNone(dropdownIndex, selectedNodeIndex);
        }

        GUILayout.Space(20);

        // Optional events
        EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("SetsCondition"));
        if (nodeProp.FindPropertyRelative("SetsCondition").boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("Condition"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("SetsUndertaking"));
        if (nodeProp.FindPropertyRelative("SetsUndertaking").boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("Undertaking"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("CompleteTask"));
        if (nodeProp.FindPropertyRelative("CompleteTask").boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("Task"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("GivesItem"));
        if (nodeProp.FindPropertyRelative("GivesItem").boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("Item"));
            EditorGUILayout.PropertyField(nodeProp.FindPropertyRelative("Amount"));
            EditorGUI.indentLevel--;
        }

        EditorGUI.indentLevel--;

        serializedDialogue.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    #region Dropdown Helpers with None Option

    private string[] GetNodeDropdownOptionsWithNone(int currentNodeIndex)
    {
        List<string> options = new List<string> { "None" }; // -1 = end of conversation
        for (int i = 0; i < dialogue.dialogueNodes.Count; i++)
        {
            if (i == currentNodeIndex) continue;
            options.Add($"Node {i}");
        }
        return options.ToArray();
    }

    private int GetDropdownIndexWithNone(int targetNodeID, int currentNodeIndex)
    {
        if (targetNodeID == -1) return 0; // None

        int index = 1; // first real node after None
        for (int i = 0; i < dialogue.dialogueNodes.Count; i++)
        {
            if (i == currentNodeIndex) continue;
            if (i == targetNodeID) return index;
            index++;
        }
        return 0; // fallback to None
    }

    private int GetNodeIDFromDropdownWithNone(int selectedIndex, int currentNodeIndex)
    {
        if (selectedIndex == 0) return -1; // None

        selectedIndex--; // shift index because 0 = None
        for (int i = 0; i < dialogue.dialogueNodes.Count; i++)
        {
            if (i == currentNodeIndex) continue;
            if (selectedIndex == 0) return i;
            selectedIndex--;
        }
        return -1;
    }

    #endregion

    private void RemoveNode(int indexToRemove)
    {
        dialogue.dialogueNodes.RemoveAt(indexToRemove);

        if (dialogue.startPhraseID == indexToRemove)
            dialogue.startPhraseID = 0;
        else if (dialogue.startPhraseID > indexToRemove)
            dialogue.startPhraseID--;

        for (int i = 0; i < dialogue.dialogueNodes.Count; i++)
        {
            DialogueNode node = dialogue.dialogueNodes[i];

            // AutoNextNode
            if (node.AutoNextNodeID == indexToRemove)
                node.AutoNextNodeID = -1;
            else if (node.AutoNextNodeID > indexToRemove)
                node.AutoNextNodeID--;

            // Choices NextNodeID
            foreach (var choice in node.Choices)
            {
                if (choice.NextNodeID == indexToRemove)
                    choice.NextNodeID = -1;
                else if (choice.NextNodeID > indexToRemove)
                    choice.NextNodeID--;
            }
        }

        // Reassign NodeIndex
        for (int i = 0; i < dialogue.dialogueNodes.Count; i++)
            dialogue.dialogueNodes[i].NodeIndex = i;

        EditorUtility.SetDirty(dialogue);
    }
}

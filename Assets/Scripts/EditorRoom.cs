using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class NewBehaviourScript : Editor
{
    Room room;

    private void OnEnable()
    {
        room = (Room)target;
    }

    public override void OnInspectorGUI()
    {
        //serializedObject.Update();

        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        room.Type = (Room.RoomType)EditorGUILayout.EnumPopup("Type", room.Type);

        if (room.Type == Room.RoomType.Default)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            room.DoorU = (GameObject)EditorGUILayout.ObjectField("DoorU", room.DoorU, typeof(GameObject), true);
            room.DoorD = (GameObject)EditorGUILayout.ObjectField("DoorD", room.DoorD, typeof(GameObject), true);
            room.DoorL = (GameObject)EditorGUILayout.ObjectField("DoorL", room.DoorL, typeof(GameObject), true);
            room.DoorR = (GameObject)EditorGUILayout.ObjectField("DoorR", room.DoorR, typeof(GameObject), true);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            room.DoorUMap = (GameObject)EditorGUILayout.ObjectField("DoorUMap", room.DoorUMap, typeof(GameObject), true);
            room.DoorDMap = (GameObject)EditorGUILayout.ObjectField("DoorDMap", room.DoorDMap, typeof(GameObject), true);
            room.DoorLMap = (GameObject)EditorGUILayout.ObjectField("DoorLMap", room.DoorLMap, typeof(GameObject), true);
            room.DoorRMap = (GameObject)EditorGUILayout.ObjectField("DoorRMap", room.DoorRMap, typeof(GameObject), true);
        }
        else if (room.Type == Room.RoomType.BigRoom)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            room.DoorUR = (GameObject)EditorGUILayout.ObjectField("DoorUR", room.DoorUR, typeof(GameObject), true);
            room.DoorDR = (GameObject)EditorGUILayout.ObjectField("DoorDR", room.DoorDR, typeof(GameObject), true);
            room.DoorLU = (GameObject)EditorGUILayout.ObjectField("DoorLU", room.DoorLU, typeof(GameObject), true);
            room.DoorRU = (GameObject)EditorGUILayout.ObjectField("DoorRU", room.DoorRU, typeof(GameObject), true);
            room.DoorUL = (GameObject)EditorGUILayout.ObjectField("DoorUL", room.DoorUL, typeof(GameObject), true);
            room.DoorDL = (GameObject)EditorGUILayout.ObjectField("DoorDL", room.DoorDL, typeof(GameObject), true);
            room.DoorLD = (GameObject)EditorGUILayout.ObjectField("DoorLD", room.DoorLD, typeof(GameObject), true);
            room.DoorRD = (GameObject)EditorGUILayout.ObjectField("DoorRD", room.DoorRD, typeof(GameObject), true);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            room.DoorURMap = (GameObject)EditorGUILayout.ObjectField("DoorURMap", room.DoorURMap, typeof(GameObject), true);
            room.DoorDRMap = (GameObject)EditorGUILayout.ObjectField("DoorDRMap", room.DoorDRMap, typeof(GameObject), true);
            room.DoorLUMap = (GameObject)EditorGUILayout.ObjectField("DoorLUMap", room.DoorLUMap, typeof(GameObject), true);
            room.DoorRUMap = (GameObject)EditorGUILayout.ObjectField("DoorRUMap", room.DoorRUMap, typeof(GameObject), true);
            room.DoorULMap = (GameObject)EditorGUILayout.ObjectField("DoorULMap", room.DoorULMap, typeof(GameObject), true);
            room.DoorDLMap = (GameObject)EditorGUILayout.ObjectField("DoorDLMap", room.DoorDLMap, typeof(GameObject), true);
            room.DoorLDMap = (GameObject)EditorGUILayout.ObjectField("DoorLDMap", room.DoorLDMap, typeof(GameObject), true);
            room.DoorRDMap = (GameObject)EditorGUILayout.ObjectField("DoorRDMap", room.DoorRDMap, typeof(GameObject), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ItemContainer))]
public class ItemContainerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        ItemContainer container = target as ItemContainer;
        if(GUILayout.Button("Clear container")){
            for (int i = 0; i<container.slots.Count; i++){
                container.slots[i].Clear();
            }
        }
        DrawDefaultInspector();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

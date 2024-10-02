using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class ItemToolbarPanel : ItemPanel
{
    [SerializeField] ToolBarController toolbarController;

    public override void OnClick(int id)
    {
        toolbarController.Set(id);
        Highlight(id);
    }

    int currentSelectedTool;
    public void Highlight(int id){

        buttons[currentSelectedTool].Highlight(false);
        currentSelectedTool = id;
        buttons[currentSelectedTool].Highlight(true);
    }

}

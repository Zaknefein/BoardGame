using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCursor : MonoBehaviour
{
    string text = "PUT TEXT HERE";
    private string currentToolTipText = "";
    private GUIStyle guiStyleFore;
    private GUIStyle guiStyleBack;


    void Start()
    {
        guiStyleFore = new GUIStyle();
        guiStyleFore.normal.textColor = Color.white;
        guiStyleFore.alignment = TextAnchor.UpperCenter;
        guiStyleFore.wordWrap = true;
        guiStyleBack = new GUIStyle();
        guiStyleBack.normal.textColor = Color.black;
        guiStyleBack.alignment = TextAnchor.UpperCenter;
        guiStyleBack.wordWrap = true;
    }
    void OnMouseEnter()
    {
        currentToolTipText = transform.name;
    }
    void OnMouseExit()
    {
        currentToolTipText = "";
    }
    void OnGUI()
    {
        if (currentToolTipText != "")
        {
            var x = Event.current.mousePosition.x;
            var y = Event.current.mousePosition.y;
            GUI.Label(new Rect(x - 149, y + 40, 300, 60), currentToolTipText, guiStyleBack);
            GUI.Label(new Rect(x - 150, y + 40, 300, 60), currentToolTipText, guiStyleFore);
        }
    }

}

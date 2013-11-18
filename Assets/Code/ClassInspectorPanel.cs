using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class ClassInspectorPanel : MonoBehaviour
{
	public ClassControl SelectedClass = null;
    public bool Show = false;

    private static ClassInspectorPanel mInstance;
    public static ClassInspectorPanel Instance
    {
        get
        {
            return mInstance;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        mInstance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnGUI()
    {
        if (Show && SelectedClass != null)
        {
            Rect panelPos = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 100, 100);
            GUI.Box(panelPos, "Class Inspector");
            GUI.TextField(new Rect(panelPos.x + 5, panelPos.y + 5, 10, 10), SelectedClass.ClassName);
        }
    }

}
	

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class ClassInspectorPanel : MonoBehaviour
{
    private ClassControl mSelectedClass = null;
	public ClassControl SelectedClass
    {
        get { return mSelectedClass; }
        set
        {
            mSelectedClass = value;
            FocusOnSelectedClass();
        }
    }

    private bool mShow = false;
    public bool Show
    {
        get { return mShow; }
        set
        {
            mShow = value;
            mPanelPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }
    }

    public Vector2 PanelSize = new Vector2(200, 100);
    private Vector2 mPanelPos = Vector2.zero;

    private static ClassInspectorPanel mInstance;
    public static ClassInspectorPanel Instance
    {
        get
        {
            if (mInstance == null)
            {
                Debug.LogWarning(string.Format("No {0} singleton exists! Creating new one.", typeof(ClassInspectorPanel).Name));
                GameObject owner = new GameObject("ClassInspectorPanel");
                mInstance = owner.AddComponent<ClassInspectorPanel>();
            }
            return mInstance;
        }
    }

    private void Awake()
    {
        if (mInstance != null && mInstance != this)
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
            GUI.Box(new Rect(mPanelPos.x, mPanelPos.y, PanelSize.x, PanelSize.y), "Class Inspector");
            GUI.Label(new Rect(mPanelPos.x + 10, mPanelPos.y + 20, 100, 40), "Class name: " + SelectedClass.ClassName);

//            GUI.BeginScrollView(new Rect(mPanelPos.x + 10, mPanelPos.y + 50, 100, 100),

        }
    }

    private void FocusOnSelectedClass()
    {
        if (mSelectedClass != null)
        {
            Color initColor = mSelectedClass.InnerClassEmitter.startColor;
            mSelectedClass.InnerClassEmitter.startColor = new Color(initColor.r, initColor.g, initColor.b + 100);
        }
    }
}
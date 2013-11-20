using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class ClassInspectorPanel : MonoBehaviour
{
    #region Tunables
    public Vector2 PanelSize = new Vector2(200, 100);
    #endregion

    #region Singleton stuff
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
    #endregion

    #region Private Members
    private ClassControl mSelectedClass = null;
    private bool mShow = false;
    private Vector2 mPanelPos = Vector2.zero;
    #endregion

    #region Public Properties
    public ClassControl SelectedClass
    {
        get { return mSelectedClass; }
        set
        {
            mSelectedClass = value;
            if (mSelectedClass != null)
            {
                FocusOnSelectedClass();
            }
        }
    }

    public bool Show
    {
        get { return mShow; }
        set
        {
            mShow = value;
            mPanelPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }
    }
    #endregion

    #region Component Methods
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
        }

        mInstance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        InputController.Instance.PostEvent += InputEventHandler;
    }

    private void OnDisable()
    {
        InputController.Instance.PostEvent -= InputEventHandler;
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
    #endregion

    #region Event Handlers
    private void InputEventHandler(Event ev)
    {
        if (ev != null && ev.isMouse && ev.type == EventType.MouseDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                ClassControl clickedClass = hit.transform.gameObject.GetComponent<ClassControl>();
                if (clickedClass != null)
                {
                    if (SelectedClass == clickedClass)
                    {
                        Show = !Show;
                    }
                    else
                    {
                        SelectedClass = clickedClass;
                        Show = true;
                    }
                }
            }
            else
            {
                SelectedClass = null;
            }
        }
    }
    #endregion

    #region Private Methods
    private void FocusOnSelectedClass()
    {
        if (mSelectedClass != null)
        {
            Color initColor = mSelectedClass.InnerClassEmitter.startColor;
            mSelectedClass.InnerClassEmitter.startColor = new Color(initColor.r, initColor.g, initColor.b + 100);
        }
    }
    #endregion
}
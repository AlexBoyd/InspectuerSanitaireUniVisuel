using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class ClassInspectorPanel : MonoBehaviour
{
    #region Tunables
    public Vector2 PanelSize = new Vector2(200, 400);
    public Color SelectedClassParticleColor = Color.blue;
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
    private Vector2 mScrollPos = Vector2.zero;
    #endregion

    #region Public Properties
    public ClassControl SelectedClass
    {
        get { return mSelectedClass; }
        set
        {
            ClassControl prevSelectedClass = mSelectedClass;
            mSelectedClass = value;

            if (mSelectedClass != prevSelectedClass)
            {
                if (mSelectedClass != null)
                {
                    FocusOnSelectedClass();
                }

                if (prevSelectedClass != null)
                {
                    prevSelectedClass.RevertColorsToBase();
                }
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
        InputController.Instance.PostEvent += InputEventHandler;

        DontDestroyOnLoad(gameObject);
    }

    private void OnGUI()
    {
        if (Show && SelectedClass != null)
        {
            //Draw the box panel
            Rect panel = new Rect(mPanelPos.x, mPanelPos.y, PanelSize.x, PanelSize.y);
            GUI.Box(panel, "Class Inspector");

            Rect panelArea = new Rect(panel.x, panel.y + 40, panel.width, panel.height);
            GUILayout.BeginArea(panelArea);
            GUILayout.BeginVertical();

            //Class name
            GUILayout.Label("Class name: " + SelectedClass.ClassName);

            //Class sewage level
            GUILayout.Label("Sewage level: " + SelectedClass.SewageLevel);

            //List class dependencies
            GUILayout.Label("Class dependencies (" + SelectedClass.ClassDependancies.Count + "):");
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, false, false);
            foreach(ClassControl.ClassHookup ch in SelectedClass.ClassDependancies)
            {
                if (ch.AttachedClass != null)
                {
                    if (GUILayout.Button(ch.AttachedClass.ClassName))
                    {
                        SelectedClass = ch.AttachedClass;
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
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
        if (SelectedClass != null)
        {
            SelectedClass.ChangeInnerParticleColor(SelectedClassParticleColor);
            SelectedClass.ChangeDependencyParticleColor(SelectedClassParticleColor);
        }
    }
    #endregion
}
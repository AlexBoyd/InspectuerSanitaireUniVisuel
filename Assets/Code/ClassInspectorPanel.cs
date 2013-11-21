using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class ClassInspectorPanel : Singleton<ClassInspectorPanel>
{
    #region Enums
    public enum ScreenAlignment
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }
    #endregion

    #region Tunables
    [SerializeField]private int m_PanelWidth = 200;
    [SerializeField]private int m_PanelHeight = 400;
    public Color SelectedClassDependenciesParticleColor = Color.blue;
    public Color SelectedClassDependentsParticleColor = Color.magenta;
    public bool ShowAtMousePosition = false;
    public bool KeepVisible = false;
    public ScreenAlignment PanelAlignment = ScreenAlignment.TopRight;
    #endregion

    #region Private Members
    private ClassControl mSelectedClass = null;
    private bool mShow = false;
    private Vector2 mPanelPos = Vector2.zero;
    private Vector2 mScrollPos = Vector2.zero;
    #endregion

    #region Public Properties
    public int PanelWidth
    {
        get { return m_PanelWidth; }
    }

    public int PanelHeight
    {
        get { return Mathf.Clamp(m_PanelHeight, 100, Screen.height/2); }
    }

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
    #endregion

    #region Component Methods
    private void Awake()
    {
        InputController.Instance.PostEvent += InputEventHandler;
    }

    private void OnGUI()
    {
        if (KeepVisible || (mShow && SelectedClass != null))
        {
            //Figure out where to position the panel
            if (ShowAtMousePosition)
            {
                mPanelPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            }
            else
            {
                switch(PanelAlignment)
                {
                    case ScreenAlignment.TopLeft:
                        mPanelPos = new Vector2(0, 0);
                        break;
                    case ScreenAlignment.TopRight:
                        mPanelPos = new Vector2(Screen.width - PanelWidth, 0);
                        break;
                    case ScreenAlignment.BottomLeft:
                        mPanelPos = new Vector2(0, Screen.height - PanelHeight);
                        break;
                    case ScreenAlignment.BottomRight:
                        mPanelPos = new Vector2(Screen.width - PanelWidth, Screen.height - PanelHeight);
                        break;
                }
            }

            //Draw the box panel
            Rect panel = new Rect(mPanelPos.x, mPanelPos.y, PanelWidth, PanelHeight);

            GUILayout.BeginArea(panel, "Class Inspector", "box");
            GUILayout.BeginVertical();
            GUILayout.Space(30f);

            //Class name
            string className = SelectedClass != null ? SelectedClass.ClassName : string.Empty;
            GUILayout.Label("Class name: " + className);

            //Class score
            string classScore = SelectedClass != null ? SelectedClass.SewageLevel.ToString() : string.Empty;
            GUILayout.Label("Sewage level: " + classScore);

            //List class dependencies
            int numDepedencies = SelectedClass != null ? SelectedClass.ClassDependancies.Count : 0;
            GUILayout.Label("Class dependencies (" + numDepedencies + "):");

            mScrollPos = GUILayout.BeginScrollView(mScrollPos, false, false, GUILayout.MaxHeight(Screen.height/2));
            if (SelectedClass != null)
            {
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
                        mShow = !mShow;
                    }
                    else
                    {
                        SelectedClass = clickedClass;
                        mShow = true;
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
            SelectedClass.ChangeInnerParticleColor(SelectedClassDependenciesParticleColor);
            SelectedClass.ChangeDependencyParticleColor(SelectedClassDependenciesParticleColor);
            SelectedClass.ChangeDependentsParticleColor(SelectedClassDependentsParticleColor);
        }
    }
    #endregion
}
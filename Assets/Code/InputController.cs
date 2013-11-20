using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

//Use cases
//  Select class
//  Deselect class
//  Drag camera
//  Rotate camera

public class InputController : MonoBehaviour
{
    public Action<Event> PostEvent;

    #region Singleton stuff
    private static InputController mInstance;
    public static InputController Instance
    {
        get
        {
            if (mInstance == null)
            {
                Debug.LogWarning(string.Format("No {0} singleton exists! Creating new one.", typeof(InputController).Name));
                GameObject owner = new GameObject("InputController");
                mInstance = owner.AddComponent<InputController>();
            }
            return mInstance;
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

    private void OnGUI()
    {
        if (Event.current.isMouse)
        {
            HandleMouseEvent();
            Event.current.Use();
        }
    }
    #endregion

    #region Private Methods
    private void HandleMouseEvent()
    {
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (Input.GetMouseButtonDown(0))
                {
                    PostMouseEvent(new Event(Event.current));
                }
                break;
            case EventType.MouseDrag:
                break;
        }
    }

    private void PostMouseEvent(Event ev)
    {
        if (PostEvent != null)
        {
            PostEvent(ev);
        }
    }
    #endregion
}
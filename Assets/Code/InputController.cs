using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

//Use cases
//  Select class
//  Deselect class
//  Drag camera
//  Rotate camera

public class InputController : Singleton<InputController>
{
    public Action<Event> PostEvent;

    #region Component Methods
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
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
    private void OnGUI()
    {
        if (Event.current.isMouse)
        {
            HandleMouseEvent();
        }
    }

    private void HandleMouseEvent()
    {
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        ClassControl classComponent = hit.transform.GetComponent<ClassControl>();
                        if (classComponent != null)
                        {
                            if (ClassInspectorPanel.Instance.SelectedClass == classComponent)
                            {
                                ClassInspectorPanel.Instance.Show = !ClassInspectorPanel.Instance.Show;
                            }
                            else
                            {
                                ClassInspectorPanel.Instance.SelectedClass = classComponent;
                                ClassInspectorPanel.Instance.Show = true;
                            }
                        }
                    }
                    else
                    {
                        ClassInspectorPanel.Instance.SelectedClass = null;
                    }
                    Event.current.Use();
                }
                break;
            case EventType.MouseUp:
                break;
            case EventType.MouseDrag:
                break;
        }
    }
}
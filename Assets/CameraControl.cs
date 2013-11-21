using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public int ZoomSpeed = 5;
	public int RotationSpeed = 1;
	void Update () 
	{
		if(Camera.mainCamera.isOrthoGraphic)
		{
			Camera.mainCamera.orthographicSize -= Input.GetAxis("Vertical") * ZoomSpeed;
		}
		else
		{
			transform.position += transform.forward * Input.GetAxis("Vertical") * ZoomSpeed;
		}
		transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Horizontal") * RotationSpeed);
		
		if(Input.GetButton("Jump"))
		{
			Camera.mainCamera.orthographic =! Camera.mainCamera.orthographic;
		}
	}
}

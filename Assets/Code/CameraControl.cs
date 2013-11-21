using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{

	public int ZoomSpeed = 5;
	public int RotationSpeed = 1;

    private void Update()
	{
		if (Camera.main.isOrthoGraphic)
		{
			Camera.main.orthographicSize -= Input.GetAxis("Vertical") * ZoomSpeed;
		}
		else
		{
			transform.position += transform.forward * Input.GetAxis("Vertical") * ZoomSpeed;
		}
		transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Horizontal") * RotationSpeed);

        if (Input.GetMouseButton(0))
        {
            transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Mouse X") * RotationSpeed);
        }

        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Mouse Y") * RotationSpeed);
        }

		if(Input.GetButtonUp("Jump"))
		{
			Camera.main.orthographic =! Camera.main.orthographic;
		}
	}
}

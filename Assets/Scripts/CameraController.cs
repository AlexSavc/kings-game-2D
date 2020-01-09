using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
	
	public float panSpeed = 10f;
	public float panBorderThickness = 10f;
	public Vector2 panLimit;
    Vector3 original;
    //bool dragging = false;
    public float rotateSpeed = 5f;

    public float scrollSpeed = 200f;

	void Update ()
	{
		Vector3 Pos = transform.position;
					

		if (Input.GetKey("w")/* || Input.mousePosition.y >= Screen.height - panBorderThickness*/)
		{
			Pos.y += panSpeed * Time.deltaTime;
            //Pos.z = Pos.y + 0.5f;
        }

		transform.position = Pos;

		if (Input.GetKey("s")/* || Input.mousePosition.y <= panBorderThickness*/)
		{
			Pos.y -= panSpeed * Time.deltaTime;
            //Pos.z = Pos.y + 0.5f;
        }

		transform.position = Pos;

		if (Input.GetKey("d")/* || Input.mousePosition.x >= Screen.width - panBorderThickness*/)
		{
			Pos.x += panSpeed * Time.deltaTime;
        }

		transform.position = Pos;

		if (Input.GetKey("a")/* || Input.mousePosition.x <= panBorderThickness*/)
		{
			Pos.x -= panSpeed * Time.deltaTime;
		}

        float scroll = Input.GetAxis ("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scroll * scrollSpeed * Time.deltaTime;

		Pos.x = Mathf.Clamp (Pos.x, -panLimit.x, panLimit.x);
		Pos.y = Mathf.Clamp (Pos.y, -panLimit.y, panLimit.y);
        
		transform.position = Pos;
	}
}

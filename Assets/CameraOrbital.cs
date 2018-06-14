using UnityEngine;
using System.Collections;

public class CameraOrbital : MonoBehaviour 
{
	public int mouseSens = 10, scrollSens = 10, moveSens = 1;
	Vector3 moveSensitivity;
	
	Vector3 cameraRotationY = new Vector3();
	Vector3 cameraRotationX = new Vector3();
    Vector3 cameraPosition = new Vector3();
	Vector2 deltaMouse = new Vector2();
	bool isDragging;
	Vector3 lastClick;
	bool displayButtons;
	RaycastHit hit;
	
	void Start () 
	{
		moveSensitivity = new Vector3(moveSens, 0, moveSens);
		cameraRotationX = gameObject.transform.localEulerAngles;
		cameraRotationY = gameObject.transform.parent.localEulerAngles;
		//this.gameObject.transform.parent = GameObject.Find("CamPar").transform;
	}
	
	void Update () 
	{
		leftMouse();
		rightMouse();
		mouseWheel();
		keyboardInput();
		gameObject.transform.localEulerAngles = cameraRotationX;
		gameObject.transform.parent.localEulerAngles = cameraRotationY;
	}
	
	void leftMouse()
	{
		if (Input.GetKeyUp(KeyCode.Mouse0) && !isDragging)
		{
			lastClick = Input.mousePosition;
			//Debug.Log(lastClick.ToString());
			if (Physics.Raycast(GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 100, 1))
			{
				Debug.Log ("Works");
			}
			else
				displayButtons = false;
		}
	}

	void rightMouse()
	{
		if (Mathf.Abs(deltaMouse.x) > .1 && Mathf.Abs(deltaMouse.y) > .1)
			isDragging = true;
		if (isDragging && Input.GetKeyUp(KeyCode.Mouse1))
			isDragging = false;
		if (!Input.GetKey(KeyCode.Mouse2) && Input.GetKey(KeyCode.Mouse1))
		{
			deltaMouse.x = Input.GetAxisRaw("Mouse X") * mouseSens;
			deltaMouse.y = Input.GetAxisRaw("Mouse Y") * mouseSens;
			if (isDragging)
			{
				//cameraRotation.y += deltaMouse.x * .25f;
				cameraRotationY.y += deltaMouse.x * .25f;
				if (cameraRotationX.x + 180 > 90 && cameraRotationX.x + 180 < 270)
				{
					cameraRotationX.x -= deltaMouse.y * .25f;
				}
				else if (cameraRotationX.x >= 90)
				{
					cameraRotationX.x = 89.9999f;
				}
				else if (cameraRotationX.x <= -90)
				{
					cameraRotationX.x = -89.9999f;
				}
			}
		}
	}

	void keyboardInput()
	{
		if (Input.GetKey(KeyCode.UpArrow))
			this.gameObject.transform.parent.Translate(new Vector3( Vector3.forward.x, 0,  Vector3.forward.z) * moveSens * .01f);
		else if (Input.GetKey(KeyCode.DownArrow))
			this.gameObject.transform.parent.Translate(new Vector3(-Vector3.forward.x, 0, -Vector3.forward.z) * moveSens * .01f);
		if (Input.GetKey(KeyCode.RightArrow))
			this.gameObject.transform.parent.Translate(new Vector3( Vector3.right.x, 0,    Vector3.right.z) * moveSens * .01f);
		else if (Input.GetKey(KeyCode.LeftArrow))
			this.gameObject.transform.parent.Translate(new Vector3(-Vector3.right.x, 0,   -Vector3.right.z) * moveSens * .01f);
	}
	
	void mouseWheel()
	{
        if (Input.GetKey(KeyCode.Mouse2)) {
            deltaMouse.x = Input.GetAxisRaw("Mouse X") * mouseSens;
            deltaMouse.y = Input.GetAxisRaw("Mouse Y") * mouseSens;
            //cameraRotation.y += deltaMouse.x * .25f;
            this.gameObject.transform.parent.Translate(new Vector3(-deltaMouse.x, 0, -deltaMouse.y) * moveSens * .01f);
        }
		Vector3 camPos = gameObject.GetComponentInChildren<Camera>().transform.localPosition;
		camPos += new Vector3(0, 0, Input.mouseScrollDelta.y * (scrollSens * .0625f));
		gameObject.GetComponentInChildren<Camera>().transform.localPosition = camPos;
	}
}

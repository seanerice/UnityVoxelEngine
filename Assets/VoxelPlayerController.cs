using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

public class VoxelPlayerController : MonoBehaviour {
	Camera ControllerCamera;
	ChunkLoader chunkLoader;

	Vector3 CenterViewPort = new Vector3(.5f, .5f);
	public float MaxRaycastDist = 10;
	RaycastHit LastRaycastHit;

	// Use this for initialization
	void Start () {
		ControllerCamera = gameObject.GetComponent<Camera>();
		chunkLoader = gameObject.GetComponent<ChunkLoader>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse0))
			OnLeftMouse();
	}

	public void OnLeftMouse() {
		if (Physics.Raycast(transform.position, transform.position + ControllerCamera.ViewportToWorldPoint(CenterViewPort), out LastRaycastHit, MaxRaycastDist)) {
			Vector3 hitPos = LastRaycastHit.point;
			Vector3 normal = LastRaycastHit.normal;
			Vector3 voxelGlobalPos = new Vector3(Mathf.Floor(hitPos.x), Mathf.Floor(hitPos.y), Mathf.Floor(hitPos.z));

		}
	}
}

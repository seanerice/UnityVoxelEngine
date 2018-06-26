using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;

public class VoxelPlayerController : MonoBehaviour {
	Camera ControllerCamera;
	ChunkLoader chunkLoader;
    public GameObject TransparentVoxel;

	Vector3 CenterViewPort = new Vector3(.5f, .5f , .5f);
	public float MaxRaycastDist = 10;
	RaycastHit LastRaycastHit;

	// Use this for initialization
	void Start () {
		ControllerCamera = gameObject.GetComponentInChildren<Camera>();
		chunkLoader = gameObject.GetComponent<ChunkLoader>();
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = ControllerCamera.ViewportPointToRay(CenterViewPort);
        if (Physics.Raycast(ray, out LastRaycastHit, MaxRaycastDist))
        {
			TransparentVoxel.SetActive(true);
            Vector3 hitPos = LastRaycastHit.point;
            Vector3 normal = LastRaycastHit.normal;
            Vector3 invNorm = Vector3.one - normal;
            normal.Scale(CenterViewPort);
            invNorm.Scale(CenterViewPort);
            Vector3 voxelGlobalPos = new Vector3(Mathf.Floor(hitPos.x + invNorm.x), Mathf.Floor(hitPos.y + invNorm.y), Mathf.Floor(hitPos.z + invNorm.z));
            TransparentVoxel.transform.position = voxelGlobalPos;
            //Debug.DrawLine(ray.origin, hitPos, Color.blue, .01f);
            //Debug.Log(hitPos + " " + voxelGlobalPos);
        } else {
			TransparentVoxel.SetActive(false);
		}
        if (Input.GetKeyDown(KeyCode.Mouse0))
            OnLeftMouse();
        if (Input.GetKeyDown(KeyCode.Mouse1))
			OnRightMouse();
	}

	public void OnRightMouse() {
        //Debug.Log("RightMouse");
        Ray ray = ControllerCamera.ViewportPointToRay(CenterViewPort);
        
        if (Physics.Raycast(ray, out LastRaycastHit, MaxRaycastDist)) {
            Vector3 hitPos = LastRaycastHit.point;
            Vector3 normal = LastRaycastHit.normal;
            Vector3 invNorm = Vector3.one - normal;
            invNorm.Scale(CenterViewPort);
            Vector3 voxelGlobalPos = new Vector3(Mathf.Floor(hitPos.x + invNorm.x), Mathf.Floor(hitPos.y + invNorm.y), Mathf.Floor(hitPos.z + invNorm.z)) + normal;
            //Debug.Log(voxelGlobalPos);
            //Debug.Log("Normal: " + normal);
            Debug.DrawRay(ray.origin, ray.direction, Color.blue, .25f);
            chunkLoader.AddBlock(voxelGlobalPos, VoxelType.Grass);
		}
        else
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.green, .25f);
        }
	}

    public void OnLeftMouse()
    {
        //Debug.Log("RightMouse");
        Ray ray = ControllerCamera.ViewportPointToRay(CenterViewPort);

        if (Physics.Raycast(ray, out LastRaycastHit, MaxRaycastDist))
        {
            Vector3 hitPos = LastRaycastHit.point;
            Vector3 normal = LastRaycastHit.normal;
            Vector3 invNorm = Vector3.one - normal;
            normal.Scale(CenterViewPort);
            invNorm.Scale(CenterViewPort);
            Vector3 voxelGlobalPos = new Vector3(Mathf.Floor(hitPos.x + invNorm.x), Mathf.Floor(hitPos.y + invNorm.y), Mathf.Floor(hitPos.z + invNorm.z));
            //Debug.Log(voxelGlobalPos);
            //Debug.Log("Normal: " + normal);
            Debug.DrawRay(ray.origin, ray.direction, Color.red, .25f);
            chunkLoader.RemoveBlock(voxelGlobalPos);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.green, .25f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChunkLoaderUIHandler : MonoBehaviour {

	// Debug
	public Text RenderChunkQueueCountText;


	// Use this for initialization
	void Start () {
		SetLoadQueueCount(0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetLoadQueueCount(int val) {
		RenderChunkQueueCountText.text = "Load Queue: " + val;
	}

}

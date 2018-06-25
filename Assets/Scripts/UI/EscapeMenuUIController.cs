using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenuUIController : MonoBehaviour {

	Canvas ThisCanvas;
	public UnityStandardAssets.Characters.FirstPerson.FirstPersonController FirstPersonController;
	public VoxelPlayerController PlayerController;
	public Canvas GraphicsCanvas;
	public Canvas SoundCanvas;
	public Canvas DebugCanvas;

	// Use this for initialization
	void Start () {
		ThisCanvas = gameObject.GetComponent<Canvas>();
		ThisCanvas.enabled = false;
		DeactivateAllCanvases();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			ThisCanvas.enabled = !ThisCanvas.isActiveAndEnabled;
			PlayerController.enabled = !ThisCanvas.isActiveAndEnabled;
			if (ThisCanvas.enabled) {
				FirstPersonController.m_MouseLook.SetCursorLock(!ThisCanvas.isActiveAndEnabled);
				FirstPersonController.enabled = !FirstPersonController.isActiveAndEnabled;
				//FirstPersonController.m_MouseLook.UpdateCursorLock();
			} else {
				FirstPersonController.enabled = !FirstPersonController.isActiveAndEnabled;
				FirstPersonController.m_MouseLook.SetCursorLock(!ThisCanvas.isActiveAndEnabled);
				//FirstPersonController.m_MouseLook.UpdateCursorLock();
			}
			
			DeactivateAllCanvases();
		}
	}

	public void DeactivateAllCanvases() {
		GraphicsCanvas.enabled = false;
		SoundCanvas.enabled = false;
		DebugCanvas.enabled = false;
	}

	public void Graphics() {
		DeactivateAllCanvases();
		GraphicsCanvas.enabled = true;
	}

	public void Sound() {
		DeactivateAllCanvases();
		SoundCanvas.enabled = true;
	}

	public void DebugMenu() {
		DeactivateAllCanvases();
		DebugCanvas.enabled = true;
	}

	public void Quit () {
		Application.Quit();
	}
}

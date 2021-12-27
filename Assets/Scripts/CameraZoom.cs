using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
	float minZoom = 0.7f;
	float maxZoom = 2.5f;
	float zoomValue = 0.05f;
	CinemachineBrain brain;
	CinemachineVirtualCamera cam;
	float timeAfterZoom = 7.5f;
	float timer;
	IEnumerator resetZoom;
	bool zoomActive;
	void Start() 
	{
		SetCamera();
	}
	private void Update()
	{
		

		if(zoomActive)
			timer += Time.deltaTime;

		SetZoom();

		if (timer >= timeAfterZoom)
        {
			resetZoom = ResetZoomCo();
			StartCoroutine(resetZoom);
		}
			

	}

	void SetZoom()
    {
		SetCamera();
		if (Input.mouseScrollDelta.y > 0)
		{
			if(resetZoom!=null)
				StopCoroutine(resetZoom);
			cam.m_Lens.OrthographicSize = Mathf.Clamp(cam.m_Lens.OrthographicSize -= zoomValue, minZoom, maxZoom);
			zoomActive = true;
			timer = 0;
		}
		else if (Input.mouseScrollDelta.y < 0)
		{
			if (resetZoom != null)
				StopCoroutine(resetZoom);
			cam.m_Lens.OrthographicSize = Mathf.Clamp(cam.m_Lens.OrthographicSize += zoomValue, minZoom, maxZoom);
			zoomActive = true;
			timer = 0;
		}
	}

	IEnumerator ResetZoomCo()
    {

		cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, 1.5f, Time.deltaTime);
		zoomActive = false;
		yield return null;
    }

	void SetCamera()
    {
		if(cam == null)
        {
			brain = Camera.main.GetComponent<CinemachineBrain>();
			cam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
		}
			
	}
}

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
	public bool resetWithTimer;
	void Start() 
	{
		brain = Camera.main.GetComponent<CinemachineBrain>();
		SetCamera();
		GameEventManager.onMouseScrollEvent.AddListener(SetZoom);
	}
    private void OnDisable()
    {
        GameEventManager.onMouseScrollEvent.RemoveListener(SetZoom);
    }

    private void Update()
	{
        //SetZoom();

		if (resetWithTimer)
		{
            if (zoomActive)
                timer += Time.deltaTime;

            if (timer >= timeAfterZoom)
            {
                resetZoom = ResetZoomCo();
                StartCoroutine(resetZoom);
            }
        }
	}

	void SetZoom(float dir)
    {
		if (PlayerInformation.instance.uiScreenVisible)
			return;
		SetCamera();
		if (dir > 0)
			Zoom(-zoomValue);
		else if (dir < 0)
			Zoom(zoomValue);
	}
	private void Zoom(float value)
    {
        if (resetZoom != null)
            StopCoroutine(resetZoom);
        cam.m_Lens.OrthographicSize = Mathf.Clamp(cam.m_Lens.OrthographicSize += value, minZoom, maxZoom);
        zoomActive = true;
        timer = 0;
    }

    IEnumerator ResetZoomCo()
    {
		//SetCamera();
		cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, 1.5f, Time.deltaTime);
		zoomActive = false;
		yield return null;
    }

	void SetCamera()
    {
		if(cam == null)
			cam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
	}
}

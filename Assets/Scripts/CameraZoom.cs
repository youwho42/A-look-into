using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
	float minZoom = 0.7f;
	float maxZoom = 2.5f;
	float zoomValue = 0.05f;
	float baseZoom = 1.25f;
	CinemachineBrain brain;
	CinemachineVirtualCamera cam;
	float timeAfterZoom = 7.5f;
	
	IEnumerator resetZoom;
	
	public bool resetWithTimer;
	public Camera silhouetteCamera;
	void Start() 
	{
		brain = Camera.main.GetComponent<CinemachineBrain>();
		SetCamera();
		GameEventManager.onMouseScrollEvent.AddListener(SetZoom);
	}
    private void OnDestroy()
    {
        GameEventManager.onMouseScrollEvent.RemoveListener(SetZoom);
    }

 

	void SetZoom(float dir)
    {
		if (UIScreenManager.instance.GetCurrentUI() != UIScreenType.None)
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
		var val = Mathf.Clamp(cam.m_Lens.OrthographicSize += value, minZoom, maxZoom);
        cam.m_Lens.OrthographicSize = val;
		silhouetteCamera.orthographicSize = val;
        

        if (UIScreenManager.instance.gameplay.autoZoomBinary == 1)
        {
            if (resetZoom != null)
                StopCoroutine(resetZoom);
            resetZoom = ResetZoomCo(val > baseZoom);
            StartCoroutine(resetZoom);
        }


    }

    IEnumerator ResetZoomCo(bool isPos)
    {
        yield return new WaitForSeconds(timeAfterZoom);
		while (isPos ? cam.m_Lens.OrthographicSize > baseZoom : cam.m_Lens.OrthographicSize < baseZoom)
		{
            cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, baseZoom, Time.deltaTime);
            silhouetteCamera.orthographicSize = cam.m_Lens.OrthographicSize;
			yield return null;
        }
 
		yield return null;
    }

	void SetCamera()
    {
		if(cam == null)
			cam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
	}
}

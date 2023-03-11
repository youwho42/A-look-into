using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartTimelineCutscene : MonoBehaviour
{
    public PlayableDirector timelineToPlay;

    public void StartTimeline()
    {
        timelineToPlay.Play();
    }
}

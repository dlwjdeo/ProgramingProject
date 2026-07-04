using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : Singleton<TimelineController>
{
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private PlayableAsset EndingStartTimeline;
    [SerializeField] private PlayableAsset EndingTimeline;

    public void PlayEndingStartTimeline()
    {
        playableDirector.playableAsset = EndingStartTimeline;
        playableDirector.Play();
    }

    public void PlayEndingTimeline()
    {
        playableDirector.playableAsset = EndingTimeline;
        playableDirector.Play();
    }   
}

using STAR.Utils;
using StarCooperation.ExportCCP;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TimelineControl : MonoBehaviour
{
    public GameObject controller;
    public ToggleGroup Group;
    public int MainTrackIndex = 1;
    public Toggle toggle;
    public GameEvent camEvent;

    private PlayableDirector director;
    private int ClipState = 0;
    private List<TimelineClip> clips;

    void Awake()
    {
        director = controller.GetComponent<PlayableDirector>();
        CreateMarkers();
        clips = GetRootClips();
        director.paused += TriggerToggleOff;
        director.played += TriggerToggleOn;
    }

    private void TriggerToggleOff(PlayableDirector adirector)
    {
        if(toggle.isOn)
            toggle.SetIsOnWithoutNotify(false);
    }
    private void TriggerToggleOn(PlayableDirector adirector)
    {
        if (!toggle.isOn)
            toggle.SetIsOnWithoutNotify(true);
    }

    void Update()
    {
        //GetCurrentlyPlayingClip();
    }

    public void StartTimeline()
    {
        if (director.state == PlayState.Paused)
        {
            director.Play();
        }
        else
        {
            director.Pause();
        }
    }

    public void SkipBackward()
    {
        //if (ClipState < clips.Count) ClipState--;
        //director.time = clips[ClipState].start;
        director.time -= 10;
        director.Play();
    }

    public void SetClip()
    {
        List<Toggle> toggleActive = Group.ActiveToggles().ToList();
        if (toggleActive.Count > 0) {
            if (ClipState < clips.Count)
                ClipState = toggleActive.First().gameObject.GetComponent<UIComponent_Step>().listPosition;
            director.time = clips[ClipState].start;
            director.Play();
        }
    }

    public void SkipForward()
    {
        //if(ClipState < clips.Count) ClipState++;
        //director.time = clips[ClipState].start;
        director.time += 10;
        director.Play();

    }

    private void CreateMarkers()
    {
        /*foreach(TimelineClip clip in clips)
        {
        IEnumerable<IMarker> marker = root.GetMarkers();
            //clip.
        }*/
    }

    private List<TimelineClip> GetRootClips()
    {
        TimelineAsset timeline = (TimelineAsset)director.playableAsset;
        TrackAsset root = timeline.GetRootTrack(MainTrackIndex);
        IEnumerable<TimelineClip> clips = root.GetClips();
        List<TimelineClip> clipList = clips.ToList();
        clipList = SortClips(clipList);
        return clipList;
    }

    private List<TimelineClip> SortClips(List<TimelineClip> clips)
    {
        var n = clips.Count;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (clips[j].start > clips[j + 1].start)
                {
                    var tempVar = clips[j];
                    clips[j] = clips[j + 1];
                    clips[j + 1] = tempVar;
                }
        return clips;
    }

    private int GetCurrentlyPlayingClip()
    {
        double time = director.time;
        int index = 0;

        foreach(TimelineClip clip in clips)
        {
            if ((time > clip.start) && (time < clip.end))
            {
                break;
            }
            index++;
            if(((float)time+0.1) == ((float)clip.end+0.1))
                Group.GetToggles().ToList()[index].isOn = true;
        }
        return index;
    }

    public void ResetRootTime()
    {
        if (director != null && director.gameObject.activeSelf == true)
        {
            director.time = 1;
            if (director.playableGraph.IsValid())
                director.playableGraph.Evaluate(1);
            //director.Play();
            director.Pause();
            camEvent.RaiseBool(true);
            //toggle.isOn = false;
        }
    }
}

using StarCooperation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightHandler : MonoBehaviour
{
    public List<ModelHighlighter> ActiveHighlighter;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnEnable()
    {
        ModelHighlighter.OnModelHighlightChanged += ModelHIghlighted;
        ModelHighlighter.OnModelSentTransparent += ModelSetTranspaprent;
        ActiveHighlighter = new List<ModelHighlighter>();

    }
    private void OnDisable()
    {
        ModelHighlighter.OnModelHighlightChanged -= ModelHIghlighted;

    }

    private void ModelSetTranspaprent(ModelHighlighter highlighter)
    {
        if (highlighter.isHighlighted)
        {
            if (ActiveHighlighter.Contains(highlighter))
            {
                //means this exact highlighter is used as highlighter already. So do not add?

            }
            if (!ActiveHighlighter.Contains(highlighter))
                ActiveHighlighter.Add(highlighter);
            var ordered = ActiveHighlighter.OrderBy((x => x.Priority)).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].HighlightWithoutNotification(true);
            }
        }
        else
        {
            //Undo this Highlighter. What does this mean
            //    highlighter.HighlightWithoutNotification(false);
            ActiveHighlighter.Remove(highlighter);

            var newList = ActiveHighlighter.OrderBy(x => x.Priority).ToList();
            for (int i = 0; i < newList.Count; i++)
            {
                newList[i].HighlightWithoutNotification(true);
            }

        }
    }
    private void ModelHIghlighted(ModelHighlighter highlighter)
    {
        if (highlighter.isHighlighted)
        {
            if (!ActiveHighlighter.Contains(highlighter))
                ActiveHighlighter.Add(highlighter);
            var ordered = ActiveHighlighter.OrderBy((x => x.Priority)).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].HighlightWithoutNotification(true);
            }
        }
        else
        {
            //Undo this Highlighter. What does this mean
            //    highlighter.HighlightWithoutNotification(false);
            ActiveHighlighter.Remove(highlighter);

            var newList = ActiveHighlighter.OrderBy(x => x.Priority).ToList();
            for (int i = 0; i < newList.Count; i++)
            {
                newList[i].HighlightWithoutNotification(true);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

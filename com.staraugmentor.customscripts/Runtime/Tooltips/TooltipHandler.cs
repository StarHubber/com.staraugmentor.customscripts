using StarCooperation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    public void OnUIClicked(string UiGuid)
    {
        if (UiGuid == GetComponent<Tooltip>().CorrespondingUIElement.Guid)
        {
            //this is me
            GetComponent<Tooltip>().HighlighterStateChanged(!GetComponent<Tooltip>().isHighlighted);

        }
        else
        {
            GetComponent<Tooltip>().HighlighterStateChanged(false);

        }
    }

    public void OnTooltipClicked(string Guid)
    {
        if (Guid == GetComponent<Tooltip>().CorrespondingUIElement.Guid)
        {
            //this is me
            GetComponent<Tooltip>().HighlighterStateChanged(!GetComponent<Tooltip>().isHighlighted);

        }
        else
        {
            GetComponent<Tooltip>().HighlighterStateChanged(false);
            //  GetComponent<Tooltip>().SetState(TooltipState.Hotspot);

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
    public class TooltipGroupMember : MonoBehaviour
    {
		public static List<TooltipGroupMember> allTooltipGroupMembers = new List<TooltipGroupMember>();

		private void Awake()
		{
			allTooltipGroupMembers.Add(this);
        }

		private void OnDestroy()
		{
			allTooltipGroupMembers.Remove(this);
		}
	}
}

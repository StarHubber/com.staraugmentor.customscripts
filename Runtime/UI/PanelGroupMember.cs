using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
    public class PanelGroupMember : MonoBehaviour
    {
		public static List<PanelGroupMember> allPanelGroupMembers = new List<PanelGroupMember>();

        private void Awake()
        {
			allPanelGroupMembers.Add(this);
        }

		private void OnDestroy()
		{
			allPanelGroupMembers.Remove(this);
		}
	}
}

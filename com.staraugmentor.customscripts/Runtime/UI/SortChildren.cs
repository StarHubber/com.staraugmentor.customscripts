using StarCooperation.LegacyLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class SortChildren : MonoBehaviour
	{
		public enum SortingMethod
		{
			Alphabetically
		}

		public SortingMethod sortingMethod = SortingMethod.Alphabetically;  // not used atm, but for clarity and further extensions

		// Start is called before the first frame update
		private void Start()
		{
			Sort();
			Localizer.OnTextsUpdated += Sort;
		}

		private void OnDestroy()
		{
			Localizer.OnTextsUpdated -= Sort;
		}

		private void Sort()
		{
			// Skip everything if panel is not subPanel
			var childrenSorters = GetComponentsInChildren<SortChildren>(true);
			if (childrenSorters.Length > 1) // Not just this instance found, but actual children
			{
				return;
			}

			var namedChildren = new Dictionary<string, Transform>();
			var sortedNames = new List<string>();
			bool hasNonToggleElement = false;

			// Get list of childrens' names and dictionary of names/Transforms
			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				if (!child.GetComponent<Toggle>())
				{
					hasNonToggleElement = true;	// Compensate for BackButton
					continue;
				}
				var localizer = child.GetComponentInChildren<LocalizedTextBase>();
				if (localizer != null)
				{
					var text = localizer.GetText();
					if (string.IsNullOrEmpty(text))
					{
						continue;
					}
					sortedNames.Add(text);
					namedChildren.Add(text, child);
				}
			}

			// Sort names alphabetically
			sortedNames.Sort();

			// Start from index 0 and set sibling indices (needs to run in order)
			for (int i = 0; i < sortedNames.Count; i++)
			{
				var newIdx = hasNonToggleElement ? i + 1 : i;	// + 1 when BackButton is there.
				namedChildren[sortedNames[i]].SetSiblingIndex(newIdx);
			}
		}
	}
}
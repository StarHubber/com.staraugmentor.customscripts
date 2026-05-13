using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class ToggleGroupController : MonoBehaviour
{
	[Header("Group changer Toggle")]
	public Toggle groupLogicChanger;
	public bool logicChangerDisablesGroup = true;
	public bool logicChangerTicksAllOn = true;

	private ToggleGroup tGroup;
	private List<Toggle> groupToggles;

	private bool allOn = false;

	private void Awake()
	{
		tGroup = GetComponent<ToggleGroup>();
	}

	// Start is called before the first frame update
	private void Start()
	{
		groupToggles = new List<Toggle>(tGroup.GetToggles());

		/////
		// Assign the main callback for logicChanger toggle
		/////
		groupLogicChanger.onValueChanged.AddListener(isOn =>
		{
			// If unticked, remove listeners in groupToggles
			if (!isOn)
			{
				UntickLogicChangerOnNextToggleClick(false);
			}

			// Enable/disable toggleGroup
			EnableToggleGroup(logicChangerDisablesGroup ? !isOn : isOn);

			if (logicChangerTicksAllOn)
			{
				// If unticked, set all toggles false except the one that was clicked (if not clicked on logicChanger directly)
				if (!isOn)
				{
					// The toggle that was clicked to be highlighted, must be set to on after all others are set off
					Toggle clickedToggle = null;
					foreach (var toggle in groupToggles)
					{
						if (!toggle.isOn)
						{
							clickedToggle = toggle;
						}
					}

					// Set all toggles off and set the calling toggle on, if exists
					SetAllToggles(false);
					if (clickedToggle != null)
					{
						clickedToggle.isOn = true;
					}
				}

				// If ticked, set all toggles on
				else
				{
					SetAllToggles(true);
				}
			}

			// If ticked, untick at next click on any groupToggle
			if (isOn)
			{
				UntickLogicChangerOnNextToggleClick(true);
			}
		});
	}

	/// <summary>
	/// Assign or remove event to untick logicChanger toggle at next click on any groupToggle.
	/// </summary>
	/// <param name="doIt"></param>
	private void UntickLogicChangerOnNextToggleClick(bool doIt)
	{
		foreach (var toggle in groupToggles)
		{
			if (doIt)
			{
				toggle.onValueChanged.AddListener(UntickLogicChanger);
			}
			else
			{
				toggle.onValueChanged.RemoveListener(UntickLogicChanger);
			}
		}
	}

	/// <summary>
	/// Actual untick function, needs to be boolean function to be assigned and removed (not delegate, can't be removed).
	/// </summary>
	/// <param name="untick"></param>
	private void UntickLogicChanger(bool untick)
	{
		groupLogicChanger.isOn = false;
	}

	/// <summary>
	/// Enable/disable toggle group.
	/// </summary>
	/// <param name="enable"></param>
	public void EnableToggleGroup(bool enable)
	{
		if (!enable && tGroup.Count() != 0)
		{
			foreach (var toggle in groupToggles)
			{
				tGroup.UnregisterToggle(toggle);
				toggle.group = null;
			}
		}
		else if (enable && tGroup.Count() == 0)
		{
			foreach (var toggle in groupToggles)
			{
				//toggle.isOn = false;
				toggle.group = tGroup;
				tGroup.RegisterToggle(toggle);
			}
		}
	}

	/// <summary>
	/// Set all toggles on/off. ToggleGroup gets disabled/re-enabled automatically.
	/// </summary>
	/// <param name="on"></param>
	public void SetAllToggles(bool on)
	{
		allOn = on;
		foreach (var toggle in groupToggles)
		{
			toggle.isOn = on;
		}
	}
}

public static class ToggleGroupExtensions
{
	// from https://forum.unity.com/threads/how-to-get-reference-of-all-toggles-from-togglegroup.463534/
	private static System.Reflection.FieldInfo _toggleListMember;

	/// <summary>
	/// Gets the list of toggles. Do NOT add to the list, only read from it.
	/// </summary>
	/// <param name="grp"></param>
	/// <returns></returns>
	public static IList<Toggle> GetToggles(this ToggleGroup grp)
	{
		if (_toggleListMember == null)
		{
			_toggleListMember = typeof(ToggleGroup).GetField("m_Toggles",
				System.Reflection.BindingFlags.Instance |
				System.Reflection.BindingFlags.NonPublic |
				System.Reflection.BindingFlags.GetField);
			if (_toggleListMember == null)
			{
				throw new System.Exception("UnityEngine.UI.ToggleGroup source code must have changed in latest version and is no longer compatible with this version of code.");
			}
		}
		return _toggleListMember.GetValue(grp) as IList<Toggle>;
	}

	public static int Count(this ToggleGroup grp)
	{
		return GetToggles(grp).Count;
	}

	public static Toggle Get(this ToggleGroup grp, int index)
	{
		return GetToggles(grp)[index];
	}

}

using System.Collections.Generic;
using UnityEngine.Events;

namespace StarCooperation
{
    public interface IAutoConfigurableSceneManager
    {
		List<ToggleListener> highlighterToggles { get; }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
namespace StarCooperation.ExportCCP
{
    [System.Serializable]
    public class UITab
    {
        public string Guid;
        public int listPosition;
        public List<UIStep> StepList;
        public Dictionary<string, string> NamesDictionary;
        public StepDetail StepShape;

        public UITab()
        {

        }
        public UITab(string Guid, int listpos, List<UIStep> StepList, Dictionary<string, string> namesDict, StepDetail detail)
        {
            this.StepShape = detail;
            this.NamesDictionary = namesDict;
            this.Guid = Guid;
            this.listPosition = listpos;
            this.StepList = StepList;
          //  StepList = tab.StepList.Select(step => new UIStep(step.Guid, step.listPosition, , step.NamesDictionary, step.StepShape)).ToList();
        }

    }
}

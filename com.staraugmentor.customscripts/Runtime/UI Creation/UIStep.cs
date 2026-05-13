using System.Collections.Generic;
namespace StarCooperation.ExportCCP
{
    public class UIStep
    {
        public string Guid;
        public int listPosition;
        public List<UIStep> SubSteps;
        public Dictionary<string, string> NamesDictionary, InfoDictionary;
        public StepDetail StepShape;
        public UIStep()
        {

        }
        public UIStep(string guid, int listPos, List<UIStep> subSteps, Dictionary<string, string> namesDict, Dictionary<string, string> InfoDict, StepDetail detail)
        {
            this.Guid = guid;
            this.listPosition = listPos;
            this.SubSteps = subSteps;
            this.NamesDictionary = namesDict;
            this.StepShape = detail;
            this.InfoDictionary = InfoDict;
        }
    }
}
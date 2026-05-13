using StarCooperation;
using StarCooperation.ExportCCP;
using StarCooperation.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





[System.Serializable]
public class SerializableInteractorData
{
    [SerializeField]
    public new Dictionary<string, GameObject> Data;

    public new void AddObj(string key, GameObject obj)
    {
        if (Data is null) Data = new Dictionary<string, GameObject>();

        Data.Add(key, obj);
    }

    public SerializableInteractorData(string guid, string key, GameObject obj)
    {

    }
}
public class InteractorCreator : MonoBehaviour
{
    public Transform tabHolder, toggleHolder;
    [SerializeField] private List<GameObject> MainToggles;
    private List<ToggleListener> highlighterToggles = new List<ToggleListener>();
    [SerializeField] private GameObject tooltipsHolder;
    [SerializeField] public GameObject viewerInterface;
    [SerializeField] public GameObject komponentenMain;
    [SerializeField] private GameObject particlesHolder;
    public List<SerializableInteractorData> Data { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Data = new List<SerializableInteractorData>();
        //fetch all objects with guid for custom content.
        foreach (Transform komp in tabHolder.GetComponentsInChildren<Transform>(true))
        {
            komp.TryGetComponent<UIComponent_Tab>(out var comp);
            if (comp != null) MainToggles.Add(komp.gameObject);
        }
        foreach (Transform komp in toggleHolder.GetComponentsInChildren<Transform>(true))
        {
            komp.TryGetComponent<UIComponent_Step>(out var comp);
            if (comp != null) MainToggles.Add(komp.gameObject);
        }


        highlighterToggles.AddRange(toggleHolder.GetComponentsInChildren<ToggleListener>());
        highlighterToggles.RemoveAll(t => t.highlighter == null);

        for (var i = 0; i < highlighterToggles.Count; i++)
        {
            var idx = i;
            highlighterToggles[i].GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
            {
                SetHighlighterState(new IndexedToggleState { index = idx, state = isOn });
            });
        }

        //Generates the data for interface.
        StarCooperation.Interface.DataInterface.DATAObject = new StarCooperation.Interface.DataInterface();
        foreach (var komp in MainToggles)
        {
            komp.transform.TryGetComponent<UIComponent_Step>(out var stepChild);
            if (stepChild != null)
            {
                SerializableInteractorData actor = new SerializableInteractorData(komp.transform.GetComponent<UIComponent_Step>().Guid, "Toggles", komp.transform.gameObject);
                /*komp.transform.TryGetComponent<LupenHandler>(out var tooltipObj);
                if (tooltipObj != null)
                {*/
                foreach (var tool in stepChild.Toggles)
                    //actor.AddObj(tool.GetComponent<GuidComponent>().Guid, tool.transform.gameObject);
                //}
                Data.Add(actor);

            }
            else
            {

            }
        }
    }
    private ModelHighlighter GetHighlighterFromIndex(int index)
    {
        if (index < 0 || index >= highlighterToggles.Count)
        {
            Debug.LogError($"Highlighter index out of bounds ({index})");
            return null;
        }
        return highlighterToggles[index].highlighter;
    }

    public void SetHighlighterState(IndexedToggleState highlighterState)
    {
        var target = GetHighlighterFromIndex(highlighterState.index);
        if (target == null)
        {
            Debug.LogError("Highlighter missing, cannot set state", this);
            return;
        }
        HighlightModelPart(highlighterState.state, target);
    }
    public virtual void HighlightModelPart(bool doHighlight, ModelHighlighter highlighter)
    {
        highlighter.Highlight(doHighlight);
    }

    // Update is called once per frame
    void Update()
    {

    }



}

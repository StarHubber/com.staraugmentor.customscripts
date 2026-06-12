using StarCooperation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScenario : MonoBehaviour
{
    public ModelHighlighter TransparentHighlighter;
    public List<Transform> ParticleSystems;
    public Transform CameraDefault;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateScenario(bool toggle)
    {
        ParticleSystems.ForEach(x => x.gameObject.SetActive(toggle));
        TransparentHighlighter.Highlight(toggle);

        /*if (toggle)
        {
            FindObjectOfType<MoveCamera>().SetNewCameraLocation(CameraDefault);
        }*/

    }
}

using UnityEngine;
//[ExecuteAlways]
public class RevealFlowController : MonoBehaviour
{
    [HideInInspector] public bool IsDone = false;
    [HideInInspector] public float SpeedMultiplier = 1f;

    [SerializeField] private Renderer targetMaterial;
    [SerializeField] private string progressProperty = "_Progress";
    [SerializeField] public float progress = 0f;
    [SerializeField, Range(0.1f, 10f)] public float duration = 3f;
    [SerializeField, Range(0f, 10f)] private float delay = 0f;

    private float speed;
    private float timer = 0f;
    private float durationDelay = 0f;
    private bool isRevealing = false;
    private bool isStopped = false;
    private Material instanceMaterial;

    public void BeginReveal()
    {
        progress = 0f;
        durationDelay = duration + delay;
        speed = SpeedMultiplier / durationDelay;
        IsDone = false;
        isRevealing = false;
        timer = 0f;
        isStopped = false;
    }
    public void StopReveal()
    {
        isStopped = true;
        isRevealing = false;
        IsDone = false; // Optional: Kann auf true gesetzt werden, wenn du willst, dass es "beendet" ist
    }

    private void Start()
    {
        if (targetMaterial == null)
        {
            Debug.LogError("RevealFlowController: Kein Material zugewiesen!");
            enabled = false;
            return;
        }


        // Zuweisung an Renderer
        var renderer = GetComponent<Renderer>();
        //renderer.sortingOrder = 10;
        instanceMaterial = renderer.material;

        instanceMaterial.SetFloat(progressProperty, 0f);
    }

    private void Update()
    {
        if (isStopped || IsDone) return;

        if (!isRevealing)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                isRevealing = true;
                timer = 0f;
            }
        }

        if (IsDone) return;

        progress += speed * Time.deltaTime;

        if (progress >= 1f)
        {
            progress = 1f;
            IsDone = true;
            isRevealing = false; // Neue Verzögerung starten
        }

        instanceMaterial.SetFloat(progressProperty, progress);
    }
}

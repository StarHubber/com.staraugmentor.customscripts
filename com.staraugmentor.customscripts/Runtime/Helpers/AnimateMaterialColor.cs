using UnityEngine;

[ExecuteAlways]
public class AnimateMaterialColor : MonoBehaviour
{
    public Material targetMaterial;

    public Color Ping;
    public Color Pong;
    public int durchlaeufe;
    public bool HoldColor;

    float duration = 0.5f; // ein kompletter PingPong-Zyklus (hin und zurück)
    float amplitude = 1f;  // maximale Auslenkung

    private float timer = 0f;
    private bool isDone = false;
    private float fullduration = 0f;

    private void Update()
    {
        //if (isDone) return;

        fullduration = duration * durchlaeufe;
        timer += Time.deltaTime;


        /*if (timer >= fullduration)
        {
            timer = fullduration;
            isDone = true;
        }*/

        if (targetMaterial != null && !HoldColor)
        {
            var pingPong = Mathf.PingPong(timer * (2f * amplitude / duration), amplitude);
            var color = Color.Lerp(Ping, Pong, pingPong);
            targetMaterial.color = color;
        }
        else
        {
            targetMaterial.color = Pong;
        }
    }

    private void OnDisable()
    {
        targetMaterial.color = Ping;
        timer = 0f;
        isDone = false;
    }
}

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlackScreenController : MonoBehaviour
{
    [SerializeField] FullScreenPassRendererFeature fullScreen;
    [SerializeField] RenderObjects[] RenderObjects;
    // Update is called once per frame
    public void CallBlackScreen(bool enabled)
    {
        foreach(var render in RenderObjects)
        {
            fullScreen.SetActive(enabled);
            render.SetActive(enabled);
        }
    }

}

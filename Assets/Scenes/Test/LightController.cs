
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class LightController : MonoBehaviour
{
    [SerializeField] float[] LightSize = { 3, 5, 8, 10 };

    int a = 0;
    [SerializeField] Light2D light;

    public void SetLight(int num)
    {
        if (num > LightSize.Length) return;
        float outer = LightSize[num];
        light.pointLightOuterRadius = outer;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4)) { SetLight(a++); }
    }
}

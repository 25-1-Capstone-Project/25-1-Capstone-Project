
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class LightController : MonoBehaviour
{
    [SerializeField] float[] LightSize = { 0, 4, 8, 10 };

    [SerializeField] Light2D light;

    public void SetLight(int num)
    {
        if (num > LightSize.Length) return;
        float outer = LightSize[num];
        light.pointLightOuterRadius = outer;
    }
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.F4)) { SetLight(a++); }
    //       if (Input.GetKeyDown(KeyCode.F5)) { SetLight(a--); }
    // }
}

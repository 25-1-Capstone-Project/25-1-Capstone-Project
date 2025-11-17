using UnityEngine;
using UnityEngine.UI;

public class UI_Option : MonoBehaviour
{
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider MasterSlider;
    public void SetBGMVolume()
    {
        AudioManager.Instance.SetBGMVolume(BGMSlider.value);
    }
    public void SetSFXVolume()
    {
        AudioManager.Instance.SetSFXVolume(SFXSlider.value);
    }
    public void SetMasterVolume()
    {
        AudioManager.Instance.SetMasterVolume(MasterSlider.value);
    }
}

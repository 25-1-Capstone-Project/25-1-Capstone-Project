using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// 페이드 아웃 및 페이드 인을 담당하는 스크립트입니다.
/// 씬전환에 활용중입니다.
/// </summary>
public class FadeController : Singleton<FadeController>
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public IEnumerator FadeOut(Color color, float delay, float maxAlpha = 1f)
    {
        //PlayerInputBlocker.Block(true);
        float time = 0;
        color.a = 0;
        fadeImage.color = color;
        
        while (time < delay)
        {

            color.a = Mathf.Lerp(0, maxAlpha, time / delay);
            fadeImage.color = color;
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        color.a = 1;
        fadeImage.color = color;
    }

    public IEnumerator FadeIn(Color color, float delay, float maxAlpha = 1f)
    {
        float time = 0;
        color.a = maxAlpha;
        fadeImage.color = color;
       

        while (time < delay)
        {
            color.a = Mathf.Lerp(maxAlpha, 0, time / delay);
            fadeImage.color = color;
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        color.a = 0;
        fadeImage.color = color;
        //PlayerInputBlocker.Block(false);
    }
  
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn(Color.black, fadeDuration));
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
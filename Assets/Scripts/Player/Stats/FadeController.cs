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
    public IEnumerator FadeOut()
    {
        PlayerInputBlocker.Block(true);
        float time = 0;
        Color color = fadeImage.color;
        while (time < fadeDuration)
        {

            color.a = Mathf.Lerp(0, 1, time / fadeDuration);
            fadeImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        fadeImage.color = color;


    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }
    public IEnumerator FadeIn()
    {
        float time = 0;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            color.a = Mathf.Lerp(1, 0, time / fadeDuration);
            fadeImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        color.a = 0;
        fadeImage.color = color;
        PlayerInputBlocker.Block(false);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
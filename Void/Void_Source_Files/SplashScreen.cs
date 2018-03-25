using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplashScreen : MonoBehaviour
{
    [SerializeField]
    private AudioClip ping;

    [SerializeField]
    private Image splashImage;

    private float fadeSpeed = 0.01f;
    private float delay = 0.01f;

    void Awake()
    {
        StartCoroutine(FadeIn());
    }

    void Start()
    {
        AudioManager.instance.StopMusic();
    }

    IEnumerator FadeIn()
    {
        while (splashImage.color.a < 1f)
        {
            splashImage.color = new Color(splashImage.color.r, splashImage.color.g, splashImage.color.b, splashImage.color.a + fadeSpeed);
            yield return new WaitForSeconds(delay);
        }

        AudioManager.instance.PlaySingle(false, ping);
        splashImage.color = new Color(splashImage.color.r, splashImage.color.g, splashImage.color.b, 1f);
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (splashImage.color.a > 0f)
        {
            splashImage.color = new Color(splashImage.color.r, splashImage.color.g, splashImage.color.b, splashImage.color.a - fadeSpeed);
            yield return new WaitForSeconds(delay);
        }

        splashImage.color = new Color(splashImage.color.r, splashImage.color.g, splashImage.color.b, 0f);
        yield return null;

        Application.LoadLevel(1);
    }
}

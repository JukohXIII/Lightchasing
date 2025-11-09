using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Image fadeImage;
    public float fadeDuration = 1f;

    void Awake()
    {
        Instance = this;
        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator FadeToBlack()
    {
        fadeImage.gameObject.SetActive(true);

        Color c = fadeImage.color;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; 
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System; // コルーチンを使用するために必要

public class FadeManager : MonoBehaviour
{
    private Image fadePanelImage;
    [SerializeField] private float fadeTime = 1.0f;

    private void Awake()
    {
        fadePanelImage = GetComponent<Image>();
        // 初期状態では透明に設定
        fadePanelImage.color = new Color(0f, 0f, 0f, 0f);
    }

    public void StartFadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }
    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        float timer = 0f;
        
        // ImageのAlpha値を徐々に1（不透明）に近づける
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeTime);
            fadePanelImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null; // 1フレーム待機
        }

        // フェードアウト完了（完全に黒くなった）
        fadePanelImage.color = new Color(0f, 0f, 0f, 1f);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        
        // ImageのAlpha値を徐々に0（透明）に近づける
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            fadePanelImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null; // 1フレーム待機
        }

        // フェードイン完了（完全に透明になった）
        fadePanelImage.color = new Color(0f, 0f, 0f, 0f);

        fadePanelImage.gameObject.SetActive(false);
    }
}
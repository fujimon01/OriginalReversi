using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableCell : MonoBehaviour
{
    [Header("Blink Settings")]
    public float blinkInterval = 1.0f; // 点滅の間隔（秒）
    public float minAlpha = 0.2f;      // 最小アルファ値（透明度）
    public float maxAlpha = 1.0f;      // 最大アルファ値（不透明度）

    private Renderer cellRenderer;
    private Material cellMaterial;
    public int x;
    public int y;
    private bool isBlinking = false; // 現在点滅中かどうか

    void Awake()
    {
        cellRenderer = GetComponent<Renderer>();
        if (cellRenderer == null)
        {
            Debug.LogError("BlinkingCell requires a Renderer component!", this);
            enabled = false; // Rendererがない場合はスクリプトを無効化
            return;
        }

        cellMaterial = cellRenderer.material; // インスタンス化されたマテリアルを取得
        // 初期状態は最大アルファ値で表示
        SetMaterialAlpha(maxAlpha);
    }
    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // 点滅を開始する public メソッド
    public void StartBlink()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkRoutine());
        }
    }

    // 点滅を停止する public メソッド
    public void StopBlink()
    {
        if (isBlinking)
        {
            isBlinking = false;
            StopAllCoroutines(); // 実行中のコルーチンを全て停止
            SetMaterialAlpha(0f); // 非表示にする（完全に透明にする）
            gameObject.SetActive(false); // オブジェクト自体を非アクティブにする
        }
    }

    // 点滅のコルーチン
    private IEnumerator BlinkRoutine()
    {
        while (isBlinking)
        {
            // 透明にする
            yield return StartCoroutine(FadeAlpha(maxAlpha, minAlpha, blinkInterval / 2f));
            // 不透明にする
            yield return StartCoroutine(FadeAlpha(minAlpha, maxAlpha, blinkInterval / 2f));
        }
    }

    // アルファ値を徐々に変更するコルーチン
    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        Color currentColor = cellMaterial.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            SetMaterialAlpha(currentAlpha);
            yield return null;
        }
        SetMaterialAlpha(endAlpha); // 最後に目標アルファ値を確実に設定
    }

    // マテリアルのアルファ値を設定するヘルパーメソッド
    private void SetMaterialAlpha(float alpha)
    {
        Color color = cellMaterial.color;
        color.a = alpha;
        cellMaterial.color = color;
    }

    // オブジェクトが破棄されるときにマテリアルのインスタンスを解放（Optionalだが推奨）
    void OnDestroy()
    {
        if (cellMaterial != null)
        {
            Destroy(cellMaterial);
        }
    }
}

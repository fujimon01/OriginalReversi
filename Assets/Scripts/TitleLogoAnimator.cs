using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TitleLogoPulsing : MonoBehaviour
{
    private Image titleImage;

    [Header("永続アニメーション設定")]
    public float targetScale = 1.0f;        // 通常時のスケール（静止点）
    public float loopScale = 1.05f;         // 脈動時の最大スケール
    public float loopDuration = 2.0f;       // 1往復（拡大→縮小）にかける時間
    public int loopCount = -1;              // -1 = 無限ループ

    void Awake()
    {
        titleImage = GetComponent<Image>();
        if (titleImage == null)
        {
            Debug.LogError("Imageコンポーネントが見つかりません。");
            return;
        }

        // 初期設定：透明度を完全に表示（1.0）にし、スケールを静止点に設定
        Color initialColor = titleImage.color;
        initialColor.a = 1f; // 完全に出現した状態
        titleImage.color = initialColor;
        
        transform.localScale = Vector3.one * targetScale; 

        // 永続アニメーションを開始
        PlayPulsingAnimation();
    }

    private void PlayPulsingAnimation()
    {
        // 既存のTweenが残っている場合はキルしておく
        DOTween.Kill(transform);

        // スケールをtargetScaleからloopScaleへ、そしてまたtargetScaleへ戻るアニメーション
        // 無限ループ (SetLoops(-1)) と往復 (LoopType.Yoyo) を使用
        transform.DOScale(loopScale, loopDuration)
                 .SetEase(Ease.InOutSine) // 滑らかな拡大・縮小
                 .SetLoops(loopCount, LoopType.Yoyo) // Yoyoで往復（拡大→縮小）を繰り返す
                 .SetId(this); // このTweenを一意に識別するためのIDを設定 (停止時に便利)
    }

    /// <summary>
    /// シーンが切り替わる際など、アニメーションを停止したいときに呼び出す
    /// </summary>
    private void OnDestroy()
    {
        // オブジェクトが破棄されるときに、対応するTweenも停止させる
        DOTween.Kill(this);
    }
}
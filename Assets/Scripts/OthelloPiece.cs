using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OthelloPiece : MonoBehaviour
{
    public int x;
    public int y;

    OthelloField.State state = OthelloField.State.None;

    [Header("Flip Animation Settings")]
    public float flipDuration = 0.3f;
    public float liftHeight = 1.0f;
    public float liftDuration = 0.1f;
    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetStateImmediately(OthelloField.State newState)
    {
        this.state = newState;
        UpdateVisuals(newState);
    }

    public void FlipAndSetState(OthelloField.State newState)
    {
        if (state == newState || state == OthelloField.State.None)
        {
            SetStateImmediately(newState);
            return;
        }

        transform.DOKill(true);

        // 新しい状態を先に設定
        this.state = newState;

        // 目標の回転角度
        float targetZRotation = (newState == OthelloField.State.White) ? 180f : 0f;
        
        // 180度回転させるための相対角度
        Vector3 flipAngle = new Vector3(0, 0, 180); 

        // シーケンスの作成
        Sequence flipSequence = DOTween.Sequence();
        
        // 1. 【浮き上がり】: Y軸方向に持ち上げる (Append)
        flipSequence.Append(
            // transform.localPosition.y は元の高さ
            transform.DOLocalMoveY(transform.localPosition.y + liftHeight, liftDuration)
                     .SetEase(Ease.OutSine) 
        );

        // 2. 【回転＆降下】: 回転と同時に元の高さに戻る (Join)
        flipSequence.Join(
            // 回転アニメーション
            transform.DORotate(flipAngle, flipDuration, RotateMode.LocalAxisAdd)
                     .SetEase(Ease.InOutSine) 
        );
        flipSequence.Join(
            // 降下アニメーション（元の高さに戻る）
            transform.DOLocalMoveY(transform.localPosition.y, flipDuration) 
                     .SetEase(Ease.InSine)
        );

        // 3. 【完了処理】: アニメーション完了後、即座に見た目（色/テクスチャ）を切り替える
        flipSequence.OnComplete(() =>
        {
            // アニメーション完了時、目標の回転角度にセットし直す（誤差防止）
            transform.localRotation = Quaternion.Euler(0, 0, targetZRotation); 
            
            // 重要な修正点: アニメーション完了後、状態に応じた見た目に切り替える
            UpdateVisuals(this.state);
            Debug.Log($"Piece at ({x},{y}) flipped to {this.state}");
        });
        
        flipSequence.Play();
    }

    public void UpdateVisuals(OthelloField.State state)
    {
        if(state == OthelloField.State.White)
        {
            gameObject.SetActive(true);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);       
        }
        else if(state == OthelloField.State.Black)
        {
            gameObject.SetActive(true);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public OthelloField.State GetState()
    {
        return state;
    }
}

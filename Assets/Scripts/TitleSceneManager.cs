using UnityEngine;
using UnityEngine.UI; // CanvasGroupを使うために必要
using UnityEngine.SceneManagement; // シーン遷移を使うために必要
using System.Collections;
using Unity.VisualScripting;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject fadePanel;
    // タイトルロゴ (SpriteRenderer) の参照
    public Image titleLogo; 
    
    // スタート画面のUIグループ (CanvasGroup) の参照
    public CanvasGroup startScreenGroup; 

    public CanvasGroup difficultySelectGroup;

    // 色選択画面のUIグループ (CanvasGroup) の参照
    public CanvasGroup colorSelectGroup;

    // 白ボタンと黒ボタンを押した時に呼ばれるメソッド
    // string playerColor: 選択された色 ("White" または "Black")

    private void Start()
    {
        // フェードインを開始するコルーチンを呼び出す
        fadePanel.GetComponent<FadeManager>().StartFadeIn();
    }
    public void SelectColorAndLoadScene(string playerColor)
    {
        Debug.Log("選択された色: " + playerColor);
        PlayerPrefs.SetString("PlayerColor", playerColor); 
        fadePanel.SetActive(true);
        // フェードアウトを開始し、OthelloSceneをロードするコルーチンを呼び出す
        fadePanel.GetComponent<FadeManager>().StartFadeOutAndLoadScene("OthelloScene");
    }

    public void SelectDifficulty(string difficulty)
    {
        Debug.Log("選択された難易度: " + difficulty);
        PlayerPrefs.SetString("Difficulty", difficulty); 

        // 難易度選択画面のUIグループを非表示・操作不可にする
        SetCanvasGroupActive(difficultySelectGroup, false);

        // 色選択画面のUIグループを表示・操作可能にする
        SetCanvasGroupActive(colorSelectGroup, true);
    }

    // スタートボタンが押された時に呼ばれるメソッド
    public void OnStartButtonClicked()
    {
        // 1. タイトルロゴ (Image) を非表示にする
        if (titleLogo != null)
        {
            titleLogo.enabled = false;
        }

        // 2. スタート画面のUIグループを非表示・操作不可にする
        SetCanvasGroupActive(startScreenGroup, false);

        // 3. 色選択画面のUIグループを表示・操作可能にする
        //SetCanvasGroupActive(colorSelectGroup, true);
        SetCanvasGroupActive(difficultySelectGroup, true);
    }
    
    // CanvasGroupの表示・操作可否を切り替えるヘルパーメソッド
    private void SetCanvasGroupActive(CanvasGroup group, bool isActive)
    {
        if (group != null)
        {
            group.alpha = isActive ? 1f : 0f;
            group.interactable = isActive;
            group.blocksRaycasts = isActive;
            group.gameObject.SetActive(isActive);
        }
    }
}
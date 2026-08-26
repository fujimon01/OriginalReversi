using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ResultSceneManager : MonoBehaviour
{
    // フェードに使用する画面全体を覆うImageコンポーネント
    [SerializeField] private GameObject fadePanel; 
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI pieceCountText;

    private void Start()
    {
        // シーン開始時にフェードインを開始
        fadePanel.GetComponent<FadeManager>().StartFadeIn();
        SetResult();
    }
    public void OnHomeButtonClicked()
    {
        // 他のボタン操作などを一時的に無効化する処理があればここに入れる
        fadePanel.SetActive(true); // フェードパネルを有効化
        // フェードアウトとシーン遷移のコルーチンを開始
        fadePanel.GetComponent<FadeManager>().StartFadeOutAndLoadScene("TitleScene");
    }

    public void SetResult()
    {
        string playerColor = PlayerPrefs.GetString("PlayerColor", "Black");
        string winner = PlayerPrefs.GetString("Winner", "Draw");
        int blackCount = PlayerPrefs.GetInt("BlackCount", 0);
        int whiteCount = PlayerPrefs.GetInt("WhiteCount", 0);
        if(winner == "Draw")
        {
            // 引き分け: 黄色
            winnerText.text = "<color=yellow>Draw</color>";
        }
        else if(winner == playerColor)
        {
            // 勝ち: 赤色
            winnerText.text = "<color=red>You Win!!</color>";
        }
        else
        {
            // 負け: 青色
            winnerText.text = "<color=blue>You Lose...</color>";
        }
        pieceCountText.text = $"<color=black>Black: {blackCount}</color>\n<color=white>White: {whiteCount}</color>";
    }

}
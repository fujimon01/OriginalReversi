using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private TextMeshProUGUI pieceCountText;
    [SerializeField] private TextMeshProUGUI currentPlayerText;
    [SerializeField] private OthelloField othelloField;
    [SerializeField] private GameObject playerGlowFrame;
    [SerializeField] private GameObject enemyGlowFrame;
    [SerializeField] private AudioClip playerPlaceSound;
    [SerializeField] private AudioClip enemyPlaceSound;
    private OthelloField.State playerColor;
    private OthelloField.State currentPlayer;
    private bool isPlaceing = false;
    private OthelloField.Difficulty enemyDifficulty;
    // Start is called before the first frame update
    void Start()
    {
        fadePanel.GetComponent<FadeManager>().StartFadeIn();
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentPlayer == playerColor) // 左クリックを検知し、現在のプレイヤーが人間プレイヤーの場合のみ処理
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // レイキャストを実行
            if (Physics.Raycast(ray, out hit))
            {
                HandlePlayerInput(hit);
            }
        }
    }

    private void GameStart()
    {
        playerColor = (PlayerPrefs.GetString("PlayerColor","Black") == "Black") ? OthelloField.State.Black : OthelloField.State.White;

        string difficultyString = PlayerPrefs.GetString("Difficulty", "Hard");
        Debug.Log("Selected Difficulty: " + difficultyString);
        switch (difficultyString)
        {
            case "Easy":
                enemyDifficulty = OthelloField.Difficulty.Easy;
                break;
            case "Normal":
                enemyDifficulty = OthelloField.Difficulty.Normal;
                break;
            case "Hard":
            default:
                enemyDifficulty = OthelloField.Difficulty.Hard;
                break;
        }
        Debug.Log("Enemy Difficulty: " + enemyDifficulty.ToString());

        currentPlayer = OthelloField.State.Black;
        UpdateGlowFrame();
        othelloField.InitializeField();
        othelloField.VisualizePlaceableCells(currentPlayer);
        pieceCountText.text = $"<color=black>Black:{othelloField.GetPieceCount(OthelloField.State.Black)}</color>\n<color=white>White:{othelloField.GetPieceCount(OthelloField.State.White)}</color>";
        currentPlayerText.text = "Current Player: " + currentPlayer.ToString();
        if(currentPlayer != playerColor && !isPlaceing)
        {
            StartCoroutine(ProcessEnemyTurn());
        }
    }

    private void UpdateGlowFrame()
    {
        if(currentPlayer == playerColor)
        {
            playerGlowFrame.SetActive(true);
            enemyGlowFrame.SetActive(false);
        }
        else
        {
            playerGlowFrame.SetActive(false);
            enemyGlowFrame.SetActive(true);
        }
    }

    private void HandlePlayerInput(RaycastHit hit)
    {
        if (hit.collider.gameObject.tag == "Placeable")
        {
            //コマを置く処理
            PlaceableCell placeableCell = hit.collider.gameObject.GetComponent<PlaceableCell>();
            SoundManager.Play(playerPlaceSound);
            othelloField.PlacePieceAt(placeableCell.x, placeableCell.y, currentPlayer);
            pieceCountText.text = $"<color=black>Black:{othelloField.GetPieceCount(OthelloField.State.Black)}</color>\n<color=white>White:{othelloField.GetPieceCount(OthelloField.State.White)}</color>";
            othelloField.ClearPlaceableCells();
            CheckGameOver();
        }
    }

    private void SwitchTurn()
    {
        currentPlayer = currentPlayer == OthelloField.State.Black ? OthelloField.State.White : OthelloField.State.Black;
        currentPlayerText.text = "Current Player:" + currentPlayer.ToString();
        UpdateGlowFrame();
        othelloField.VisualizePlaceableCells(currentPlayer);

        if(!othelloField.HasPlaceableCells(currentPlayer))
        {
            currentPlayer = currentPlayer == OthelloField.State.Black ? OthelloField.State.White : OthelloField.State.Black;
            currentPlayerText.text = "Current Player:" + currentPlayer.ToString();
            UpdateGlowFrame();
            othelloField.VisualizePlaceableCells(currentPlayer);
        }

        if(currentPlayer != playerColor && !isPlaceing)
        {
            StartCoroutine(ProcessEnemyTurn());
        }
    }

    private void CheckGameOver()
    {
        if(othelloField.IsGameOver())
        {
            HandleGameOver();
        }
        else
        {
            SwitchTurn(); 
        }
    }

    private void HandleGameOver()
    {
        PlayerPrefs.SetString("Winner", othelloField.DetermineWinner().ToString());
        PlayerPrefs.SetInt("BlackCount", othelloField.GetPieceCount(OthelloField.State.Black));
        PlayerPrefs.SetInt("WhiteCount", othelloField.GetPieceCount(OthelloField.State.White));
        fadePanel.SetActive(true);
        fadePanel.GetComponent<FadeManager>().StartFadeOutAndLoadScene("ResultScene");
    }

    private IEnumerator ProcessEnemyTurn()
    {
        isPlaceing = true;
        // プレイヤーに手番が変わったことが分かるように少し待つ
        yield return new WaitForSeconds(1.0f); 
        List<(int, int)> placeablePositions = othelloField.GetPlaceablePositions(currentPlayer);

        // if (placeablePositions.Count > 0)
        // {
        //     // ランダムに位置を選択
        //     var random = new System.Random();
        //     var (x, y) = placeablePositions[random.Next(placeablePositions.Count)];
        //     SoundManager.Play(enemyPlaceSound);
        //     othelloField.PlacePieceAt(x, y, currentPlayer);
        //     isPlaceing = false;
        //     pieceCountText.text = $"<color=black>Black:{othelloField.GetPieceCount(OthelloField.State.Black)}</color>\n<color=white>White:{othelloField.GetPieceCount(OthelloField.State.White)}</color>";
        //     othelloField.ClearPlaceableCells();
        //     CheckGameOver();
        // }
        // else
        // {
        //     Debug.Log("No placeable positions for enemy.");
        // }

        if (placeablePositions.Count > 0)
        {
            // ★ 変更点: ランダム選択からFindBestMoveによる探索に置き換え
            var (x, y) = othelloField.FindBestMove(currentPlayer, enemyDifficulty);
            
            // ランダムに位置を選択 (元のコード - 削除またはコメントアウト)
            // var random = new System.Random();
            // var (x, y) = placeablePositions[random.Next(placeablePositions.Count)];

            if (x != -1 && y != -1)
            {
                SoundManager.Play(enemyPlaceSound);
                othelloField.PlacePieceAt(x, y, currentPlayer);
                isPlaceing = false;
                pieceCountText.text = $"<color=black>Black:{othelloField.GetPieceCount(OthelloField.State.Black)}</color>\n<color=white>White:{othelloField.GetPieceCount(OthelloField.State.White)}</color>";
                othelloField.ClearPlaceableCells();
                CheckGameOver();
            }
            else
            {
                Debug.LogError("FindBestMove returned an invalid position despite placeable positions existing.");
                isPlaceing = false; // 念のため解除
                CheckGameOver(); // エラーだが、一応ターンを進める
            }
        }
        else
        {
            Debug.Log("No placeable positions for enemy. Passing turn.");
            isPlaceing = false;
            othelloField.ClearPlaceableCells(); // 置ける場所の可視化をクリア
            CheckGameOver(); // スイッチターン処理へ
        }
    }

}

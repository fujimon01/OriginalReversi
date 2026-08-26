using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OthelloField : MonoBehaviour
{
    [SerializeField]public OthelloPiece OthelloPiecePrefab;
    [SerializeField] public PlaceableCell placeableCellPrefab;
    public enum State
    {
        Draw,
        None,
        Black,
        White
    }

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    //private List<OthelloPiece> othelloPieces = new List<OthelloPiece>();
    private const int row = 8;
    private const int col = 8;

    private readonly int[,] positionWeights = new int[row,col]
    {
        { 100, -20, 10, 5, 5, 10, -20, 100 },
        { -20, -30, -1, -1, -1, -1, -30, -20 },
        { 10, -1, 1, 1, 1, 1, -1, 10 },
        { 5, -1, 1, 0, 0, 1, -1, 5 },
        { 5, -1, 1, 0, 0, 1, -1, 5 },
        { 10, -1, 1, 1, 1, 1, -1, 10 },
        { -20, -30, -1, -1, -1, -1, -30, -20 },
        { 100, -20, 10, 5, 5, 10, -20, 100 }        
    };

    List<OthelloPiece> othelloBoard = new List<OthelloPiece>();
    List<PlaceableCell> placeableCells = new List<PlaceableCell>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetPieceCount(State state)
    {
        int count = 0;
        foreach(OthelloPiece piece in othelloBoard)
        {
            if(piece.GetState() == state)
            {
                count++;
            }
        }
        return count;
    }

    public bool IsGameOver()
    {
        if((GetPieceCount(State.Black) + GetPieceCount(State.White) == 64)||(!HasPlaceableCells(State.Black)&&!HasPlaceableCells(State.White)))
        {
            return true;
        }
        return false;
    }

    public State DetermineWinner()
    {
        int blackCount = GetPieceCount(State.Black);
        int whiteCount = GetPieceCount(State.White);
        if(blackCount > whiteCount)
        {
            return State.Black;
        }
        else if(whiteCount > blackCount)
        {
            return State.White;
        }
        else
        {
            return State.Draw; 
        }
    }

    public bool HasPlaceableCells(State state)
    {
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < col; j++)
            {
                if(CanPlacePiece(i, j, state))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void VisualizePlaceableCells(State state)
    {
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < col; j++)
            {
                if(CanPlacePiece(i, j, state))
                {
                    PlaceableCell placeableCell = Instantiate(placeableCellPrefab, new Vector3(-10.31f+2.96f*i, 0.0f, -10.41f+2.96f*j), Quaternion.identity);
                    placeableCell.SetPosition(i, j);
                    placeableCells.Add(placeableCell);
                    placeableCell.StartBlink();
                }
            }
        }
    }
    public void InitializeField()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                OthelloPiece othelloPiece = Instantiate(OthelloPiecePrefab, new Vector3(-10.31f+2.96f*i, 0.0f, -10.41f+2.96f*j), Quaternion.identity);
                othelloPiece.SetPosition(i, j);
                othelloPiece.SetStateImmediately(State.None);
                othelloBoard.Add(othelloPiece); 
            }
        }
        // Set initial four pieces
        GetOthelloPiece(3, 3).SetStateImmediately(State.White);
        GetOthelloPiece(4, 4).SetStateImmediately(State.White);        
        GetOthelloPiece(3, 4).SetStateImmediately(State.Black);
        GetOthelloPiece(4, 3).SetStateImmediately(State.Black);
    }

    public OthelloPiece GetOthelloPiece(int x, int y)
    {
        foreach (OthelloPiece othelloPiece in othelloBoard)
        {
            if(othelloPiece.x == x && othelloPiece.y == y)
            {
                return othelloPiece;
            }
        }
        return null;
    }

    public bool CanPlacePiece(int x, int y, State state)
    {
        bool canPlace = false;
        OthelloPiece othelloPiece = GetOthelloPiece(x, y);
        if(othelloPiece.GetState() == State.None)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if(CanPlaceInDirection(othelloPiece, dx, dy, state))
                    {
                        canPlace = true;
                        break;
                    }
                }
            }
        }
        else
        {
            canPlace = false;
        }
        return canPlace;
    }

    private bool CanPlaceInDirection(OthelloPiece othelloPiece, int dirX, int dirY, State state)
    {
        int x = othelloPiece.x;
        int y = othelloPiece.y;
        bool hasOppositePiece = false;

        while(true)
        {
            x += dirX;
            y += dirY;
            OthelloPiece nextPiece = GetOthelloPiece(x, y);
            if(nextPiece == null || nextPiece.GetState() == State.None)
            {
                return false;
            }
            else if(nextPiece.GetState() != state)
            {
                hasOppositePiece = true;
            }
            else
            {
                return hasOppositePiece;
            }
        }
    }
    public void PlacePieceAt(int x, int y, State state)
    {
        OthelloPiece othelloPiece = GetOthelloPiece(x, y);
        if(othelloPiece != null && CanPlacePiece(x, y, state))
        {
            foreach(PlaceableCell cell in placeableCells)
            {
                cell.StopBlink();
            }
            othelloPiece.SetStateImmediately(state);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if(CanPlaceInDirection(othelloPiece, dx, dy, state))
                    {
                        FlipInDirection(othelloPiece, dx, dy, state);
                    }
                }
            }
            // After placing a piece, remove all placeable cells
        }
    }
    public void ClearPlaceableCells()
    {
        foreach(PlaceableCell cell in placeableCells)
        {
            cell.StopBlink();
            Destroy(cell.gameObject);
        }
        placeableCells.Clear();
    }
    private void FlipInDirection(OthelloPiece othelloPiece, int dirX, int dirY, State state)
    {
        int x = othelloPiece.x;
        int y = othelloPiece.y;

        while(true)
        {
            x += dirX;
            y += dirY;
            OthelloPiece nextPiece = GetOthelloPiece(x, y);
            if(nextPiece.GetState() != state)
            {
                nextPiece.FlipAndSetState(state);//追加
            }
            else
            {
                break;
            }
        }
    }


    public List<(int, int)> GetPlaceablePositions(State state)
    {
        List<(int, int)> placeablePositions = new List<(int, int)>();

        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < col; y++)
            {
                if (CanPlacePiece(x, y, state))
                {
                    placeablePositions.Add((x, y));
                }
            }
        }

        return placeablePositions;
    }

    private int EvaluateBoard(State state)
    {
        State opponent = (state == State.Black) ? State.White : State.Black;
        int score = 0;

        for(int x = 0; x < row; x++)
        {
            for(int y = 0; y < col; y++)
            {
                OthelloPiece piece = GetOthelloPiece(x, y);
                if(piece.GetState() == state)
                {
                    score += positionWeights[x, y];
                    score += 1;
                }
                else if(piece.GetState() == opponent)
                {
                    score -= positionWeights[x, y];
                    score -= 1;
                }
            }
        }

        int myMobility = GetPlaceablePositions(state).Count;
        int opponentMobility = GetPlaceablePositions(opponent).Count;

        score += (myMobility - opponentMobility) * 10;
        return score;
    }

    private int AlphaBeta(int depth, int maxDepth, int alpha, int beta, bool isMaximizingPlayer, State playerState)
    {
        // 1. 探索終了条件: 最大深さに達したか、ゲーム終了
        if (depth == maxDepth || IsGameOver())
        {
            // 評価関数を呼び出す
            return EvaluateBoard(isMaximizingPlayer ? playerState : (playerState == State.Black ? State.White : State.Black));
        }

        List<(int, int)> placeablePositions = GetPlaceablePositions(playerState);
        State opponentState = (playerState == State.Black) ? State.White : State.Black;

        // 2. パス判定
        if (placeablePositions.Count == 0)
        {
            // 相手も打てない場合はゲーム終了として評価値を返す
            if (GetPlaceablePositions(opponentState).Count == 0)
            {
                return EvaluateBoard(isMaximizingPlayer ? playerState : opponentState);
            }

            // 相手に手番を渡して探索を続ける (深さは進める)
            return AlphaBeta(depth + 1, maxDepth, alpha, beta, isMaximizingPlayer, opponentState);
        }

        // 3. 最大化プレイヤー (CPU) の処理
        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (var pos in placeablePositions)
            {
                // 仮想的に手を打つ
                List<(int, int, State)> originalStates = SimulatePlacePieceAt(pos.Item1, pos.Item2, playerState);

                // 再帰的に次の局面を探索
                int eval = AlphaBeta(depth + 1, maxDepth, alpha, beta, false, opponentState);

                // 仮想的に打った手を元に戻す（バックトラック）
                RevertBoard(originalStates);

                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, maxEval);
                if (beta <= alpha)
                {
                    break; // αβ枝刈り
                }
            }
            return maxEval;
        }
        // 4. 最小化プレイヤー (相手) の処理
        else
        {
            int minEval = int.MaxValue;
            foreach (var pos in placeablePositions)
            {
                // 仮想的に手を打つ
                List<(int, int, State)> originalStates = SimulatePlacePieceAt(pos.Item1, pos.Item2, playerState);

                // 再帰的に次の局面を探索
                int eval = AlphaBeta(depth + 1, maxDepth, alpha, beta, true, opponentState);

                // 仮想的に打った手を元に戻す（バックトラック）
                RevertBoard(originalStates);

                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, minEval);
                if (beta <= alpha)
                {
                    break; // αβ枝刈り
                }
            }
            return minEval;
        }
    }

    /// <summary>
    /// CPUの最善手を決定する
    /// </summary>
    /// <param name="state">CPUのプレイヤー状態</param>
    /// <param name="difficulty">難易度</param>
    /// <returns>最善手の座標 (x, y)</returns>
    public (int, int) FindBestMove(State state, Difficulty difficulty)
    {
        List<(int, int)> placeablePositions = GetPlaceablePositions(state);
        if (placeablePositions.Count == 0)
        {
            return (-1, -1); // 打てる手がない
        }

        // 難易度に応じて探索の深さを設定
        int maxDepth;
        switch (difficulty)
        {
            case Difficulty.Easy:
                // EASY: 1手先まで見て、評価値が高い手をランダムに選ぶ（探索は浅く）
                maxDepth = 1;
                break;
            case Difficulty.Normal:
                // NORMAL: 3手先まで探索
                maxDepth = 3; 
                break;
            case Difficulty.Hard:
                // HARD: 5手先まで探索（PCの性能に応じて調整してください）
                maxDepth = 5; 
                break;
            default:
                maxDepth = 1;
                break;
        }
        
        // EASYモードの場合、評価値が同じ手をランダムに選ぶためにリストを使う
        if (difficulty == Difficulty.Easy)
        {
            maxDepth = 1; 
            return GetRandomGoodMove(state, maxDepth);
        }

        int bestScore = int.MinValue;
        (int, int) bestMove = placeablePositions[0]; // 初期値はとりあえず最初の手

        State opponentState = (state == State.Black) ? State.White : State.Black;

        foreach (var pos in placeablePositions)
        {
            // 仮想的に手を打つ
            List<(int, int, State)> originalStates = SimulatePlacePieceAt(pos.Item1, pos.Item2, state);

            // 探索を開始
            // CPUは最大化プレイヤーなので、次の相手の手は最小化(false)から始める
            int score = AlphaBeta(1, maxDepth, int.MinValue, int.MaxValue, false, opponentState);

            // 仮想的に打った手を元に戻す（バックトラック）
            RevertBoard(originalStates);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = pos;
            }
        }

        return bestMove;
    }

    // EASYモード用の処理
    private (int, int) GetRandomGoodMove(State state, int maxDepth)
    {
        List<(int, int)> placeablePositions = GetPlaceablePositions(state);
        if (placeablePositions.Count == 0) return (-1, -1);

        int bestScore = int.MinValue;
        List<(int, int)> bestMoves = new List<(int, int)>();
        State opponentState = (state == State.Black) ? State.White : State.Black;

        foreach (var pos in placeablePositions)
        {
            List<(int, int, State)> originalStates = SimulatePlacePieceAt(pos.Item1, pos.Item2, state);
            int score = AlphaBeta(1, maxDepth, int.MinValue, int.MaxValue, false, opponentState);
            RevertBoard(originalStates);

            if (score > bestScore)
            {
                bestScore = score;
                bestMoves.Clear();
                bestMoves.Add(pos);
            }
            else if (score == bestScore)
            {
                bestMoves.Add(pos);
            }
        }
        
        // ベストスコアの手の中からランダムに一つ選ぶ
        var random = new System.Random();
        return bestMoves[random.Next(bestMoves.Count)];
    }


    /// <summary>
    /// 実際にコマを置く代わりに、仮想的に状態を変更する。
    /// 変更されたコマの元の状態リストを返す。
    /// </summary>
    private List<(int, int, State)> SimulatePlacePieceAt(int x, int y, State state)
    {
        List<(int, int, State)> originalStates = new List<(int, int, State)>();
        OthelloPiece placedPiece = GetOthelloPiece(x, y);

        // 新しく置くコマの状態を保存
        originalStates.Add((x, y, placedPiece.GetState()));
        placedPiece.SetStateImmediately(state);

        // 挟まれたコマの状態を保存し、フリップする
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (CanPlaceInDirectionSimulated(x, y, dx, dy, state))
                {
                    SimulateFlipInDirection(x, y, dx, dy, state, originalStates);
                }
            }
        }
        return originalStates;
    }

    /// <summary>
    /// SimulatePlacePieceAtで変更された盤面を元に戻す（バックトラック）
    /// </summary>
    private void RevertBoard(List<(int, int, State)> originalStates)
    {
        foreach (var item in originalStates)
        {
            GetOthelloPiece(item.Item1, item.Item2).SetStateImmediately(item.Item3);
        }
    }

    /// <summary>
    /// 仮想的なフリップを実行し、フリップされたコマの元の状態をリストに追加する
    /// </summary>
    private void SimulateFlipInDirection(int startX, int startY, int dirX, int dirY, State state, List<(int, int, State)> originalStates)
    {
        int x = startX;
        int y = startY;

        while (true)
        {
            x += dirX;
            y += dirY;
            OthelloPiece nextPiece = GetOthelloPiece(x, y);

            if (nextPiece == null || nextPiece.GetState() == state || nextPiece.GetState() == State.None)
            {
                break; // 終了条件
            }
            else
            {
                // フリップ対象のコマの元の状態を保存し、新しい状態を設定
                originalStates.Add((x, y, nextPiece.GetState()));
                nextPiece.SetStateImmediately(state);
            }
        }
    }
    
    // CanPlaceInDirectionのシミュレーション（仮想的な配置）バージョン
    private bool CanPlaceInDirectionSimulated(int startX, int startY, int dirX, int dirY, State state)
    {
        int x = startX;
        int y = startY;
        bool hasOppositePiece = false;

        while (true)
        {
            x += dirX;
            y += dirY;
            OthelloPiece nextPiece = GetOthelloPiece(x, y);

            if (nextPiece == null || nextPiece.GetState() == State.None)
            {
                return false;
            }
            else if (nextPiece.GetState() != state)
            {
                hasOppositePiece = true;
            }
            else // nextPiece.GetState() == state
            {
                return hasOppositePiece;
            }
        }
    }

}   
    

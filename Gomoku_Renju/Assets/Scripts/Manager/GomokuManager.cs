using UnityEngine;

public class GomokuManager : MonoBehaviour, IGomokuQuery, IGomokuCommand
{
    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };

    [SerializeField] private bool blackFirst = true;

    private IGridQuery gridQuery;
    private IGridCommand gridCommand;
    private IPlaceCommand placeCommand;
    private RenjuRuleChecker renjuRuleChecker;

    public StoneType CurrentStoneType { get; private set; }
    public bool IsGamePlaying { get; private set; }

    /// <summary>
    /// 무승부 체크용 벡터
    /// </summary>
    private Vector2Int drawCheckBoard = new Vector2Int(0, 0);

    public void Inject(IGridQuery gridQuery, IGridCommand gridCommand, IPlaceCommand placeCommand)
    {
        this.gridQuery = gridQuery;
        this.gridCommand = gridCommand;
        this.placeCommand = placeCommand;

        renjuRuleChecker = new RenjuRuleChecker(gridQuery, gridCommand);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        gridCommand.ResetGrid();

        CurrentStoneType = blackFirst ? StoneType.Black : StoneType.White;
        IsGamePlaying = true;

        Debug.Log("게임 시작");

        placeCommand.StartPlace();
    }

    public bool CanPlace(Vector2Int pos)
    {
        if (!IsGamePlaying) return false;
        if (!gridQuery.CanPlace(pos)) return false;
        if (CheckWin(pos, CurrentStoneType)) return true;
        if (!renjuRuleChecker.CanPlace(pos, CurrentStoneType)) return false;

        return true;
    }

    public bool TryPlace(Vector2Int pos)
    {
        if (!CanPlace(pos)) return false;

        gridCommand.SetGridStone(pos, CurrentStoneType);
        placeCommand.PlaceStone(pos, CurrentStoneType);

        if (CheckWin(pos, CurrentStoneType))
        {
            EndGame(CurrentStoneType);
            return true;
        }

        if (CheckDraw())
        {
            EndDraw();
            return true;
        }

        ChangeTurn();
        placeCommand.StartPlace();

        return true;
    }

    private void ChangeTurn()
    {
        CurrentStoneType = CurrentStoneType == StoneType.Black ? StoneType.White : StoneType.Black;
    }

    private bool CheckWin(Vector2Int pos, StoneType stoneType)
    {
        foreach (Vector2Int dir in directions)
        {
            int count = 1 + CheckLine(dir, pos, stoneType) + CheckLine(dir * -1, pos, stoneType);

            if (stoneType == StoneType.Black && count == 5) return true;
            if (stoneType == StoneType.White && count >= 5) return true;
        }

        return false;
    }

    /// <summary>
    /// 한 방향으로 이어진 돌 개수 검사
    /// </summary>
    private int CheckLine(Vector2Int dir, Vector2Int pos, StoneType stoneType)
    {
        int count = 0;
        Vector2Int checkPos = pos;

        while (true)
        {
            checkPos += dir;

            if (!gridQuery.IsInside(checkPos)) break;
            if (gridQuery.GetGridStone(checkPos) != stoneType) break;

            count++;
        }

        return count;
    }

    /// <summary>
    /// 모든 칸이 찼는지 검사
    /// </summary>
    private bool CheckDraw()
    {
        for (int x = 0; x < gridQuery.GridSize; x++)
        {
            drawCheckBoard.x = x;

            for (int y = 0; y < gridQuery.GridSize; y++)
            {
                drawCheckBoard.y = y;

                if (gridQuery.GetGridStone(drawCheckBoard) == StoneType.None) return false;
            }
        }

        return true;
    }

    private void EndGame(StoneType winner)
    {
        Debug.Log($"{winner} 승리");

        IsGamePlaying = false;
        placeCommand.CancelPlace();
    }

    private void EndDraw()
    {
        Debug.Log("무승부");

        IsGamePlaying = false;
        placeCommand.CancelPlace();
    }
}
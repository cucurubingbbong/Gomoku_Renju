using UnityEngine;

public class GomokuManager : MonoBehaviour
{
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };

    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlaceManager placeManager;
    [SerializeField] private bool blackFirst = true;

    public StoneType CurrentStoneType { get; private set; }
    public bool IsGamePlaying { get; private set; }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        gridManager.ResetGrid();
        CurrentStoneType = blackFirst ? StoneType.Black : StoneType.White;
        IsGamePlaying = true;
        Debug.Log("게임 시작");
        placeManager.StartPlace();
    }

    public bool CanPlace(int x, int y)
    {
        if (!IsGamePlaying) return false;
        if (!gridManager.CanPlace(x, y)) return false;
        if (!CheckRenjuRule(new Vector2Int(x, y), CurrentStoneType)) return false;

        return true;
    }

    public bool TryPlace(Vector2Int pos)
    {
        if (!CanPlace(pos.x, pos.y)) return false;

        gridManager.SetGridStone(pos.x, pos.y, (int)CurrentStoneType);
        placeManager.PlaceStone(pos, CurrentStoneType);

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
        placeManager.StartPlace();
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

            if (!gridManager.IsInside(checkPos.x, checkPos.y)) break;
            if (gridManager.GetGridStone(checkPos.x, checkPos.y) != stoneType) break;

            count++;
        }

        return count;
    }

    /// <summary>
    /// 정방향과 역방향을 합친 전체 돌 개수
    /// </summary>
    private int CheckLineCount(Vector2Int pos, Vector2Int dir, StoneType stoneType)
    {
        return 1 + CheckLine(dir, pos, stoneType) + CheckLine(dir * -1, pos, stoneType);
    }

    private bool CheckRenjuRule(Vector2Int pos, StoneType stoneType)
    {
        if (stoneType == StoneType.White) return true;

        gridManager.SetGridStone(pos.x, pos.y, (int)stoneType);

        bool isLongPlace = CheckLongPlace(pos, stoneType);
        bool isDoubleFour = CheckDoubleFour(pos, stoneType);
        bool isDoubleThree = CheckDoubleThree(pos, stoneType);

        gridManager.SetGridStone(pos.x, pos.y, (int)StoneType.None);

        if(isLongPlace || isDoubleFour || isDoubleThree) return false;

        return true;
    }

    /// <summary>
    /// 한 방향으로 6개 이상 이어졌는지 검사
    /// </summary>
    private bool CheckLongPlace(Vector2Int pos, StoneType stoneType)
    {
        foreach (Vector2Int dir in directions)
        {
            if (CheckLineCount(pos, dir, stoneType) >= 6) return true;
        }

        return false;
    }

    /// <summary>
    /// 4가 두 방향 이상 존재하는지 검사
    /// </summary>
    private bool CheckDoubleFour(Vector2Int pos, StoneType stoneType)
    {
        int fourCount = 0;

        foreach (Vector2Int dir in directions)
        {
            if (CheckFour(pos, dir, stoneType)) fourCount++;
            if (fourCount >= 2) return true;
        }

        return false;
    }

    /// <summary>
    /// 해당 방향에서 한 칸만 더 두면 오목이 되는지 검사
    /// </summary>
    private bool CheckFour(Vector2Int pos, Vector2Int dir, StoneType stoneType)
    {
        for (int i = -4; i <= 4; i++)
        {
            if (i == 0) continue;

            Vector2Int checkPos = pos + dir * i;

            if (!gridManager.IsInside(checkPos.x, checkPos.y)) continue;
            if (gridManager.GetGridStone(checkPos.x, checkPos.y) != StoneType.None) continue;

            gridManager.SetGridStone(checkPos.x, checkPos.y, (int)stoneType);

            int count = CheckLineCount(checkPos, dir, stoneType);

            gridManager.SetGridStone(checkPos.x, checkPos.y, (int)StoneType.None);

            if (count == 5) return true;
        }

        return false;
    }

    /// <summary>
    /// 열린 3이 두 방향 이상 존재하는지 검사
    /// </summary>
    private bool CheckDoubleThree(Vector2Int pos, StoneType stoneType)
    {
        int threeCount = 0;

        foreach (Vector2Int dir in directions)
        {
            if (CheckOpenThree(pos, dir, stoneType)) threeCount++;
            if (threeCount >= 2) return true;
        }

        return false;
    }

    /// <summary>
    /// 해당 방향에서 한 칸 더 두었을 때 열린 4가 되는지 검사
    /// </summary>
    private bool CheckOpenThree(Vector2Int pos, Vector2Int dir, StoneType stoneType)
    {
        for (int i = -3; i <= 3; i++)
        {
            if (i == 0) continue;

            Vector2Int checkPos = pos + dir * i;

            if (!gridManager.IsInside(checkPos.x, checkPos.y)) continue;
            if (gridManager.GetGridStone(checkPos.x, checkPos.y) != StoneType.None) continue;

            gridManager.SetGridStone(checkPos.x, checkPos.y, (int)stoneType);

            bool isOpenFour = CheckOpenFour(checkPos, dir, stoneType);

            gridManager.SetGridStone(checkPos.x, checkPos.y, (int)StoneType.None);

            if (isOpenFour) return true;
        }

        return false;
    }

    /// <summary>
    /// 4개의 돌 양쪽이 비어있는지 검사
    /// </summary>
    private bool CheckOpenFour(Vector2Int pos, Vector2Int dir, StoneType stoneType)
    {
        int forwardCount = CheckLine(dir, pos, stoneType);
        int backwardCount = CheckLine(dir * -1, pos, stoneType);
        int count = 1 + forwardCount + backwardCount;

        if (count != 4) return false;

        Vector2Int forwardEnd = pos + dir * (forwardCount + 1);
        Vector2Int backwardEnd = pos - dir * (backwardCount + 1);

        bool forwardEmpty = gridManager.IsInside(forwardEnd.x, forwardEnd.y) && gridManager.GetGridStone(forwardEnd.x, forwardEnd.y) == StoneType.None;
        bool backwardEmpty = gridManager.IsInside(backwardEnd.x, backwardEnd.y) && gridManager.GetGridStone(backwardEnd.x, backwardEnd.y) == StoneType.None;

        return forwardEmpty && backwardEmpty;
    }

    /// <summary>
    /// 모든 칸이 찼는지 검사
    /// </summary>
    private bool CheckDraw()
    {
        for (int x = 0; x < gridManager.GridSize; x++)
        {
            for (int y = 0; y < gridManager.GridSize; y++)
            {
                if (gridManager.GetGridStone(x, y) == StoneType.None) return false;
            }
        }

        return true;
    }

    private void EndGame(StoneType winner)
    {
        Debug.Log($"{winner} 승리");
        IsGamePlaying = false;
        placeManager.CancelPlace();
    }

    private void EndDraw()
    {
        Debug.Log("무승부");
        IsGamePlaying = false;
        placeManager.CancelPlace();
    }
}
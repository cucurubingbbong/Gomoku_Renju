using UnityEngine;

public class RenjuRuleChecker
{
    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };

    private readonly IGridQuery gridQuery;
    private readonly IGridCommand gridCommand;

    /// <summary>
    /// DIP 적용
    /// </summary>
    /// <param name="gridQuery"></param>
    /// <param name="gridCommand"></param>
    public RenjuRuleChecker(IGridQuery gridQuery, IGridCommand gridCommand)
    {
        this.gridQuery = gridQuery;
        this.gridCommand = gridCommand;
    }

    public bool CanPlace(Vector2Int pos, StoneType stoneType)
    {
        if (stoneType == StoneType.White) return true;

        gridCommand.SetGridStone(pos, stoneType);

        bool isLongPlace = CheckLongPlace(pos, stoneType);
        bool isDoubleFour = CheckDoubleFour(pos, stoneType);
        bool isDoubleThree = CheckDoubleThree(pos, stoneType);

        gridCommand.SetGridStone(pos, StoneType.None);

        if (isLongPlace || isDoubleFour || isDoubleThree) return false;

        return true;
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
    /// 정방향과 역방향을 합친 전체 돌 개수
    /// </summary>
    private int CheckLineCount(Vector2Int pos, Vector2Int dir, StoneType stoneType)
    {
        return 1 + CheckLine(dir, pos, stoneType) + CheckLine(dir * -1, pos, stoneType);
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

            if (!gridQuery.IsInside(checkPos)) continue;
            if (gridQuery.GetGridStone(checkPos) != StoneType.None) continue;

            gridCommand.SetGridStone(checkPos, stoneType);

            int count = CheckLineCount(checkPos, dir, stoneType);

            gridCommand.SetGridStone(checkPos, StoneType.None);

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

            if (!gridQuery.IsInside(checkPos)) continue;
            if (gridQuery.GetGridStone(checkPos) != StoneType.None) continue;

            gridCommand.SetGridStone(checkPos, stoneType);

            bool isOpenFour = CheckOpenFour(checkPos, dir, stoneType);

            gridCommand.SetGridStone(checkPos, StoneType.None);

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

        bool forwardEmpty = gridQuery.IsInside(forwardEnd) && gridQuery.GetGridStone(forwardEnd) == StoneType.None;
        bool backwardEmpty = gridQuery.IsInside(backwardEnd) && gridQuery.GetGridStone(backwardEnd) == StoneType.None;

        return forwardEmpty && backwardEmpty;
    }
}
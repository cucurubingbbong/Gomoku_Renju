using UnityEngine;

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// 2D 그리드 
    /// 0 : 비어있음 , 1 : 백돌 , 2 : 흑돌
    /// </summary>
    public int[,] grid = null;

    /// <summary>
    /// 그리드 사이즈
    /// </summary>
    [SerializeField]
    private int gridSize = 15;

    public int GridSize => gridSize;
    private void GenerateGrid()
    {
        grid = new int[gridSize, gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = (int)StoneType.None;
            }
        }
    }

    /// <summary>
    /// 그리드 초기화
    /// </summary>
    public void ResetGrid()
    {
        GenerateGrid();
    }

    public void SetGridStone(int x, int y, int stoneIndex)
    {
        if (!IsInside(x, y))
        {
            return;
        }

        grid[x, y] = stoneIndex;
    }

    /// <summary>
    /// 해당 위치의 돌 반환
    /// </summary>
    public int GetGridStone(int x, int y)
    {
        if (!IsInside(x, y))
        {
            return (int)StoneType.None;
        }

        return grid[x, y];
    }

    public bool CanPlace(int x, int y)
    {
        if (!IsInside(x, y))
        {
            return false;
        }

        return grid[x, y] == (int)StoneType.None;
    }

    /// <summary>
    /// 그리드 범위 확인
    /// </summary>
    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }

    public Vector2Int ConvertToGridPos(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.z);

        return new Vector2Int(x, y);
    }
}
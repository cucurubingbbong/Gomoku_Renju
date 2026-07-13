using UnityEngine;

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// 2D 그리드 
    /// 0 : 비어있음 , 1 : 백돌 , 2 : 흑돌
    /// </summary>
    public StoneType[,] grid = null;

    /// <summary>
    /// 그리드 원점 좌표
    /// 중앙좌표를 기준으로 그리드 좌표를 계산하기 위해 사용
    /// </summary>
    [SerializeField] private Vector2Int originPos = new Vector2Int(0, 0);

    /// <summary>
    /// 그리드 사이즈
    /// </summary>
    [SerializeField]
    private int gridSize = 15;

    public int GridSize => gridSize;
    private void GenerateGrid()
    {
        grid = new StoneType[gridSize, gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = StoneType.None;
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

        grid[x, y] = (StoneType)stoneIndex;
    }

    /// <summary>
    /// 해당 위치의 돌 반환
    /// </summary>
    public StoneType GetGridStone(int x, int y)
    {
        if (!IsInside(x, y)) return StoneType.None;

        return grid[x, y];
    }

    public bool CanPlace(int x, int y)
    {
        if (!IsInside(x, y)) return false;

        return grid[x, y] == StoneType.None;
    }

    /// <summary>
    /// 그리드 범위 확인
    /// </summary>
    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }

    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변환
    /// </summary>
    /// <param name="worldPos">월드 좌표</param>
    /// <returns>그리드 좌표</returns>
    public Vector2Int ConvertToGridPos(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int();
        gridPos.x = Mathf.RoundToInt(worldPos.x - originPos.x);
        gridPos.y = Mathf.RoundToInt(worldPos.z - originPos.y);

        return gridPos;
    }
}
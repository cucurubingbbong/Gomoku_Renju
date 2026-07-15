using UnityEngine;

public class GridManager : MonoBehaviour, IGridQuery, IGridCommand
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
    [SerializeField] private int gridSize = 15;

    public int GridSize => gridSize;

    private void GenerateGrid()
    {
        grid = new StoneType[gridSize, gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++) grid[i, j] = StoneType.None;
        }
    }

    /// <summary>
    /// 그리드 초기화
    /// </summary>
    public void ResetGrid()
    {
        GenerateGrid();
    }

    public void SetGridStone(Vector2Int pos, StoneType stoneType)
    {
        if (!IsInside(pos)) return;

        grid[pos.x, pos.y] = stoneType;
    }

    /// <summary>
    /// 해당 위치의 돌 반환
    /// </summary>
    public StoneType GetGridStone(Vector2Int pos)
    {
        if (!IsInside(pos)) return StoneType.None;

        return grid[pos.x, pos.y];
    }

    public bool CanPlace(Vector2Int pos)
    {
        if (!IsInside(pos)) return false;

        return grid[pos.x, pos.y] == StoneType.None;
    }

    /// <summary>
    /// 그리드 범위 확인
    /// </summary>
    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize && pos.y >= 0 && pos.y < gridSize;
    }

    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변환
    /// </summary>
    public Vector2Int ConvertToGridPos(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int();

        gridPos.x = Mathf.RoundToInt(worldPos.x - originPos.x);
        gridPos.y = Mathf.RoundToInt(worldPos.z - originPos.y);

        return gridPos;
    }
}
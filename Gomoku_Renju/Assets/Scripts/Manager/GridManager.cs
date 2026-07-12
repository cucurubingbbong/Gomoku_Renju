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

    private void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = 0;
            }
        }
    }

    public void SetGridStone(int x, int y, int stoneIndex)
    {
        grid[x, y] = stoneIndex;
    }

    public bool CanPlace(int x, int y)
    {
        return (grid[x, y] != 0);
    }

    public Vector2Int ConvertToGridPos(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.z);

        return new Vector2Int(x, y);
    }
}

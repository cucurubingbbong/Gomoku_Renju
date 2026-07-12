using Unity.Mathematics;
using UnityEngine;
public class GomokuManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlaceManager placeManager;

    public StoneType CurrentStoneType { get; private set; }
    public bool IsGamePlaying { get; private set; }

    [SerializeField] private bool blackFirst = true;

    public void StartGame()
    {
        gridManager.ResetGrid();
        CurrentStoneType = (blackFirst) ? StoneType.Black : StoneType.White;
        IsGamePlaying = true;
        placeManager.StartPlace();
    }
    public bool CanPlace(int x, int y)
    {
        if (!IsGamePlaying) return false;
        // 렌주룰 적용하기

        return gridManager.CanPlace(x, y);
    }

    public bool TryPlace(Vector2Int pos)
    {
        if (!CanPlace(pos.x, pos.y)) return false;
        gridManager.SetGridStone(pos.x, pos.y, (int)CurrentStoneType);
        placeManager.PlaceStone(pos, CurrentStoneType);

        if (CheckWin(pos, CurrentStoneType))
        {
            EndGame();
            return true;
        }

        ChangeTurn();
        placeManager.StartPlace();

        return true;
    }

    private void ChangeTurn()
    {
        CurrentStoneType = (CurrentStoneType == StoneType.Black) ? StoneType.White : StoneType.Black;
    }

    private bool CheckWin(Vector2Int pos, StoneType currentStoneTYpe)
    {
        // 가로세로 , 대각선으로 검사하기
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),  // 가로
            new Vector2Int(0, 1),  // 세로
            new Vector2Int(1, -1), // 우하향 대각선
            new Vector2Int(1, 1)   // 우상향 대각선
        };
        foreach(Vector2Int dir in directions)
        {
            int count = 1;
            // 순방향으로 검사하기 = 카운트 리턴
            // 역방향으로 검사하기 = 카운트 리턴
            // 카운트 5개이상이면 승리판정
        }
        return false;
    }

    private void EndGame()
    {
        Debug.Log("게임종료");
    }
}

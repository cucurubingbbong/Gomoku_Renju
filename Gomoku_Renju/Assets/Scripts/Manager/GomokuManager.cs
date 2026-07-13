using Unity.Mathematics;
using UnityEngine;
public class GomokuManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlaceManager placeManager;

    public StoneType CurrentStoneType { get; private set; }
    public bool IsGamePlaying { get; private set; }

    [SerializeField] private bool blackFirst = true;

    public void Start()
    {
        StartGame();
    }
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

    private bool CheckWin(Vector2Int pos, StoneType currentStoneType)
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
            // 정방향 검사
            CheckLine(dir , pos , currentStoneType);
            // 역방향 검사
            CheckLine(dir*-1 , pos , currentStoneType);
            if(count >= 5) return true;
        }
        return false;
    }

    /// <summary>
    /// 오목 승리판정을 위한 라인 체크
    /// </summary>
    /// <param name="Dir">방향</param>
    /// <param name="pos">검사를 시작할 위치</param>
    /// <param name="currentCheckStoneType">현재 체크할 돌 타입</param>
    /// <returns></returns>
    private int CheckLine(Vector2Int Dir , Vector2Int pos , StoneType currentCheckStoneType)
    {
        int count = 0;
        for(int i = 0; i < 4; i++)
        {
            Vector2Int checkPos = pos;
            checkPos += Dir;
            if(gridManager.GetGridStone(checkPos.x , checkPos.y) == CurrentStoneType);
            else break;
            count++;
        }
        return count;
    }

    private void EndGame()
    {
        Debug.Log("게임종료");
    }
}

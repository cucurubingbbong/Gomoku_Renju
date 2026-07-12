using Unity.Mathematics;
using UnityEngine;
public class GomokuManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlaceManager placeManager;

    public StoneType CurrentStoneType { get; private set; }
    public bool IsGamePlaying { get; private set; }

    public void StartGame()
    {

    }
    public bool CanPlace(int x, int y)
    {
        if (!IsGamePlaying) return false;

        return gridManager.CanPlace(x, y);
    }
}

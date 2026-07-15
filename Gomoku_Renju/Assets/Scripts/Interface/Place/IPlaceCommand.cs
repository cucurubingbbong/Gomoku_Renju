using UnityEngine;

public interface IPlaceCommand
{
    void StartPlace();
    void CancelPlace();
    void PlaceStone(Vector2Int pos, StoneType stoneType);
}

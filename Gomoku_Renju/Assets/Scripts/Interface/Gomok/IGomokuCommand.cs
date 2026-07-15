using System;
using UnityEngine;

public interface IGomokuCommand
{
    void StartGame();
    bool TryPlace(Vector2Int pos);
}
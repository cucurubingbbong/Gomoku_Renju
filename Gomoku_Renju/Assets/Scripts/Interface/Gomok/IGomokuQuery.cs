using System;
using UnityEngine;

public interface IGomokuQuery
{
    StoneType CurrentStoneType { get; }
    bool IsGamePlaying { get; }

    bool CanPlace(Vector2Int pos);
}
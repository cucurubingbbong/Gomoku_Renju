using System;
using UnityEngine;

public interface IGridQuery
{
    int GridSize { get; }

    bool IsInside(Vector2Int pos);
    bool CanPlace(Vector2Int pos);
    StoneType GetGridStone(Vector2Int pos);
    Vector2Int ConvertToGridPos(Vector3 worldPos);
}
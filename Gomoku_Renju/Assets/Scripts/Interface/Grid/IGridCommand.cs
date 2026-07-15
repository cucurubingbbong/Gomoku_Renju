using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface IGridCommand
{
    void ResetGrid();
    void SetGridStone(Vector2Int pos, StoneType stoneType);
}
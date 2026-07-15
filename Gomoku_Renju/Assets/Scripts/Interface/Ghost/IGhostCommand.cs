using UnityEngine;

public interface IGhostCommand
{
    void GetGhost();
    void UpdateGhost(bool isBuild, Vector2Int pos);
    void ResetGhost();
}

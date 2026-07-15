using System;
using UnityEngine;

public class GhostManager : MonoBehaviour, IGhostCommand
{
    /// <summary>
    /// 0 : true , 1 : false
    /// </summary>
    [SerializeField] private Material[] ghostMatArr = new Material[2];

    [SerializeField] private Material selectedMaterial;

    [SerializeField] private GameObject currentGhostObj = null;

    [SerializeField] private GameObject ghostObjPrefab = null;

    [SerializeField] private MeshRenderer ghostMesh;

    public void GetGhost()
    {
        if (currentGhostObj != null) return;

        currentGhostObj = Instantiate(ghostObjPrefab);
        ghostMesh = currentGhostObj.GetComponent<MeshRenderer>();
    }

    public void UpdateGhost(bool isBuild, Vector2Int pos)
    {
        selectedMaterial = isBuild ? ghostMatArr[0] : ghostMatArr[1];

        currentGhostObj.transform.position = new Vector3(pos.x, 1, pos.y);
        ghostMesh.material = selectedMaterial;
    }

    public void ResetGhost()
    {
        currentGhostObj.SetActive(false);
    }
}
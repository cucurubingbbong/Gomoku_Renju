using Unity.Mathematics;
using UnityEngine;
public class GomokuManager : MonoBehaviour
{
    [Header("게임오브젝트")]
    // 배치하는 돌들의 루트 오브젝트
    [SerializeField] private GameObject StoneRoot = null;
    /// <summary>
    /// 0번 : 백 , 1번 : 흑
    /// </summary>
    [SerializeField] private GameObject[] stones = null;

    void Awake()
    {
        
    }


    public void PlaceStone(int x , int y , int stoneIndex)
    {
        
    }
}

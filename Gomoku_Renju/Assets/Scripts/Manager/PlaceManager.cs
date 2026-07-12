using UnityEngine;

public enum PlaceStatus
{
    // 내 차례 아님
    Idle,

    // 미리보기 중
    Preview,

    // 돌 두는 중
    Place,
}

public class PlaceManager : MonoBehaviour
{
    /// <summary>
    /// 현재 착수 상태
    /// </summary>
    [SerializeField] private PlaceStatus currentPlaceStatus = PlaceStatus.Idle;

    /// <summary>
    /// 선택 가능한 레이어
    /// </summary>
    [SerializeField] private LayerMask selectLayerMask;

    /// <summary>
    /// 그리드 매니저 / 추후 인터페이스로 쿼리 형식으로 분리 예정
    /// </summary>
    [SerializeField] private GridManager gridManager;

    private Camera mainCamera;

    [Header("게임오브젝트")]
    // 배치하는 돌들의 루트 오브젝트
    [SerializeField] private GameObject StoneRoot = null;
    /// <summary>
    /// 0번 : 백 , 1번 : 흑
    /// </summary>
    [SerializeField] private GameObject[] stones = null;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (currentPlaceStatus == PlaceStatus.Preview)
        {
            UpdatePlaceFlow();
        }
    }

    private void UpdatePlaceFlow()
    {
        if (!TrySelectPos(out Vector3 selectPos))
        {
            return;
        }

        Vector2Int gridPos = gridManager.ConvertToGridPos(selectPos);
        bool canPlace = gridManager.CanPlace(gridPos.x, gridPos.y);

        HandlePlaceInput(canPlace, gridPos);
    }

    private void HandlePlaceInput(bool canPlace, Vector2Int pos)
    {
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            Place(pos);
        }
    }

    private void Place(Vector2Int pos)
    {
        ChangeState(PlaceStatus.Place);

        gridManager.SetGridStone(pos.x, pos.y, 0);

        ChangeState(PlaceStatus.Idle);
    }

    /// <summary>
    /// 마우스가 바둑판을 가리키고 있는지 확인
    /// </summary>
    private bool TrySelectPos(out Vector3 selectPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, selectLayerMask))
        {
            selectPos = hit.point;
            return true;
        }

        selectPos = default;
        return false;
    }

    /// <summary>
    /// 착수 시작
    /// </summary>
    public void StartPlace()
    {
        ChangeState(PlaceStatus.Preview);
    }

    /// <summary>
    /// 착수 취소
    /// </summary>
    public void CancelPlace()
    {
        ChangeState(PlaceStatus.Idle);
    }

    /// <summary>
    /// 착수 상태 변경
    /// </summary>
    private void ChangeState(PlaceStatus newStatus)
    {
        if (currentPlaceStatus == newStatus)
        {
            return;
        }

        currentPlaceStatus = newStatus;
    }
}
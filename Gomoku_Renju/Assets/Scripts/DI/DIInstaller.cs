using UnityEngine;

/// <summary>
/// 의존성 주입을 위한 클래스입니다. 
/// </summary>
public class DIInstaller : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GomokuManager gomokuManager;
    [SerializeField] private PlaceManager placeManager;
    [SerializeField] private GhostManager ghostManager;

    private void Awake()
    {
        IGridQuery gridQuery = gridManager;
        IGridCommand gridCommand = gridManager;

        IGomokuQuery gomokuQuery = gomokuManager;
        IGomokuCommand gomokuCommand = gomokuManager;

        IPlaceCommand placeCommand = placeManager;
        IGhostCommand ghostCommand = ghostManager;

        gomokuManager.Inject(gridQuery, gridCommand, placeCommand);
        placeManager.Inject(gridQuery, gomokuQuery, gomokuCommand, ghostCommand);
    }
}

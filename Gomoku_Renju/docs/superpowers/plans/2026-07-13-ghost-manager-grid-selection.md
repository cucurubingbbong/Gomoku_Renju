# GhostManager and Grid Selection Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Display a transparent current-turn stone on valid intersections and keep selection, preview, and placement aligned when `GomokuBoard` moves.

**Architecture:** `GridManager` owns both directions of board coordinate conversion using the board center transform. `PlaceManager` converts raycast hits into validated grid positions, while `GhostManager` owns only the preview object's lifecycle and transparent rendering setup.

**Tech Stack:** Unity 6.3, C#, Universal Render Pipeline, Unity Physics raycasts

## Global Constraints

- Preserve the existing manager-oriented structure and Korean comment style.
- Preserve the existing `stones` array order: index `0` is white and index `1` is black.
- Support board position changes only; board rotation and cell-size changes are outside this change.
- Do not add a Unity test assembly; the user approved compile verification and Play Mode scenarios.
- Preserve unrelated uncommitted scene, material, and texture changes.

---

### Task 1: Centralize board coordinate conversion

**Files:**
- Modify: `Assets/Scripts/Manager/GridManager.cs:11-21,86-98`
- Modify: `Assets/00.GameCore/Scene/Game.unity:154-167`

**Interfaces:**
- Consumes: `Transform.position`, `gridSize`, and grid coordinates represented by `Vector2Int`.
- Produces: `Vector2Int ConvertToGridPos(Vector3 worldPos)` and `Vector3 ConvertToWorldPos(Vector2Int gridPos)`.

- [ ] **Step 1: Replace the numeric origin with the board transform and stone height**

Replace the current `originPos` field and its comment with:

```csharp
    /// <summary>
    /// 바둑판 중앙 위치
    /// </summary>
    [SerializeField] private Transform boardTransform = null;

    /// <summary>
    /// 바둑판을 기준으로 돌이 배치되는 높이
    /// </summary>
    [SerializeField] private float stoneHeight = 1f;
```

- [ ] **Step 2: Implement matching world-to-grid and grid-to-world conversions**

Replace `ConvertToGridPos` and add `ConvertToWorldPos` immediately after it:

```csharp
    public Vector2Int ConvertToGridPos(Vector3 worldPos)
    {
        float centerIndex = (gridSize - 1) * 0.5f;
        Vector3 boardOffset = worldPos - boardTransform.position;

        int x = Mathf.RoundToInt(boardOffset.x + centerIndex);
        int y = Mathf.RoundToInt(boardOffset.z + centerIndex);

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변환
    /// </summary>
    /// <param name="gridPos">그리드 좌표</param>
    /// <returns>돌이 배치될 월드 좌표</returns>
    public Vector3 ConvertToWorldPos(Vector2Int gridPos)
    {
        float centerIndex = (gridSize - 1) * 0.5f;

        float x = boardTransform.position.x + gridPos.x - centerIndex;
        float y = boardTransform.position.y + stoneHeight;
        float z = boardTransform.position.z + gridPos.y - centerIndex;

        return new Vector3(x, y, z);
    }
```

- [ ] **Step 3: Wire the existing board transform in the scene**

Add these serialized values to the existing `GridManager` component. The board transform is file ID `944570569`:

```yaml
  boardTransform: {fileID: 944570569}
  stoneHeight: 1
  gridSize: 15
```

- [ ] **Step 4: Inspect the focused diff**

Run `git diff -- Assets/Scripts/Manager/GridManager.cs Assets/00.GameCore/Scene/Game.unity`.

Expected: `GridManager` uses `GomokuBoard` as its center; unrelated scene hunks remain unchanged.

---

### Task 2: Add GhostManager preview lifecycle

**Files:**
- Create: `Assets/Scripts/Manager/GhostManager.cs`

**Interfaces:**
- Consumes: a stone prefab, a world position, and the current `StoneType`.
- Produces: `ShowGhost(GameObject stonePrefab, Vector3 worldPos, StoneType stoneType)` and `HideGhost()`.

- [ ] **Step 1: Create GhostManager with these fields and public methods**

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GhostManager : MonoBehaviour
{
    /// <summary>
    /// 고스트 돌의 투명도
    /// </summary>
    [SerializeField, Range(0f, 1f)] private float ghostAlpha = 0.5f;

    private GameObject currentGhost = null;
    private StoneType currentStoneType = StoneType.None;
    private readonly List<Material> ghostMaterials = new List<Material>();

    /// <summary>
    /// 고스트 돌 표시
    /// </summary>
    public void ShowGhost(GameObject stonePrefab, Vector3 worldPos, StoneType stoneType)
    {
        if (stonePrefab == null || stoneType == StoneType.None)
        {
            HideGhost();
            return;
        }

        if (currentGhost == null || currentStoneType != stoneType)
        {
            CreateGhost(stonePrefab, stoneType);
        }

        currentGhost.transform.position = worldPos;
        currentGhost.SetActive(true);
    }

    /// <summary>
    /// 고스트 돌 숨김
    /// </summary>
    public void HideGhost()
    {
        if (currentGhost != null) currentGhost.SetActive(false);
    }
```

- [ ] **Step 2: Add ghost creation and component disabling**

```csharp
    private void CreateGhost(GameObject stonePrefab, StoneType stoneType)
    {
        ClearGhost();

        currentGhost = Instantiate(stonePrefab, transform);
        currentGhost.name = stonePrefab.name + "_Ghost";
        currentStoneType = stoneType;

        DisableGhostComponents();
        ApplyGhostMaterials();
    }

    private void DisableGhostComponents()
    {
        Collider[] colliders = currentGhost.GetComponentsInChildren<Collider>();
        foreach (Collider targetCollider in colliders)
        {
            targetCollider.enabled = false;
        }

        Light[] lights = currentGhost.GetComponentsInChildren<Light>();
        foreach (Light targetLight in lights)
        {
            targetLight.enabled = false;
        }
    }
```

- [ ] **Step 3: Add runtime transparent material handling and cleanup**

```csharp
    private void ApplyGhostMaterials()
    {
        Renderer[] renderers = currentGhost.GetComponentsInChildren<Renderer>();
        foreach (Renderer targetRenderer in renderers)
        {
            Material[] materials = targetRenderer.materials;
            foreach (Material targetMaterial in materials)
            {
                SetMaterialTransparent(targetMaterial);
                ghostMaterials.Add(targetMaterial);
            }
        }
    }

    private void SetMaterialTransparent(Material targetMaterial)
    {
        targetMaterial.SetOverrideTag("RenderType", "Transparent");
        targetMaterial.SetFloat("_Surface", 1f);
        targetMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        targetMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        targetMaterial.SetFloat("_ZWrite", 0f);
        targetMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        targetMaterial.renderQueue = (int)RenderQueue.Transparent;

        if (targetMaterial.HasProperty("_BaseColor"))
        {
            Color color = targetMaterial.GetColor("_BaseColor");
            color.a = ghostAlpha;
            targetMaterial.SetColor("_BaseColor", color);
        }
    }

    private void ClearGhost()
    {
        if (currentGhost != null) Destroy(currentGhost);

        foreach (Material targetMaterial in ghostMaterials)
        {
            if (targetMaterial != null) Destroy(targetMaterial);
        }

        ghostMaterials.Clear();
        currentGhost = null;
        currentStoneType = StoneType.None;
    }

    private void OnDestroy()
    {
        ClearGhost();
    }
}
```

- [ ] **Step 4: Inspect the new dependency surface**

Run `rg -n "class GhostManager|ShowGhost|HideGhost|UnityEditor" Assets/Scripts/Manager/GhostManager.cs`.

Expected: the class and two public methods are present; `UnityEditor` is absent.

---

### Task 3: Snap selection, preview, and placement to one coordinate source

**Files:**
- Modify: `Assets/Scripts/Manager/PlaceManager.cs:15-137`

**Interfaces:**
- Consumes: `GridManager.ConvertToGridPos`, `GridManager.ConvertToWorldPos`, `GhostManager.ShowGhost`, and `GhostManager.HideGhost`.
- Produces: `TrySelectGridPos(out Vector2Int gridPos)` and explicit white/black prefab selection.

- [ ] **Step 1: Add and initialize GhostManager**

Add `[RequireComponent(typeof(GhostManager))]` above `PlaceManager`, add `[SerializeField] private GhostManager ghostManager = null;` beside the manager references, and update `Awake`:

```csharp
    private void Awake()
    {
        mainCamera = Camera.main;

        if (ghostManager == null) ghostManager = GetComponent<GhostManager>();
        if (ghostManager == null) ghostManager = gameObject.AddComponent<GhostManager>();
    }
```

Remove the now-unused `InvalidPos` field.

- [ ] **Step 2: Replace the preview update flow**

```csharp
    private void UpdatePlaceFlow()
    {
        if (!TrySelectGridPos(out Vector2Int gridPos))
        {
            ghostManager.HideGhost();
            return;
        }

        bool canPlace = gomokuManager.CanPlace(gridPos.x, gridPos.y);

        if (canPlace)
        {
            GameObject stonePrefab = GetStonePrefab(gomokuManager.CurrentStoneType);
            Vector3 ghostPos = gridManager.ConvertToWorldPos(gridPos);
            ghostManager.ShowGhost(stonePrefab, ghostPos, gomokuManager.CurrentStoneType);
        }
        else
        {
            ghostManager.HideGhost();
        }

        HandlePlaceInput(canPlace, gridPos);
    }
```

- [ ] **Step 3: Use shared conversion and the existing prefab order for placement**

Replace `PlaceStone` and add `GetStonePrefab` after it:

```csharp
    public void PlaceStone(Vector2Int pos, StoneType stoneType)
    {
        GameObject stonePrefab = GetStonePrefab(stoneType);
        if (stonePrefab == null) return;

        Vector3 stonePos = gridManager.ConvertToWorldPos(pos);
        GameObject stoneObject = Instantiate(stonePrefab, stonePos, Quaternion.identity);

        if (StoneRoot != null) stoneObject.transform.SetParent(StoneRoot.transform);
    }

    private GameObject GetStonePrefab(StoneType stoneType)
    {
        int stonePrefabIndex = (stoneType == StoneType.White) ? 0 : 1;

        if (stoneType == StoneType.None || stones == null || stonePrefabIndex >= stones.Length)
        {
            return null;
        }

        return stones[stonePrefabIndex];
    }
```

- [ ] **Step 4: Replace raw hit selection with validated grid selection**

```csharp
    /// <summary>
    /// 마우스가 가리키는 그리드 좌표 확인
    /// </summary>
    /// <param name="gridPos">선택한 그리드 좌표</param>
    /// <returns>유효한 그리드 좌표 선택 여부</returns>
    private bool TrySelectGridPos(out Vector2Int gridPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, selectLayerMask))
        {
            gridPos = gridManager.ConvertToGridPos(hit.point);
            return gridManager.IsInside(gridPos.x, gridPos.y);
        }

        gridPos = new Vector2Int(-1, -1);
        return false;
    }
```

- [ ] **Step 5: Hide the ghost outside Preview state**

Extend `ChangeState` after assigning `currentPlaceStatus`:

```csharp
        currentPlaceStatus = newStatus;

        if (currentPlaceStatus != PlaceStatus.Preview)
        {
            ghostManager.HideGhost();
        }
```

- [ ] **Step 6: Inspect the focused script diff**

Run:

```powershell
git diff --check -- Assets/Scripts/Manager/GridManager.cs Assets/Scripts/Manager/PlaceManager.cs Assets/Scripts/Manager/GhostManager.cs
git diff -- Assets/Scripts/Manager/GridManager.cs Assets/Scripts/Manager/PlaceManager.cs Assets/Scripts/Manager/GhostManager.cs
```

Expected: no whitespace errors; selection, preview, and placement all use `GridManager` conversions.

---

### Task 4: Refresh Unity metadata and verify compilation

**Files:**
- Generated by Unity if absent: `Assets/Scripts/Manager/GhostManager.cs.meta`
- Generated by Unity: `Assembly-CSharp.csproj`

**Interfaces:**
- Consumes: all scripts from Tasks 1-3.
- Produces: a Unity-imported script with no C# compiler errors.

- [ ] **Step 1: Let Unity refresh assets**

Run:

```powershell
Test-Path Assets/Scripts/Manager/GhostManager.cs.meta
Select-String -Path Assembly-CSharp.csproj -Pattern 'GhostManager.cs'
```

Expected: `True` and one `Compile Include` match. If Unity is not open, launch Unity `6000.3.13f1` in batch mode with this project and write output to `Logs/CodexCompile.log`.

- [ ] **Step 2: Build the generated solution**

Run:

```powershell
dotnet restore Gomoku_Renju.slnx -v:minimal
dotnet build Gomoku_Renju.slnx --no-restore -v:minimal
```

Expected: build succeeds with `0 Error(s)`.

- [ ] **Step 3: Check the Unity editor log for script errors**

Run:

```powershell
Select-String -Path "$env:LOCALAPPDATA\Unity\Editor\Editor.log" -Pattern 'error CS|Assets/Scripts/Manager/(GridManager|PlaceManager|GhostManager).cs' | Select-Object -Last 30
```

Expected: no current `error CS` entries for the three modified scripts.

- [ ] **Step 4: Review final scope**

Run:

```powershell
git status --short
git diff --check
git diff --stat
```

Expected: requested scripts, the focused scene reference, and generated metadata are attributable to this implementation; pre-existing material and texture changes remain untouched.

- [ ] **Step 5: Complete Play Mode verification**

Verify in Unity:

1. Hovering an empty intersection shows a transparent stone of the current turn.
2. Hovering outside the board or over an occupied intersection hides the ghost.
3. Clicking places the same colored stone at the exact ghost position.
4. Moving `GomokuBoard` keeps selection, ghost, and actual placement aligned.

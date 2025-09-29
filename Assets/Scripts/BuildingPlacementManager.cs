using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildingPlacementManager : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;     // Основная Tilemap с полем
    [SerializeField] private Tilemap highlightTilemap;  // Tilemap для подсветки

    [Header("Tiles")]
    [SerializeField] private Tile greenTile; // Можно строить
    [SerializeField] private Tile redTile;   // Нельзя строить

    [Header("Settings")]
    [SerializeField] private Vector3Int[] allowedCells; // Клетки, где можно строить



    [Header("Buttons")]
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _cancelButton;
    [Space]
    [SerializeField] private Shop _shop;
    private GameObject pendingBuildingPrefab;
    private bool isPlacing = false;

    // Состояние каждой клетки (true = свободна, false = занята)
    private System.Collections.Generic.Dictionary<Vector3Int, bool> cellStates = new System.Collections.Generic.Dictionary<Vector3Int, bool>();

    void Start()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = groundTilemap.GetTile(pos);
            if (!tile) 
                continue;

            
            bool canBuild = System.Array.Exists(allowedCells, c => c == pos);
            cellStates[pos] = canBuild;
        }
    }

    void Update()
    {
        if (!isPlacing) return;

        Vector3Int clickedCell = GetClickedCell();
        if (clickedCell != Vector3Int.zero)
        {
            TryPlaceBuilding(clickedCell);
        }
    }


    public void StartPlacing(GameObject buildingPrefab)
    {
        _buyButton.gameObject.SetActive(false);
        _cancelButton.gameObject.SetActive(true);
        pendingBuildingPrefab = buildingPrefab;
        isPlacing = true;
        UpdateHighlight();
    }


    public void CancelPlacement()
    {
        RefundPlayer();
        StopPlacement();
    }

    private void StopPlacement()
    {
        ClearHighlight();
        isPlacing = false;
        _buyButton.gameObject.SetActive(true);
        _cancelButton.gameObject.SetActive(false);
    }

    private void TryPlaceBuilding(Vector3Int cell)
    {
        if (!cellStates.ContainsKey(cell) || !cellStates[cell]) return;

        // Получаем мировой центр тайла
        Vector3 cellWorldPos = groundTilemap.GetCellCenterWorld(cell);

        // Создаём объект с учётом поворота Tilemap
        GameObject obj = Instantiate(pendingBuildingPrefab, cellWorldPos, groundTilemap.transform.rotation);

        // Если нужно, ставим объект дочерним к Tilemap (для изометрии удобно)
        obj.transform.parent = groundTilemap.transform;

        // Обновляем состояние клетки
        cellStates[cell] = false;

        // Завершаем режим размещения
        StopPlacement();
    }



    private void UpdateHighlight()
    {
        highlightTilemap.ClearAllTiles();
        foreach (var kvp in cellStates)
        {
            highlightTilemap.SetTile(kvp.Key, kvp.Value ? greenTile : redTile);
        }
    }

    private void ClearHighlight()
    {
        highlightTilemap.ClearAllTiles();
    }

    private void RefundPlayer()
    {
        _shop.Refund();
    }


    private Vector3Int GetClickedCell()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            return groundTilemap.WorldToCell(worldPos);
        }
#endif

#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                worldPos.z = 0;
                return groundTilemap.WorldToCell(worldPos);
            }
        }
#endif
        return Vector3Int.zero;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class TileClickController : MonoBehaviour
{
    [SerializeField] int _gridWidth = 8;
    [SerializeField] int _gridHeight = 8;
    [SerializeField] float _cellSize = 1.0f;

    [SerializeField] Vector3 _originPosition = Vector3.zero;

    [SerializeField] GameObject _boardTilePrefab;

    [SerializeField] LayerMask _characterLayerMask;
    [SerializeField] LayerMask _groundLayerMask;

    ICharacter _characterInstance;
    GameObject _pickedCharacter;
    Tile _lastPickedTile;

    static Grid<Tile> _boardGrid;

    public static Grid<Tile> BoardGrid => _boardGrid;

    public static List<Tile> TileList { get; set; }


    void Awake()
    {
        _boardGrid = new Grid<Tile>(_gridWidth, _gridHeight, _cellSize, _originPosition, CreateNode);
        GameObject tileContainer = GameObject.Find("TileContainer");
        if (tileContainer == null)
            tileContainer = new GameObject("TileContainer");

        for (int i = 0; i < _boardGrid.GridWidth / 2; i++)
        {
            for (int j = 0; j < _boardGrid.GridHeight; j++)
            {
                GameObject spawnedTile = Instantiate(_boardTilePrefab, _boardGrid.GetWorldPosition(i, j), Quaternion.Euler(0, 0, 0));
                spawnedTile.name = $"Tile3D [{i}, {j}]";
                spawnedTile.transform.SetParent(tileContainer.transform);
            }
        }
    }

    void Update()
    {
        //prevent to set character's position from fight screen
        if (Camera.main.name != "MergeCamera")
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Tile tile;

        bool characterHit = Physics.Raycast(ray, out RaycastHit hitCharacterbj, 1000f, _characterLayerMask);
        bool groundHit = Physics.Raycast(ray, out RaycastHit hitGroundObj, 1000f, _groundLayerMask);

        if (characterHit)
        {
            tile = _boardGrid.GetGridObject(hitCharacterbj.transform.position);

            if (Input.GetMouseButtonDown(0))
            {
                if (tile != null)
                {
                    _lastPickedTile = tile;
                    tile.TileObject = null;
                    _pickedCharacter = hitCharacterbj.transform.gameObject;
                    _characterInstance = _pickedCharacter.GetComponent<ICharacter>();

                    SetTileState(tile, true);
                    return;
                }
            }
        }

        if (groundHit)
        {
            {
                tile = _boardGrid.GetGridObject(hitGroundObj.point);

                if (Input.GetMouseButton(0))
                {
                    if (_pickedCharacter != null)
                    {
                        _characterInstance.SetPosition(hitGroundObj.point);
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (_pickedCharacter == null)
                        return;

                    tile = _boardGrid.GetGridObject(hitGroundObj.point);

                    if (tile == null || tile.Row > 3)
                    {
                        _characterInstance.SetPosition(hitGroundObj.point);
                        SetTileState(_lastPickedTile, false);
                        _lastPickedTile.TileObject = _pickedCharacter;
                        _pickedCharacter = null;
                        return;
                    }

                    if (!tile.IsAvailable)
                    {
                        _characterInstance.SetPosition(hitGroundObj.point);
                        SetTileState(_lastPickedTile, false);
                        _lastPickedTile.TileObject = _pickedCharacter;
                    }
                    else
                    {
                        _characterInstance.SetPosition(hitGroundObj.point);
                        //Set character's tile id if character moved this tile
                        ICharacter ch = _pickedCharacter.GetComponent<ICharacter>();

                        SetTileState(tile, false);
                        tile.TileObject = _pickedCharacter;
                        if (_lastPickedTile != tile)
                            SetTileState(_lastPickedTile, true);
                    }

                    _pickedCharacter = null;
                }
            }
        }
    }

    void SetTileState(Tile tile, bool state)
    {
        tile.IsAvailable = state;
    }

    Vector3 GetTilePosition(Tile tile)
    {
        return tile.GetTilePosition();
    }

    Tile CreateNode(Grid<Tile> grid, int row, int column)
    {
        return new Tile(grid, row, column);
    }
}
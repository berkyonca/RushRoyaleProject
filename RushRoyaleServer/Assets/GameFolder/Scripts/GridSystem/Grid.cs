using UnityEngine;
using System;

public class Grid<TGridObject>
{
    int _gridWidth;
    int _gridHeight;
    float _cellSize;
    Vector3 _originPosition;
    TGridObject[,] _gridTiles;
    TGridObject[,] _enemyTiles;
    TGridObject[,] _playerTiles;
    public int GridWidth => _gridWidth;
    public int GridHeight => _gridHeight;
    public float CellSize => _cellSize;
    public Vector3 OriginPosition => _originPosition;
    public TGridObject[,] GridTiles => _gridTiles;
    public TGridObject[,] EnemyTiles => _enemyTiles;
    public TGridObject[,] PlayerTiles => _playerTiles;



    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        if (width < 0 || height < 0)
            return;

        _gridWidth = width;
        _gridHeight = height;
        _cellSize = cellSize;
        _originPosition = originPosition;

        _gridTiles = new TGridObject[width, height];

        //Create tiles
        for (int i = 0; i < _gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < _gridTiles.GetLength(1); j++)
            {
                _gridTiles[i, j] = createGridObject(this, i, j);

                Debug.DrawLine(GetWorldPosition(i, j) - new Vector3(1, 0, 1) * _cellSize / 2, GetWorldPosition(i, j + 1) - new Vector3(1, 0, 1) * _cellSize / 2, Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(i, j) - new Vector3(1, 0, 1) * _cellSize / 2, GetWorldPosition(i + 1, j) - new Vector3(1, 0, 1) * _cellSize / 2, Color.white, 100f);
            }
        }

        SetEnemyTiles();
        SetPlayerTiles();

        Debug.DrawLine(GetWorldPosition(width, 0) - new Vector3(1, 0, 1) * _cellSize / 2, GetWorldPosition(width, height) - new Vector3(1, 0, 1) * _cellSize / 2, Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(0, height) - new Vector3(1, 0, 1) * _cellSize / 2, GetWorldPosition(width, height) - new Vector3(1, 0, 1) * _cellSize / 2, Color.white, 100f);
    }
    //Get world position from row and column id
    public Vector3 GetWorldPosition(int row, int column)
    {
        return new Vector3(row, 0, column) * _cellSize + _originPosition + new Vector3(1, 0, 1) * _cellSize / 2;
    }
    //get row and column from world position
    public void GetRowAndColumn(Vector3 worldPosition, out int row, out int column)
    {
        row = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
        column = Mathf.FloorToInt((worldPosition - _originPosition).z / _cellSize);
    }

    public void SetGridObject(int row, int column, TGridObject obj)
    {
        if (row >= 0 && column >= 0 && row < _gridWidth && column < _gridHeight)
        {
            _gridTiles[row, column] = obj;
        }
    }


    public void SetGridObject(Vector3 worldPosition, TGridObject obj)
    {
        int row, column;
        GetRowAndColumn(worldPosition, out row, out column);
        SetGridObject(row, column, obj);
    }

    //Get tile from row and column id 
    public TGridObject GetGridObject(int row, int column)
    {
        if (row >= 0 && column >= 0 && row < _gridWidth && column < _gridHeight)
        {
            return _gridTiles[row, column];
        }
        else
            return default(TGridObject);
    }

    //Get tile from world position
    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int row, column;
        GetRowAndColumn(worldPosition, out row, out column);
        return GetGridObject(row, column);
    }


    public void SetEnemyTiles()
    {
        _enemyTiles = new TGridObject[(_gridTiles.GetLength(0) / 2), _gridTiles.GetLength(1)];

        for (int i = 0; i < (_gridTiles.GetLength(0) / 2); i++)
        {
            for (int j = 0; j < _gridTiles.GetLength(1); j++)
            {
                _enemyTiles[i, j] = _gridTiles[i + (_gridTiles.GetLength(0) / 2), j];
            }
        }
    }

    public void SetPlayerTiles()
    {
        _playerTiles = new TGridObject[(_gridTiles.GetLength(0) / 2), _gridTiles.GetLength(1)];

        for (int i = 0; i < (_gridTiles.GetLength(0) / 2); i++)
        {
            for (int j = 0; j < _gridTiles.GetLength(1); j++)
            {
                _playerTiles[i, j] = _gridTiles[i, j];
            }
        }
    }
}
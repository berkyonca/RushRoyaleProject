using UnityEngine;

public class Tile
{
    //Tile is belong this grid
    Grid<Tile> _grid;

    public int Row { get; set; }
    public int Column { get; set; }

    public bool IsAvailable { get; set; }
    public GameObject TileObject { get; set; }


    public Tile(Grid<Tile> grid, int row, int column)
    {
        _grid = grid;
        Row = row;
        Column = column;
        IsAvailable = true;
    }
    //Get world position from specific tile
    public Vector3 GetTilePosition()
    {
        return _grid.GetWorldPosition(Row, Column);
    }
}
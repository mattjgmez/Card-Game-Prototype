using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    [SerializeField] GameObject _tilePrefab;

    Tile[,] _grid;
    int _width = 6, _height = 5;

    protected override void Init()
    {
        _grid = new Tile[_width, _height];

        for (float x = 0; x < _width; x++)/* and */ for (float y = 0; y < _height; y++)
        {
            SpawnTile(x, y);
        }
    }

    void SpawnTile(float x, float y)
    {
        Tile spawnedTile = Instantiate
            (_tilePrefab, new Vector3(x - 2.5f, 0, y - 2), Quaternion.Euler(90, 0, 0), transform).GetComponent<Tile>();

        spawnedTile.gameObject.name = $"X:{x} Y:{y}";
        spawnedTile.SetGridPosition((int)x, (int)y);
        spawnedTile.SetIsPlayer1(x < 3);

        _grid[(int)x, (int)y] = spawnedTile;
    }

    public bool CanAdvance()
    {
        bool isPlayer1Turn = GameManager.Instance.CurrentTurn == GameState.Player1Turn;

        return !CheckColumn(isPlayer1Turn ? 3 : 2);
    }

    public bool CheckColumn(int x)
    {
        for (int y = 0; y < _height; y++)
        {
            Debug.Log($"Checking {_grid[x, y]} for active card...");
            if (_grid[x, y].ActiveCard) return true;
        }

        return false;
    }

    public bool CheckRow(int y)
    {
        for (int x = 0; x < _width; x++)
        {
            Debug.Log($"Checking {_grid[x, y]} for active card...");
            if (_grid[x, y].ActiveCard) return true;
        }

        return false;
    }

    public void ColumnSetActive(int x, bool value)
    {
        Debug.Log($"Toggling column {x}");
        for (int y = 0; y < _height; y++)
        {
            _grid[x, y].gameObject.SetActive(value);
        }
    }

    public List<Tile> GetBoardHalf(bool isPlayer1)
    {
        List<Tile> validTiles = new List<Tile>();

        foreach (Tile tile in _grid)
        {
            if (isPlayer1 && tile.GridPosition.x < 0)
                validTiles.Add(tile);
            else if (!isPlayer1 && tile.GridPosition.x > 0)
                validTiles.Add(tile);
        }

        return validTiles;
    }

    public Tile[,] Grid { get { return _grid; } }
}

    //Deprecated, replaced by ActionRanges class methods
    //public List<Tile> GetTilesInRange(Vector2Int gridPosition, List<Vector2Int> range, bool isPlayer1)
    //{
    //    List<Tile> validTiles = new List<Tile>();

    //    foreach (Vector2Int vector in range)
    //    {
    //        int x = gridPosition.x + (isPlayer1 ? vector.x : -vector.x);
    //        int y = gridPosition.y + (isPlayer1 ? vector.y : -vector.y);

    //        if (x < 0 || y < 0 || x > 5 || y > 4)
    //            continue;

    //        if (_grid[x, y] != null)
    //        {
    //            validTiles.Add(_grid[x, y]);
    //            Debug.Log($"{_grid[x, y]} set to in range.");
    //        }
    //    }
    //    return validTiles;
    //}

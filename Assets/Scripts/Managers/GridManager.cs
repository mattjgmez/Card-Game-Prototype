using System.Collections.Generic;
using UnityEngine;
using static GridSystem;

public class GridManager : MonoSingleton<GridManager>
{
    [SerializeField] private GameObject _tilePrefab;

    private Tile[,] _grid;
    private List<Tile> _playerTiles = new List<Tile>();
    readonly static int _width = 6;
    readonly static int _height = 5;

    protected override void Init()
    {
        _grid = new Tile[_width, _height];

        for (float x = 0; x < _width; x++)
        {
            for (float y = 0; y < _height; y++)
            {
                SpawnTile(x, y);
            }
        }
    }

    void SpawnTile(float x, float y)
    {
        Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x - 2.5f, 0, y - 2), Quaternion.Euler(90, 0, 0), transform).GetComponent<Tile>();

        spawnedTile.gameObject.name = $"X:{x} Y:{y}";
        spawnedTile.SetGridPosition((int)x, (int)y);

        if (x < 3)
        {
            spawnedTile.SetIsPlayer1(true);
            _playerTiles.Add(spawnedTile);
        }

        _grid[(int)x, (int)y] = spawnedTile;
    }

    /// <summary>
    /// Determines whether the current player can advance based on the number of active cards and the state of their frontline.
    /// </summary>
    /// <returns>True if the current player can advance, false otherwise.</returns>
    public bool CanAdvance()
    {
        bool isPlayer1Turn = TurnManager.Instance.CurrentTurn == PlayerTurn.Player1;
        int startColumn = isPlayer1Turn ? 0 : 3;
        int endColumn = isPlayer1Turn ? 3 : 6;
        int frontlineColumn = isPlayer1Turn ? 3 : 2;

        bool activeCards = ActiveCards(_grid, startColumn, endColumn).Count != 0;
        bool emptyFrontline = CardsInColumn(_grid, frontlineColumn).Count == 0;

        //Debug.Log($"{(isPlayer1Turn ? "Player 1" : "Player 2")} Active cards: {activeCards} Empty Frontline: {emptyFrontline}");
        return activeCards && emptyFrontline;
    }

    public void ColumnSetActive(int x, bool value)
    {
        //Debug.Log($"Toggling column {x}");
        for (int y = 0; y < _height; y++)
        {
            _grid[x, y].gameObject.SetActive(value);
        }
    }

    public void TogglePlayerSpaces(bool value)
    {
        foreach (Tile tile in _grid)
        {
            // If the tile is in playerTiles and has no active card, call SetTileActive with the value parameter
            if (_playerTiles.Contains(tile) && !tile.ActiveCard)
            {
                tile.SetTileActive(value);
            }
            // If the tile is in playerTiles and has an active card and value is false, call SetTileActive with false
            else if (_playerTiles.Contains(tile) && tile.ActiveCard && !value)
            {
                tile.SetTileActive(false);
            }
            // If the tile is not in playerTiles or has an active card, toggle the collider based on the value parameter
            else if (!_playerTiles.Contains(tile) || tile.ActiveCard)
            {
                tile.GetComponent<Collider>().enabled = !value;
            }
        }
    }

    public Tile[,] Grid { get { return _grid; } }
    public static int GridWidth { get { return _width; } }
    public static int GridHeight { get { return _height; } }
}

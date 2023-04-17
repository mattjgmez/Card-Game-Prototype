using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    [SerializeField] private GameObject _tilePrefab;

    private Tile[,] _grid;
    private List<Tile> _playerTiles = new List<Tile>();
    readonly int _width = 6;
    readonly int _height = 5;

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

        bool activeCards = ActiveCards(startColumn, endColumn).Count != 0;
        bool emptyFrontline = CardsInColumn(frontlineColumn).Count == 0;

        Debug.Log($"{(isPlayer1Turn ? "Player 1" : "Player 2")} Active cards: {activeCards} Empty Frontline: {emptyFrontline}");
        return activeCards && emptyFrontline;
    }

    /// <summary>
    /// Counts the number of active cards in the specified range of columns.
    /// </summary>
    /// <param name="startColumn">The starting column index, inclusive.</param>
    /// <param name="endColumn">The ending column index, exclusive.</param>
    /// <returns>The number of active cards in the specified range of columns.</returns>
    public List<UnitCard> ActiveCards(int startColumn, int endColumn)
    {
        List<UnitCard> activeCards = new List<UnitCard>();

        for (int x = startColumn; x < endColumn; x++)
        {
            activeCards.AddRange(CardsInColumn(x));
        }

        return activeCards;
    }

    public List<UnitCard> CardsInColumn(int x)
    {
        List<UnitCard> cardsInColumn = new List<UnitCard>();

        for (int y = _height - 1; y >= 0; y--)
        {
            if (TileHasActiveCard(x, y))
            {
                cardsInColumn.Add(Grid[x, y].ActiveCard);
            }
        }

        return cardsInColumn;
    }

    private bool TileHasActiveCard(int x, int y)
    {
        //Debug.Log($"Checking {_grid[x, y]} for active card...");
        return _grid[x, y].ActiveCard;
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
    public int GridWidth { get { return _width; } }
    public int GridHeight { get { return _height; } }
}

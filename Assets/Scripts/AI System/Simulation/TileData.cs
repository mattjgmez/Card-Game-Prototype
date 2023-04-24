using UnityEngine;

public class TileData
{
    public UnitCardData ActiveCard { get; set; }
    public Vector2Int GridPosition { get; set; }

    public static TileData FromTile(Tile tile)
    {
        if (tile == null)
        {
            Debug.LogError("TileData.FromTile: tile is null.");
            return null;
        }

        TileData tileState = new TileData
        {
            GridPosition = tile.GridPosition,
        };

        return tileState;
    }
}

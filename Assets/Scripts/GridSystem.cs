using System.Collections;
using System.Collections.Generic;

public static class GridSystem
{
    /// <summary>
    /// Counts the number of active cards in the specified range of columns.
    /// </summary>
    /// <param name="grid">The grid to check for active cards.</param>
    /// <param name="startColumn">The starting column index, inclusive.</param>
    /// <param name="endColumn">The ending column index, exclusive.</param>
    /// <returns>The number of active cards in the specified range of columns.</returns>
    public static List<UnitCard> ActiveCards(Tile[,] grid, int startColumn, int endColumn)
    {
        List<UnitCard> activeCards = new List<UnitCard>();

        for (int x = startColumn; x < endColumn; x++)
        {
            activeCards.AddRange(CardsInColumn(grid, x));
        }

        return activeCards;
    }

    /// <summary>
    /// Counts the amount of active cards in the given column.
    /// </summary>
    /// <param name="grid">The grid to check for active cards.</param>
    /// <param name="x">The column to check. Ranges from 0 to 5.</param>
    /// <returns></returns>
    public static List<UnitCard> CardsInColumn(Tile[,] grid, int x)
    {
        List<UnitCard> cardsInColumn = new List<UnitCard>();

        for (int y = 4; y >= 0; y--)
        {
            if (grid[x, y].ActiveCard != null)
            {
                cardsInColumn.Add(grid[x, y].ActiveCard);
            }
        }

        return cardsInColumn;
    }
}

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using static GridSystem_Simulated;

public class GameState
{
    #region VARIABLES
    private static int _counter = 0;
    public string ID;

    public List<CardData> Player1Hand;
    public List<CardData> Player2Hand;
    public List<UnitCardData> ActiveCards;
    public TileData[,] Grid;

    public PlayerTurn CurrentTurn;
    public int TurnNumber;

    public int Scale;
    public int Player1Supply;
    public int Player2Supply;

    // Variables to handle the weight of different GameStates
    public float BoardControlWeight = 1f;
    public float HandAdvantageWeight = -1f;
    public float SupplyAdvantageWeight = 0.01f;

    public float RangedWeight = 1f;
    public float ReachWeight = 0.5f;
    public float GlobalWeight = 2f;

    public float CleaveWeight = 1f;
    public float BurstWeight = 1f;
    public float NovaWeight = 2f;
    public float DrainWeight = 0.5f;
    public float DrawCardWeight = 0.5f;
    public float ProvokeWeight = 1f;
    public float DeathTouchWeight = 4f;
    public float OverkillWeight = 1f;
    #endregion

    #region CONSTRUCTOR
    public GameState()
    {
        // Initialize the Grid with the current state of the GridManager
        int gridWidth = GridManager.Instance.Grid.GetLength(0);
        int gridHeight = GridManager.Instance.Grid.GetLength(1);

        Grid = new TileData[gridWidth, gridHeight];

        CurrentTurn = TurnManager.Instance.CurrentTurn;
        TurnNumber = TurnManager.Instance.CurrentNumberOfTurns;

        ID = $"{(CurrentTurn == PlayerTurn.Player1 ? 1 : 2)}{++_counter:D4}";

        Player1Hand = new List<CardData>(GetHandData(1));
        Player2Hand = new List<CardData>(GetHandData(2));
        ActiveCards = new List<UnitCardData>();

        StringBuilder debugSB = new StringBuilder();
        debugSB.AppendLine($"Constructing GameState: ID:{ID} Player1Hand: {Player1Hand.Count} Player2Hand: {Player2Hand.Count}");
        debugSB.AppendLine();

        debugSB.AppendLine($"Cards in Player1 Hand: {DebugTools.ListToString(Player1Hand)}");
        debugSB.AppendLine();

        debugSB.AppendLine($"Cards in Player2 Hand: {DebugTools.ListToString(Player2Hand)}");
        debugSB.AppendLine();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = GridManager.Instance.Grid[x, y];
                Grid[x, y] = TileData.FromTile(tile);

                if (tile.HasCard)
                {
                    UnitCard cardToAdd = tile.ActiveCard.GetComponent<UnitCard>();
                    UnitCardData activeUnit = UnitCardData.FromUnitCard(cardToAdd);

                    Grid[x, y].ActiveCard = activeUnit;
                    activeUnit.CurrentTile = Grid[x, y];

                    ActiveCards.Add(activeUnit);

                    debugSB.AppendLine($"Adding {cardToAdd.GetName} to Grid at position: {Grid[x, y].GridPosition}");
                }
            }
        }
        debugSB.AppendLine();

        Debug.Log( debugSB.ToString() );

        Scale = TurnManager.Instance.Scale;
        Player1Supply = PlayerManager.Instance.GetCurrentSupply(1);
        Player2Supply = PlayerManager.Instance.GetCurrentSupply(2);
    }

    public GameState Clone()
    {
        GameState clonedState = new GameState
        {
            Player1Hand = new List<CardData>(Player1Hand),
            Player2Hand = new List<CardData>(Player2Hand),
            ActiveCards = new List<UnitCardData>(ActiveCards),

            Grid = Grid,

            CurrentTurn = CurrentTurn,
            TurnNumber = TurnNumber++,

            Scale = Scale,
            Player1Supply = Player1Supply,
            Player2Supply = Player2Supply,

            ID = $"{(CurrentTurn == PlayerTurn.Player1 ? 1 : 2)}{++_counter:D4}",
        };

        StringBuilder debugSB = new StringBuilder();
        debugSB.AppendLine($"Constructing GameState Clone: ID:{clonedState.ID} Player1Hand: {clonedState.Player1Hand.Count} Player2Hand: {clonedState.Player2Hand.Count}");
        debugSB.AppendLine();

        debugSB.AppendLine($"Cards in Player1 Hand: {DebugTools.ListToString(clonedState.Player1Hand)}");
        debugSB.AppendLine();

        debugSB.AppendLine($"Cards in Player2 Hand: {DebugTools.ListToString(clonedState.Player2Hand)}");
        debugSB.AppendLine();

        debugSB.Append($" {DebugTools.ListToString(clonedState.ActiveCards)}");
        debugSB.AppendLine();

        Debug.Log(debugSB.ToString());

        return clonedState;
    }

    public List<CardData> GetHandData(int playerNumber)
    {
        List<Card> hand = HandManager.Instance.GetHand(playerNumber);
        List<CardData> handData = new List<CardData>();

        foreach (Card card in hand)
        {
            //Debug.Log($"GameState{ID}.GetHandData({playerNumber}): Getting data for card: {card.CardInfo.Name} of type: {card.GetType()}");

            // Use reflection to determine the type of the card
            Type cardType = card.GetType();
            if (cardType == typeof(UnitCard))
            {
                UnitCard unitCard = (UnitCard)card;
                handData.Add(UnitCardData.FromUnitCard(unitCard));
            }
            else if (cardType == typeof(SpellCard))
            {
                handData.Add(SpellCardData.FromSpellCard((SpellCard)card));
            }
            else
            {
                Debug.LogWarning($"GameState{ID}.GetHandData({playerNumber}): Invalid type of: {card.GetType()}.");
            }
        }

        return handData;
    }

    public List<UnitCardData> GetActiveCardsData()
    {
        List<UnitCard> activeCards = new List<UnitCard>();
        int gridWidth = GridManager.Instance.Grid.GetLength(0);
        int gridHeight = GridManager.Instance.Grid.GetLength(1);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = GridManager.Instance.Grid[x, y];
                if (tile.HasCard && tile.ActiveCard.GetComponent<UnitCard>() != null)
                {
                    activeCards.Add(tile.ActiveCard.GetComponent<UnitCard>());
                }
            }
        }

        List<UnitCardData> activeCardsData = new List<UnitCardData>();
        foreach (UnitCard unitCard in activeCards)
        {
            activeCardsData.Add(UnitCardData.FromUnitCard(unitCard));
        }

        return activeCardsData;
    }

    public void RemoveActiveCard(UnitCardData card)
    {
        ActiveCards.Remove(card);
    }

    public TileData GetTileDataByCard(UnitCardData card)
    {
        int gridWidth = Grid.GetLength(0);
        int gridHeight = Grid.GetLength(1);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                TileData tile = Grid[x, y];

                if (tile.ActiveCard == card)
                {
                    return tile;
                }
            }
        }

        return null;
    }
    #endregion

    #region ACTIONS
    public void ApplySpellEffect(SpellCardData spellCardToPlay, Vector2Int areaOfEffectCenter)
    {
        if (spellCardToPlay == null)
        {
            Debug.LogError("spellCardToPlay is null");
            return;
        }

        if (spellCardToPlay.SpellInfo == null)
        {
            Debug.LogError("spellCardToPlay.CardInfo is null");
            return;
        }

        SpellInfo spell = spellCardToPlay.SpellInfo;
        if (spell == null)
        {
            Debug.LogError("spell is null");
            Debug.LogError("spellCardToPlay.CardInfo type: " + spellCardToPlay.CardInfo.GetType());
            return;
        }

        // Determine the area of effect and whether the spell targets enemies or allies
        Vector2Int areaOfEffect = spellCardToPlay.AreaOfEffect;
        List<bool> validTargets = spell.ValidTargets;
        bool isPlayer1 = spellCardToPlay.IsPlayer1;

        // Find the target tile closest to the area of effect center
        TileData cursorTarget = Grid[areaOfEffectCenter.x, areaOfEffectCenter.y];
        if (cursorTarget == null)
        {
            Debug.LogError("cursorTarget is null");
            return;
        }

        List<TileData> targetTiles = SpellSystem_Simulated.GetTargetTiles(cursorTarget, areaOfEffect, isPlayer1, validTargets, Grid);

        if (targetTiles == null)
        {
            Debug.LogError("targetTiles is null");
            return;
        }

        // Get the list of target cards (UnitCards) from the target tiles
        List<UnitCardData> targetCards = new List<UnitCardData>();
        foreach (TileData tile in targetTiles)
        {
            if (tile == null)
            {
                Debug.LogError("tile is null");
                continue;
            }

            if (tile.ActiveCard != null)
            {
                targetCards.Add(tile.ActiveCard);
            }
        }

        // Perform the spell on the target cards
        SpellSystem_Simulated.PerformSpell(spellCardToPlay, targetCards, this);

        // Remove the SpellCard from the player's hand
        RemoveCardFromHand(spellCardToPlay);
    }

    public void AddActiveUnit(UnitCardData unitCardToPlay, Vector2Int position)
    {
        // Remove the UnitCardData from the player's hand
        RemoveCardFromHand(unitCardToPlay);

        // Set the UnitCardData's position on the grid
        TileData targetTileData = Grid[position.x, position.y];
        targetTileData.ActiveCard = unitCardToPlay;

        unitCardToPlay.CurrentTile = targetTileData;

        // Add the UnitCardData to the list of active units
        ActiveCards.Add(unitCardToPlay);
    }

    public void RemoveCardFromHand(CardData cardToRemove)
    {
        if (cardToRemove.IsPlayer1)
        {
            Player1Hand.Remove(cardToRemove);
        }
        else if (!cardToRemove.IsPlayer1)
        {
            Player2Hand.Remove(cardToRemove);
        }
        else
        {
            Debug.LogError("Invalid card owner. Unable to remove the card from the hand.");
        }
    }

    public List<IGameAction> GetAvailableActions()
    {
        List<IGameAction> availableActions = new List<IGameAction>();

        // Get the current player's hand
        List<CardData> currentPlayerHand = CurrentTurn == PlayerTurn.Player1 ? Player1Hand : Player2Hand;

        StringBuilder debug = new StringBuilder();
        debug.AppendLine($"GameState{ID}.GetAvailableActions: CurrentPlayerHand: {DebugTools.ListToString(currentPlayerHand)}");

        foreach (CardData card in currentPlayerHand)
        {
            debug.AppendLine($"GameState{ID}.GetAvailableActions: Checking {card.Name} of type {card.GetType().Name} for available actions.");
            if (card is UnitCardData unitCard)
            {
                debug.AppendLine($"GameState{ID}.GetAvailableActions: Generating available actions for {unitCard.Name}.");
                GenerateUnitCardActions(unitCard, availableActions);
            }
            //Spell logic is disabled until other issues are solved
            //else if (card is SpellCardData spellCard)
            //{
            //    GenerateSpellCardActions(spellCard, availableActions);
            //}
            else
            {
                Debug.LogWarning($"GameState{ID}.GetAvailableActions: No available actions generated for {card.Name} of type {card.GetType().Name}.");
            }
        }

        // Add the EndTurnAction as an available action
        availableActions.Add(new EndTurnAction());

        debug.AppendLine($"GameState{ID}.GetAvailableActions: Available Actions: {DebugTools.ListToString(availableActions)}");
        Debug.Log(debug.ToString());

        return availableActions;
    }

    private void GenerateUnitCardActions(UnitCardData unitCard, List<IGameAction> availableActions)
    {
        int currentPlayerSupply = CurrentTurn == PlayerTurn.Player1 ? Player1Supply : Player2Supply;

        StringBuilder debug = new StringBuilder();

        if (unitCard.Cost <= currentPlayerSupply)
        {
            debug.AppendLine($"GameState{ID}.GenerateUnitCardActions: {(CurrentTurn == PlayerTurn.Player1 ? "Player1" : "Player2")} can afford to play {unitCard.Name}");
           
            foreach (int x in GetActiveColumns())
            {
                if ((CurrentTurn == PlayerTurn.Player1 && x < 3) || (CurrentTurn == PlayerTurn.Player2 && x > 2))
                {
                    for (int y = 0; y < 5; y++)
                    {
                        TileData tile = Grid[x, y];

                        debug.AppendLine($"GameState{ID}.GenerateUnitCardActions: Checking Tile {Grid[x,y].GridPosition} for active card.");
                        
                        if (tile.ActiveCard == null)
                        {
                            Vector2Int position = new Vector2Int(x, y);
                            PlayUnitCardAction action = new PlayUnitCardAction(unitCard, position);

                            availableActions.Add(action);

                            debug.AppendLine($"GameState{ID}.GenerateUnitCardActions: Adding possible action: {action}.");
                        }
                    }
                }
            }
        }

        Debug.Log(debug.ToString());
    }

    private void GenerateSpellCardActions(SpellCardData spellCard, List<IGameAction> availableActions)
    {
        foreach (int x in GetActiveColumns())
        {
            for (int y = 0; y < 5; y++)
            {
                Vector2Int areaOfEffectCenter = new Vector2Int(x, y);
                PlaySpellCardAction action = new PlaySpellCardAction(spellCard, areaOfEffectCenter);
                availableActions.Add(action);
            }
        }
    }

    public bool IsTerminal()
    {
        return Scale <= -4 || Scale >= 4;
    }

    public IGameAction GetRandomAction()
    {
        List<IGameAction> availableActions = GetAvailableActions();

        if (availableActions.Count == 0)
        {
            return new EndTurnAction(); // No available actions
        }

        System.Random random = new System.Random();
        int randomIndex = random.Next(availableActions.Count);

        return availableActions[randomIndex];
    }
    #endregion

    #region SCORING
    public float GetScore()
    {
        if (IsTerminal())
        {
            if (Scale <= -4) // AI wins
            {
                return float.MaxValue;
            }
            else // Player wins
            {
                return float.MinValue;
            }
        }

        // Each float is multiplied by a "weight" that decides how important it is to the AI
        float boardControl = CalculateBoardControl() * BoardControlWeight;
        float handAdvantage = (Player2Hand.Count - Player1Hand.Count) * HandAdvantageWeight;
        float supplyAdvantage = (Player2Supply - Player1Supply) * SupplyAdvantageWeight;

        float score = -Scale * 10 + boardControl + handAdvantage + supplyAdvantage;

        Debug.Log ($"GameState{ID}.GetScore: Scale: {Scale} BoardControl: {boardControl}, HandAdvantage: {handAdvantage}, SupplyAdvantage: {supplyAdvantage}, Total: {score}");

        return score;
    }

    public float CalculateBoardControl()
    {
        float aiControl = 0;
        float opponentControl = 0;

        foreach (UnitCardData card in ActiveCards)
        {
            // Calculate the base control value as the sum of attack and health
            float controlValue = card.Power + card.Health;

            // Adjust control value based on the card's special keywords or actions
            foreach (ActionInfo action in card.Actions)
            {
                controlValue += GetControlValueForAction(action.Keywords, action.Range, card.CurrentTile.GridPosition);
            }

            // Add the control value to either the AI or the opponent, based on the card's owner
            if (card.IsPlayer1)
            {
                opponentControl += controlValue;
            }
            else
            {
                aiControl += controlValue;
            }
        }

        // Return the difference in board control between the AI and the opponent
        return aiControl - opponentControl;
    }

    public float GetControlValueForAction(List<ActionKeyword> keywords, ActionRange range, Vector2Int position)
    {
        float controlValue = 0;

        // Adds value based on action range and position
        if (range == ActionRange.Ranged && !IsInFrontline(position))
        {
            controlValue += RangedWeight;
        }
        else if (range == ActionRange.Ranged || range == ActionRange.Reach)
        {
            controlValue += ReachWeight;
        }
        if (range == ActionRange.Global)
        {
            controlValue += GlobalWeight;
        }

        // Adds value based on action keywords
        if (keywords.Contains(ActionKeyword.Cleave))
        {
            controlValue += CleaveWeight;
        }
        if (keywords.Contains(ActionKeyword.Burst))
        {
            controlValue += BurstWeight;
        }
        if (keywords.Contains(ActionKeyword.Nova))
        {
            controlValue += NovaWeight;
        }
        if (keywords.Contains(ActionKeyword.Drain))
        {
            controlValue += DrainWeight;
        }
        if (keywords.Contains(ActionKeyword.DrawCard))
        {
            controlValue += DrawCardWeight;
        }
        if (keywords.Contains(ActionKeyword.Provoke))
        {
            controlValue += ProvokeWeight;
        }
        if (keywords.Contains(ActionKeyword.DeathTouch))
        {
            controlValue += DeathTouchWeight;
        }
        if (keywords.Contains(ActionKeyword.Overkill))
        {
            controlValue += OverkillWeight;
        }

        return controlValue;
    }

    public bool IsInFrontline(Vector2Int position)
    {
        // Assuming the center column is defined by a specific x value (e.g., 2)
        return position.x == 2 || position.x == 3;
    }
    #endregion

    #region ADVANCE PHASE CHECKS
    public void PerformAdvanceChecks()
    {
        bool opposingFrontColumnEmpty = IsOpposingFrontColumnEmpty();
        bool currentPlayerHasActiveCards = CurrentPlayerHasActiveCards();

        if (opposingFrontColumnEmpty && currentPlayerHasActiveCards)
        {
            MoveUnitsForward();
            UpdateScale();
        }
    }

    private bool IsOpposingFrontColumnEmpty()
    {
        int x = CurrentTurn == PlayerTurn.Player1 ? 3 : 2;

        List<UnitCardData> cardsInColumn = CardsInColumn(Grid, x);
        if (cardsInColumn.Count > 0)
        {
            return false;
        }

        return true;
    }

    private bool CurrentPlayerHasActiveCards()
    {
        int startColumn = CurrentTurn == PlayerTurn.Player1 ? 0 : 3;
        int endColumn = CurrentTurn == PlayerTurn.Player1 ? 3 : 6;

        for (int x = startColumn; x < endColumn; x++)
        {
            List<UnitCardData> cardsInColumn = CardsInColumn(Grid, x);
            if (cardsInColumn.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    private void MoveUnitsForward()
    {
        int startColumn = CurrentTurn == PlayerTurn.Player1 ? 3 : 0;
        int endColumn = CurrentTurn == PlayerTurn.Player1 ? 6 : 3;
        int direction = CurrentTurn == PlayerTurn.Player1 ? -1 : 1;

        List<UnitCardData> cardsToMove = new();
        for (int x = startColumn; x < endColumn; x++)
        {
            cardsToMove.AddRange(CardsInColumn(Grid, x));
        }

        if (cardsToMove.Count > 0)
        {
            foreach (UnitCardData card in cardsToMove)
            {
                TileData currentTile = card.CurrentTile;
                Vector2Int currentPosition = currentTile.GridPosition;
                TileData targetTile = Grid[currentPosition.x + direction, currentPosition.y];

                if (targetTile != null)
                {
                    currentTile.ActiveCard = null;
                    targetTile.ActiveCard = card;
                }
            }
        }
    }

    private void UpdateScale()
    {
        if (CurrentTurn == PlayerTurn.Player1)
        {
            Scale++;
        }
        else
        {
            Scale--;
        }
    }

    public List<int> GetActiveColumns()
    {
        List<int> activeColumns = new List<int>();

        activeColumns.AddRange(new int[] { 0, 1, 2, 3, 4, 5 });

        if (Scale < -1)
        {
            activeColumns.Remove(0);
        }
        if (Scale < -2)
        {
            activeColumns.Remove(1);
        }

        if (Scale > 1)
        {
            activeColumns.Remove(5);
        }
        if (Scale > 2)
        {
            activeColumns.Remove(4);
        }

        return activeColumns;
    }
    #endregion

    #region PHASES
    public void StartPhase()
    {
        // Implement logic for the Start phase
        // Trigger start of turn effects
    }

    public void ActionPhase()
    {
        // Implement logic for the Action phase
        // Trigger actions for each active card

        List<UnitCardData> activeCards = new List<UnitCardData>();

        bool isPlayer1Turn = CurrentTurn == PlayerTurn.Player1;

        int startColumn = isPlayer1Turn ? 0 : 5;
        int direction = isPlayer1Turn ? 1 : -1;

        for (int x = startColumn; isPlayer1Turn ? x < 3 : x > 2; x += direction)
        {
            List<UnitCardData> cardsInColumn = CardsInColumn(Grid, x);
            activeCards.AddRange(cardsInColumn);
        }

        Debug.Log($"GameState{ID}.ActionPhase: ActiveCards:{DebugTools.ListToString(activeCards)}");

        foreach (UnitCardData card in activeCards)
        {
            Debug.Log($"ActionPhase before TriggerAction: ActiveCard: {card.Name}, CurrentTile: {card.CurrentTile.GridPosition}");

            card.TriggerAction(this);

            Debug.Log($"ActionPhase after TriggerAction: ActiveCard: {card.Name}, CurrentTile: {card.CurrentTile.GridPosition}");

        }
    }

    public void AdvancePhase()
    {
        // Implement logic for the Advance phase
        // Move units forward if the conditions are met

        if (IsOpposingFrontColumnEmpty() && CurrentPlayerHasActiveCards())
        {
            MoveUnitsForward();
        }
    }

    public void DrawPhase()
    {
        // Implement logic for the Draw phase
        // Draw a card for each player
        if (Player1Hand.Count < 10)
        {
            Player1Hand.Add(DrawRandomCard(1));
        }
        if (Player2Hand.Count < 10)
        {
            Player2Hand.Add(DrawRandomCard(2));
        }
    }

    public void EndPhase()
    {
        // Implement logic for the End phase
        // Trigger end of turn effects

        foreach (UnitCardData card in ActiveCards)
        {
            card.ProvokingCard = null;
            card.IsProvoked = false;
        }
    }

    public CardData DrawRandomCard(int player)
    {
        // Load all available cards from the Resources folder
        SpellInfo[] spellInfos = Resources.LoadAll<SpellInfo>("ScriptableObjects/Spells");
        UnitInfo[] unitInfos = Resources.LoadAll<UnitInfo>("ScriptableObjects/Units");

        // Combine the arrays
        CardInfo[] allInfos = new CardInfo[spellInfos.Length + unitInfos.Length];
        Array.Copy(spellInfos, allInfos, spellInfos.Length);
        Array.Copy(unitInfos, 0, allInfos, spellInfos.Length, unitInfos.Length);

        // Pick a random card
        int randomIndex = UnityEngine.Random.Range(0, allInfos.Length);
        CardInfo randomInfo = allInfos[randomIndex];

        // Convert the random Info to CardData
        CardData randomCardData;
        if (randomInfo is SpellInfo spellInfo)
        {
            randomCardData = new SpellCardData(spellInfo);
        }
        else if (randomInfo is UnitInfo unitInfo)
        {
            randomCardData = new UnitCardData(unitInfo);
        }
        else
        {
            throw new InvalidOperationException("Invalid info type encountered in DrawRandomCard method.");
        }

        Debug.Log($"GameState{ID}.DrawRandomCard({player}): Card drawn type: {randomCardData.GetType()} Card: {randomCardData.Name}");
        randomCardData.IsPlayer1 = player == 1;

        return randomCardData;
    }

    public void SwitchTurn()
    {
        // Switch the current player
        CurrentTurn = CurrentTurn == PlayerTurn.Player1 ? PlayerTurn.Player2 : PlayerTurn.Player1;

        // Perform any other necessary updates related to the turn change
    }
    #endregion
}

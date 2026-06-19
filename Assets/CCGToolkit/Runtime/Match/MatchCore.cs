using System;
using System.Collections.Generic;
using System.Linq;
using CCGToolkit.Cards;
using UnityEngine;

namespace CCGToolkit.Match
{
    public enum MatchPhase { StartTurn, Main, EndTurn, Cleanup, Complete }
    public enum MatchCommandType { DrawCard, PlayCard, EndTurn, Attack, ChooseTarget, Concede }
    public enum MatchEventType { MatchStarted, CardShuffled, CardDrawn, FatigueDamage, ResourceChanged, CardPlayed, TurnStarted, TurnEnded, InvalidCommand, PlayerConceded }

    [Serializable]
    public sealed class MatchRules
    {
        [SerializeField] private int startingHealth = 30;
        [SerializeField] private int openingHandSize = 3;
        [SerializeField] private int maximumHandSize = 10;
        [SerializeField] private int maximumResources = 10;
        [SerializeField] private int fatigueStartDamage = 1;

        public int StartingHealth { get => startingHealth; set => startingHealth = Math.Max(1, value); }
        public int OpeningHandSize { get => openingHandSize; set => openingHandSize = Math.Max(0, value); }
        public int MaximumHandSize { get => maximumHandSize; set => maximumHandSize = Math.Max(1, value); }
        public int MaximumResources { get => maximumResources; set => maximumResources = Math.Max(0, value); }
        public int FatigueStartDamage { get => fatigueStartDamage; set => fatigueStartDamage = Math.Max(1, value); }
    }

    [Serializable]
    public sealed class MatchCardInstance
    {
        [SerializeField] private string instanceId;
        [SerializeField] private string cardId;
        [SerializeField] private string title;
        [SerializeField] private int cost;
        [SerializeField] private CardType cardType;
        [SerializeField] private int attack;
        [SerializeField] private int health;
        [SerializeField] private int durability;
        [SerializeField] private int temporaryCostModifier;

        public string InstanceId { get => instanceId; set => instanceId = value; }
        public string CardId { get => cardId; set => cardId = value; }
        public string Title { get => title; set => title = value; }
        public int Cost { get => cost; set => cost = Math.Max(0, value); }
        public CardType CardType { get => cardType; set => cardType = value; }
        public int Attack { get => attack; set => attack = value; }
        public int Health { get => health; set => health = value; }
        public int Durability { get => durability; set => durability = value; }
        public int TemporaryCostModifier { get => temporaryCostModifier; set => temporaryCostModifier = value; }
        public int EffectiveCost => Math.Max(0, cost + temporaryCostModifier);

        public static MatchCardInstance FromDefinition(CardDefinition definition, string instanceId)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            return new MatchCardInstance
            {
                instanceId = instanceId,
                cardId = definition.CardId.ToString(),
                title = definition.Title,
                cost = definition.Cost,
                cardType = definition.CardType,
                attack = definition.Stats.Attack,
                health = definition.Stats.Health,
                durability = definition.Stats.Durability
            };
        }
    }

    [Serializable]
    public sealed class MatchPlayerState
    {
        [SerializeField] private string playerId;
        [SerializeField] private int health;
        [SerializeField] private int armor;
        [SerializeField] private int resources;
        [SerializeField] private int maximumResources;
        [SerializeField] private int fatigueDamage;
        [SerializeField] private List<MatchCardInstance> deck = new List<MatchCardInstance>();
        [SerializeField] private List<MatchCardInstance> hand = new List<MatchCardInstance>();
        [SerializeField] private List<MatchCardInstance> board = new List<MatchCardInstance>();
        [SerializeField] private List<MatchCardInstance> graveyard = new List<MatchCardInstance>();
        [SerializeField] private List<MatchCardInstance> banished = new List<MatchCardInstance>();
        [SerializeField] private List<MatchCardInstance> secrets = new List<MatchCardInstance>();

        public string PlayerId { get => playerId; set => playerId = value; }
        public int Health { get => health; set => health = value; }
        public int Armor { get => armor; set => armor = Math.Max(0, value); }
        public int Resources { get => resources; set => resources = Math.Max(0, value); }
        public int MaximumResources { get => maximumResources; set => maximumResources = Math.Max(0, value); }
        public int FatigueDamage { get => fatigueDamage; set => fatigueDamage = Math.Max(0, value); }
        public List<MatchCardInstance> Deck => deck;
        public List<MatchCardInstance> Hand => hand;
        public List<MatchCardInstance> Board => board;
        public List<MatchCardInstance> Graveyard => graveyard;
        public List<MatchCardInstance> Banished => banished;
        public List<MatchCardInstance> Secrets => secrets;
    }

    [Serializable]
    public sealed class MatchState
    {
        [SerializeField] private int randomSeed;
        [SerializeField] private int turnNumber;
        [SerializeField] private int activePlayerIndex;
        [SerializeField] private MatchPhase phase;
        [SerializeField] private bool isComplete;
        [SerializeField] private string winnerPlayerId;
        [SerializeField] private List<MatchPlayerState> players = new List<MatchPlayerState>();
        [SerializeField] private List<MatchEvent> history = new List<MatchEvent>();

        public int RandomSeed { get => randomSeed; set => randomSeed = value; }
        public int TurnNumber { get => turnNumber; set => turnNumber = Math.Max(0, value); }
        public int ActivePlayerIndex { get => activePlayerIndex; set => activePlayerIndex = value; }
        public MatchPhase Phase { get => phase; set => phase = value; }
        public bool IsComplete { get => isComplete; set => isComplete = value; }
        public string WinnerPlayerId { get => winnerPlayerId; set => winnerPlayerId = value; }
        public List<MatchPlayerState> Players => players;
        public List<MatchEvent> History => history;
        public MatchPlayerState ActivePlayer => players[activePlayerIndex];
    }

    [Serializable]
    public sealed class MatchCommand
    {
        [SerializeField] private MatchCommandType type;
        [SerializeField] private string playerId;
        [SerializeField] private string sourceInstanceId;
        [SerializeField] private string targetId;
        public MatchCommandType Type { get => type; set => type = value; }
        public string PlayerId { get => playerId; set => playerId = value; }
        public string SourceInstanceId { get => sourceInstanceId; set => sourceInstanceId = value; }
        public string TargetId { get => targetId; set => targetId = value; }
        public static MatchCommand Draw(string playerId) => new MatchCommand { type = MatchCommandType.DrawCard, playerId = playerId };
        public static MatchCommand Play(string playerId, string instanceId) => new MatchCommand { type = MatchCommandType.PlayCard, playerId = playerId, sourceInstanceId = instanceId };
        public static MatchCommand EndTurn(string playerId) => new MatchCommand { type = MatchCommandType.EndTurn, playerId = playerId };
        public static MatchCommand Concede(string playerId) => new MatchCommand { type = MatchCommandType.Concede, playerId = playerId };
    }

    [Serializable]
    public sealed class MatchEvent
    {
        [SerializeField] private MatchEventType type;
        [SerializeField] private string playerId;
        [SerializeField] private string cardInstanceId;
        [SerializeField] private string message;
        [SerializeField] private int turnNumber;
        public MatchEventType Type { get => type; set => type = value; }
        public string PlayerId { get => playerId; set => playerId = value; }
        public string CardInstanceId { get => cardInstanceId; set => cardInstanceId = value; }
        public string Message { get => message; set => message = value; }
        public int TurnNumber { get => turnNumber; set => turnNumber = value; }
    }

    public sealed class DeterministicRandom
    {
        private uint state;
        public DeterministicRandom(int seed) { state = seed == 0 ? 0x6D2B79F5u : unchecked((uint)seed); }
        public int Next(int exclusiveMax)
        {
            if (exclusiveMax <= 0) throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
            state = unchecked(state * 1664525u + 1013904223u);
            return (int)(state % (uint)exclusiveMax);
        }
    }

    public sealed class MatchRunner
    {
        private readonly MatchRules rules;
        private readonly DeterministicRandom random;
        private readonly int seed;
        public MatchRunner(MatchRules rules, int seed) { this.rules = rules ?? new MatchRules(); this.seed = seed; random = new DeterministicRandom(seed); }

        public MatchState StartMatch(IList<CardDefinition> playerOneDeck, IList<CardDefinition> playerTwoDeck, string playerOneId = "player_1", string playerTwoId = "player_2")
        {
            var state = new MatchState { RandomSeed = seed, TurnNumber = 1, ActivePlayerIndex = 0, Phase = MatchPhase.Main };
            state.Players.Add(CreatePlayer(playerOneId, playerOneDeck));
            state.Players.Add(CreatePlayer(playerTwoId, playerTwoDeck));
            Shuffle(state.Players[0].Deck); Shuffle(state.Players[1].Deck);
            AddEvent(state, MatchEventType.MatchStarted, null, null, "Match started.");
            foreach (var player in state.Players)
                for (int i = 0; i < rules.OpeningHandSize; i++) DrawCard(state, player);
            AddEvent(state, MatchEventType.TurnStarted, state.ActivePlayer.PlayerId, null, "Turn started.");
            RefillForStartTurn(state.ActivePlayer);
            return state;
        }

        public bool TryExecute(MatchState state, MatchCommand command, out string error)
        {
            error = null;
            if (state == null) { error = "State is required."; return false; }
            if (command == null) { error = "Command is required."; return false; }
            if (state.IsComplete && command.Type != MatchCommandType.Concede) return Reject(state, command, "Match is complete.", out error);
            if (command.Type == MatchCommandType.Concede) { Concede(state, command.PlayerId); return true; }
            if (state.ActivePlayer.PlayerId != command.PlayerId) return Reject(state, command, "Only the active player may act.", out error);
            if (state.Phase != MatchPhase.Main) return Reject(state, command, "Commands are only accepted in main phase.", out error);

            switch (command.Type)
            {
                case MatchCommandType.DrawCard: DrawCard(state, state.ActivePlayer); return true;
                case MatchCommandType.PlayCard: return TryPlayCard(state, command, out error);
                case MatchCommandType.EndTurn: EndTurn(state); return true;
                default: return Reject(state, command, $"Command {command.Type} is not implemented in Phase 3.", out error);
            }
        }

        public string CreateSnapshot(MatchState state) => JsonUtility.ToJson(state, true);

        private MatchPlayerState CreatePlayer(string id, IList<CardDefinition> deck)
        {
            var player = new MatchPlayerState { PlayerId = id, Health = rules.StartingHealth, FatigueDamage = rules.FatigueStartDamage };
            for (int i = 0; i < deck.Count; i++) player.Deck.Add(MatchCardInstance.FromDefinition(deck[i], $"{id}_deck_{i}"));
            return player;
        }

        private void Shuffle<T>(IList<T> list) { for (int i = list.Count - 1; i > 0; i--) { int j = random.Next(i + 1); (list[i], list[j]) = (list[j], list[i]); } }
        private void DrawCard(MatchState state, MatchPlayerState player)
        {
            if (player.Deck.Count == 0) { player.Health -= player.FatigueDamage; AddEvent(state, MatchEventType.FatigueDamage, player.PlayerId, null, $"Fatigue {player.FatigueDamage}."); player.FatigueDamage++; return; }
            var card = player.Deck[0]; player.Deck.RemoveAt(0);
            if (player.Hand.Count >= rules.MaximumHandSize) player.Graveyard.Add(card); else player.Hand.Add(card);
            AddEvent(state, MatchEventType.CardDrawn, player.PlayerId, card.InstanceId, "Card drawn.");
        }
        private bool TryPlayCard(MatchState state, MatchCommand command, out string error)
        {
            var player = state.ActivePlayer;
            var card = player.Hand.FirstOrDefault(c => c.InstanceId == command.SourceInstanceId);
            if (card == null) return Reject(state, command, "Card is not in hand.", out error);
            if (player.Resources < card.EffectiveCost) return Reject(state, command, "Not enough resources.", out error);
            player.Resources -= card.EffectiveCost; player.Hand.Remove(card);
            if (card.CardType == CardType.Minion || card.CardType == CardType.Token) player.Board.Add(card); else player.Graveyard.Add(card);
            AddEvent(state, MatchEventType.CardPlayed, player.PlayerId, card.InstanceId, "Card played."); error = null; return true;
        }
        private void EndTurn(MatchState state)
        {
            state.Phase = MatchPhase.EndTurn; AddEvent(state, MatchEventType.TurnEnded, state.ActivePlayer.PlayerId, null, "Turn ended.");
            state.Phase = MatchPhase.Cleanup; state.ActivePlayer.Hand.ForEach(c => c.TemporaryCostModifier = 0);
            state.ActivePlayerIndex = (state.ActivePlayerIndex + 1) % state.Players.Count; if (state.ActivePlayerIndex == 0) state.TurnNumber++;
            state.Phase = MatchPhase.StartTurn; DrawCard(state, state.ActivePlayer); RefillForStartTurn(state.ActivePlayer);
            state.Phase = MatchPhase.Main; AddEvent(state, MatchEventType.TurnStarted, state.ActivePlayer.PlayerId, null, "Turn started.");
        }
        private void RefillForStartTurn(MatchPlayerState player) { player.MaximumResources = Math.Min(rules.MaximumResources, player.MaximumResources + 1); player.Resources = player.MaximumResources; }
        private void Concede(MatchState state, string playerId) { state.IsComplete = true; state.Phase = MatchPhase.Complete; state.WinnerPlayerId = state.Players.FirstOrDefault(p => p.PlayerId != playerId)?.PlayerId; AddEvent(state, MatchEventType.PlayerConceded, playerId, null, "Player conceded."); }
        private bool Reject(MatchState state, MatchCommand command, string message, out string error) { error = message; AddEvent(state, MatchEventType.InvalidCommand, command.PlayerId, command.SourceInstanceId, message); return false; }
        private void AddEvent(MatchState state, MatchEventType type, string playerId, string cardId, string message) => state.History.Add(new MatchEvent { Type = type, PlayerId = playerId, CardInstanceId = cardId, Message = message, TurnNumber = state.TurnNumber });
    }
}

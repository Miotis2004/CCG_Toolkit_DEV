using System.Collections.Generic;
using CCGToolkit.Cards;
using CCGToolkit.Match;
using NUnit.Framework;
using UnityEngine;

namespace CCGToolkit.Tests.EditMode
{
    public sealed class DeterministicMatchCoreTests
    {
        [Test]
        public void SameSeedProducesSameOpeningHands()
        {
            var deck = CreateDeck(8);
            var rules = new MatchRules { OpeningHandSize = 3 };

            var first = new MatchRunner(rules, 1234).StartMatch(deck, deck);
            var second = new MatchRunner(rules, 1234).StartMatch(deck, deck);

            Assert.That(HandIds(first, 0), Is.EqualTo(HandIds(second, 0)));
            Assert.That(HandIds(first, 1), Is.EqualTo(HandIds(second, 1)));
        }

        [Test]
        public void EndTurnSwitchesActivePlayerAndRefillsResources()
        {
            var state = new MatchRunner(new MatchRules { OpeningHandSize = 0 }, 7).StartMatch(CreateDeck(4), CreateDeck(4));

            bool executed = new MatchRunner(new MatchRules { OpeningHandSize = 0 }, 7).TryExecute(state, MatchCommand.EndTurn("player_1"), out string error);

            Assert.That(executed, Is.True, error);
            Assert.That(state.ActivePlayer.PlayerId, Is.EqualTo("player_2"));
            Assert.That(state.ActivePlayer.Resources, Is.EqualTo(1));
            Assert.That(state.ActivePlayer.MaximumResources, Is.EqualTo(1));
        }

        [Test]
        public void PlayingCardSpendsResourcesAndMovesCardToBoard()
        {
            var state = new MatchRunner(new MatchRules { OpeningHandSize = 1 }, 1).StartMatch(CreateDeck(1, 1), CreateDeck(1));
            var runner = new MatchRunner(new MatchRules { OpeningHandSize = 1 }, 1);
            string cardId = state.ActivePlayer.Hand[0].InstanceId;

            bool executed = runner.TryExecute(state, MatchCommand.Play("player_1", cardId), out string error);

            Assert.That(executed, Is.True, error);
            Assert.That(state.ActivePlayer.Resources, Is.Zero);
            Assert.That(state.ActivePlayer.Hand, Is.Empty);
            Assert.That(state.ActivePlayer.Board, Has.Count.EqualTo(1));
        }

        [Test]
        public void InvalidCommandIsRejectedAndRecorded()
        {
            var state = new MatchRunner(new MatchRules { OpeningHandSize = 0 }, 3).StartMatch(CreateDeck(1), CreateDeck(1));
            var runner = new MatchRunner(new MatchRules { OpeningHandSize = 0 }, 3);

            bool executed = runner.TryExecute(state, MatchCommand.Play("player_2", "missing"), out string error);

            Assert.That(executed, Is.False);
            Assert.That(error, Is.EqualTo("Only the active player may act."));
            Assert.That(state.History, Has.Exactly(1).Matches<MatchEvent>(e => e.Type == MatchEventType.InvalidCommand));
        }

        private static List<CardDefinition> CreateDeck(int count, int cost = 0)
        {
            var cards = new List<CardDefinition>();
            for (int i = 0; i < count; i++)
            {
                var card = ScriptableObject.CreateInstance<CardDefinition>();
                card.CardId = new CardIdentifier($"card_{i}");
                card.Title = $"Card {i}";
                card.Cost = cost;
                card.CardType = CardType.Minion;
                card.Stats.Enabled = true;
                card.Stats.Attack = 1;
                card.Stats.Health = 1;
                cards.Add(card);
            }
            return cards;
        }

        private static string[] HandIds(MatchState state, int playerIndex)
        {
            var ids = new string[state.Players[playerIndex].Hand.Count];
            for (int i = 0; i < ids.Length; i++) ids[i] = state.Players[playerIndex].Hand[i].CardId;
            return ids;
        }
    }
}

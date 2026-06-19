# Unity CCG Toolkit

Unity CCG Toolkit is a planned Unity 6 framework for building collectible card games in the style of Hearthstone, Legends of Runeterra, and other digital card battlers. The goal is to provide designers and programmers with a complete, extensible foundation for creating cards, collections, decks, turn-based matches, combat rules, visual presentation, AI opponents, and production-ready game flows.

## Vision

The toolkit should make it possible to create a fully functional CCG without rebuilding the same systems from scratch for every project. It will combine ScriptableObject-driven card authoring, deterministic gameplay simulation, configurable rule sets, polished Unity UI workflows, and developer tools for testing and balancing.

The project is intended to support both rapid prototyping and long-term production. Designers should be able to add new cards, keywords, factions, costs, images, descriptions, and effects through editor tools, while engineers should be able to extend the rules engine, networking layer, persistence model, and presentation systems.

## Target Unity Version

- Unity 6 or newer
- Universal Render Pipeline recommended for sample scenes and visual effects
- Input System package recommended for modern input handling
- Addressables recommended for scalable card art, audio, and cosmetic content

## Core Goals

- Provide a complete card creation pipeline for image, name, description, rarity, type, cost, stats, factions, tags, and effect data.
- Support full gameplay mechanics for hand, deck, board, graveyard/discard, resources, turns, combat, targeting, triggered abilities, and win/loss conditions.
- Offer reusable Unity UI components for menus, deck building, collection browsing, card previews, match HUDs, drag-and-drop play, targeting, tooltips, and history logs.
- Include a deterministic rules engine that can be tested independently from Unity presentation code.
- Support human-vs-AI first, with an architecture that can later support online multiplayer.
- Provide sample content and example cards demonstrating minions, spells, weapons, secrets/traps, hero powers, keywords, and triggered effects.
- Include automated tests, simulation tools, and balance utilities for long-term maintainability.

## Planned Feature Set

### Card Design and Content Authoring

- ScriptableObject card definitions for reusable card data.
- Card fields for title, subtitle, rules text, flavor text, cost, attack, health, durability, armor, class/faction, rarity, set, artist credit, image, frame style, and tags.
- Support for card categories such as minions, spells, heroes, weapons, locations, enchantments, secrets, tokens, and generated cards.
- Keyword and mechanic definitions that can be shared across cards.
- Editor windows for browsing, filtering, creating, validating, and bulk-editing card assets.
- Optional CSV/JSON import and export for designers who prefer spreadsheet workflows.
- Automatic validation for missing art, invalid costs, duplicate IDs, malformed rules text, and unsupported effect references.

### Gameplay and Rules Engine

- Deterministic match state model independent from Unity visuals.
- Player zones: deck, hand, board, graveyard/discard, banished/exile, secrets/traps, and temporary generated zones.
- Turn system with start turn, draw, main/action, combat, end turn, fatigue, and cleanup phases.
- Mana/resource system with configurable maximums, refill behavior, overload/locked resources, alternative costs, and temporary discounts.
- Card play pipeline covering requirements, target selection, payment, resolution, interrupts, and post-resolution triggers.
- Combat model for attackers, defenders, summoning sickness, taunt/guard, stealth, damage, healing, armor, death processing, and simultaneous effects.
- Event and trigger system for effects such as battlecry, deathrattle, aura, start/end turn, draw, discard, summon, cast, damage, heal, and custom events.
- Stack/queue-based effect resolution for predictable sequencing.
- Configurable win, loss, and draw conditions.
- Undo-free authoritative command model suitable for AI, replays, and future multiplayer.

### Effects and Mechanics

- Modular effect definitions for damage, heal, draw, discard, summon, destroy, silence, transform, buff, debuff, copy, discover/select, generate, shuffle, equip, freeze, stun, and resource manipulation.
- Targeting rules for friendly, enemy, all, random, adjacent, damaged, keyword-filtered, type-filtered, zone-filtered, and custom predicates.
- Conditional effects and branching choices.
- Persistent auras and temporary enchantments.
- Keyword framework for mechanics such as Taunt, Charge/Rush, Lifesteal, Divine Shield, Poisonous, Stealth, Windfury, Freeze, Immune, Spell Damage, and custom project keywords.
- Extensible scripting hooks for custom card behavior that cannot be represented by built-in data definitions.

### User Interface and Presentation

- Main menu, play menu, settings, collection, deck builder, pack-opening placeholder, and match scenes.
- Card view prefabs for hand cards, board cards, full-size previews, deck list entries, collection grid cells, and generated tokens.
- Drag-and-drop card play with legal-play highlighting.
- Targeting arrows, board slot highlights, hover previews, tooltips, and contextual prompts.
- Turn timer, end-turn button, player resource displays, hero panels, combat animations, damage/heal numbers, and event history.
- Animation controller hooks for draw, play, summon, attack, death, transform, discard, and victory/defeat sequences.
- Audio hooks for card play, impact, UI, turn transition, and ambience.

### Decks, Collections, and Progression

- Deck definition assets and runtime deck instances.
- Deck-building rules for size limits, copy limits, class/faction restrictions, banned cards, and format legality.
- Local player collection model with owned card counts, favorites, cosmetics, and unlock flags.
- Starter decks and sample constructed decks.
- Save/load support for decks, collections, settings, and tutorial progress.
- Future-ready interfaces for inventory services, economy systems, quests, achievements, and backend sync.

### AI, Simulation, and Testing

- Baseline AI opponent capable of legal plays, target selection, combat decisions, and turn sequencing.
- Heuristic AI profiles for aggressive, control, random, and tutorial behavior.
- Headless simulation mode for thousands of automated matches.
- Balance reports for win rates, card usage, average game length, resource curves, and draw consistency.
- Unit tests for core rules, card effects, combat, targeting, deck validation, and save/load flows.
- Golden replay tests to ensure deterministic behavior across engine changes.

### Multiplayer Readiness

The first production milestone should focus on local deterministic gameplay and AI. However, the architecture should prepare for multiplayer by separating commands from visuals, keeping match state serializable, validating actions server-side, and recording replays. Later phases can add lobby, matchmaking, reconnection, spectator, and authoritative networking integrations.

## Proposed Package Structure

```text
Assets/
  CCGToolkit/
    Runtime/
      Cards/
      Rules/
      Effects/
      Match/
      Players/
      Decks/
      Collections/
      Persistence/
      AI/
      Presentation/
      Utilities/
    Editor/
      CardDatabase/
      DeckTools/
      Validation/
      Simulation/
    Tests/
      EditMode/
      PlayMode/
    Samples/
      BasicGame/
      SampleCards/
      SampleDecks/
      ArtPlaceholders/
Docs/
  DEVELOPMENT.md
  Architecture.md
  CardAuthoring.md
  RulesEngine.md
```

## Development Principles

- Keep the rules engine deterministic, serializable, and testable without scene dependencies.
- Keep presentation code reactive: visuals should observe match state and commands rather than own game rules.
- Prefer data-driven card definitions for common behavior and code extensions for truly custom behavior.
- Validate content early and often through editor tooling and automated tests.
- Design APIs so projects can replace art, UI, networking, economy, and persistence without rewriting the rules core.
- Build sample content alongside systems to prove that designer workflows are practical.

## Initial Deliverables

1. Core card data model and sample card assets.
2. Deterministic match state with draw, hand, deck, resources, turns, and basic card play.
3. Basic minion combat and spell resolution.
4. Unity match scene with hand, board, heroes, resources, and end-turn flow.
5. Deck builder and collection browser prototypes.
6. AI opponent and automated simulation runner.
7. Documentation, examples, and tests for extending cards and mechanics.

## Status

Phase 0 project foundation is in place. See [DEVELOPMENT.md](DEVELOPMENT.md) for the detailed implementation roadmap and `Docs/Validation.md` for local validation commands.

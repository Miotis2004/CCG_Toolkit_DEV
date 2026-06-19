# Unity CCG Toolkit Development Roadmap

This document defines the phased development plan for building a Unity 6 toolkit for collectible card games. The roadmap is written as a practical sequence of milestones that can be implemented, tested, documented, and released incrementally.

## Phase 0: Project Foundation

### Goals

Establish the Unity project structure, coding standards, package layout, documentation conventions, and baseline automation before feature development begins.

### Steps

1. Create or verify the Unity 6 project and configure source control metadata.
2. Define the package namespace, assembly definitions, runtime/editor split, and tests folder layout.
3. Add basic documentation files for architecture, card authoring, rules engine concepts, and contribution guidelines.
4. Configure Unity packages: Input System, TextMeshPro, Addressables, test framework, and optional URP sample support.
5. Establish code style rules, naming conventions, folder conventions, and serialization conventions.
6. Add CI-friendly test commands and document local validation steps.
7. Create placeholder sample scenes for main menu, collection, deck builder, and match gameplay.
8. Define the minimum viable vertical slice and acceptance criteria for each future phase.

### Deliverables

- Clean Unity folder structure.
- Runtime and editor assemblies.
- Empty sample scenes.
- Initial test assembly.
- Documentation skeleton.

## Phase 1: Card Data Model and Authoring Basics

### Goals

Create the foundational card definition system that designers will use to describe card identity, visuals, stats, costs, types, and metadata.

### Steps

1. Implement a stable card ID strategy using GUID-backed or string-backed identifiers.
2. Create `CardDefinition` ScriptableObjects with fields for name, description, cost, art, type, rarity, class/faction, set, tags, and presentation data.
3. Add specialized optional stat blocks for attack, health, durability, armor, and resource modifiers.
4. Define enums or data assets for card type, rarity, faction/class, set, keyword, zone, and target type.
5. Add validation methods for missing IDs, invalid costs, missing titles, duplicate names, missing art, and unsupported combinations.
6. Create placeholder card frame and art assets for sample content.
7. Create 20 to 30 sample cards covering minions, spells, weapons, tokens, and generated cards.
8. Add edit-mode tests for data validation and asset loading.

### Deliverables

- ScriptableObject card schema.
- Shared metadata definitions.
- Sample card assets.
- Card validation tests.

## Phase 2: Card Database and Editor Tools

### Goals

Make card creation scalable by giving designers a searchable database, bulk-editing workflows, validation reports, and import/export options.

### Steps

1. Implement a card database service that can discover and index all card assets.
2. Build an editor window for listing, searching, filtering, and opening cards.
3. Add validation reports with severity levels for errors, warnings, and suggestions.
4. Add bulk operations for set, rarity, faction, keyword, and balance field changes.
5. Implement CSV and JSON import/export for core card fields.
6. Add duplicate ID detection and automatic ID generation tools.
7. Add preview rendering for card art, frame, stats, and rules text.
8. Document card authoring workflows for designers.

### Deliverables

- Card database API.
- Card browser editor window.
- Bulk edit tools.
- Import/export pipeline.
- Authoring documentation.

## Phase 3: Deterministic Match Core

### Goals

Build the rules engine foundation independent from Unity scene visuals. This phase should make it possible to run a simple game entirely in edit-mode tests.

### Steps

1. Define serializable match state containing players, zones, resources, health, armor, board, turn number, active player, and random seed.
2. Define immutable or controlled command objects such as draw card, play card, end turn, attack, choose target, and concede.
3. Implement a match runner that validates commands, mutates state, emits events, and records history.
4. Implement deck shuffle, opening hand, mulligan placeholder, draw, fatigue, and hand-size rules.
5. Implement resource gain, refill, spending, temporary discounts, and maximum resource rules.
6. Implement turn transitions with start-turn, main-phase, end-turn, cleanup, and active-player switching.
7. Add deterministic random number generation and seed control.
8. Add state serialization snapshots for debugging and future replay support.
9. Create edit-mode tests for draw order, turn sequencing, resource behavior, and invalid command rejection.

### Deliverables

- Serializable match state.
- Command validation and execution pipeline.
- Deterministic RNG service.
- Core rules tests.

## Phase 4: Zones, Decks, and Collections

### Goals

Implement reusable systems for decks, runtime zone movement, legal deck construction, and player-owned card collections.

### Steps

1. Implement runtime card instances separate from card definitions.
2. Implement zones for deck, hand, board, graveyard/discard, banished/exile, secrets/traps, and temporary generated cards.
3. Add a zone movement service that records source, destination, reason, and event payloads.
4. Create deck definition assets with card references and quantities.
5. Add deck validation for deck size, card copies, faction/class legality, banned cards, format legality, and required hero/card identity.
6. Implement local collection data with owned quantities, favorites, cosmetics, and unlock flags.
7. Add save/load support for collection and decks using JSON or Unity serialization.
8. Create sample starter decks and deck validation tests.

### Deliverables

- Runtime card instance model.
- Zone service.
- Deck assets and validation.
- Collection persistence prototype.

## Phase 5: Effects, Targeting, and Event System

### Goals

Enable cards to do meaningful things through modular effects, target filters, conditions, event hooks, and deterministic resolution.

### Steps

1. Define the match event model for card played, card drawn, minion summoned, damage dealt, character healed, minion died, turn started, turn ended, and custom events.
2. Implement an event queue and effect resolution queue with deterministic ordering.
3. Define target selectors for friendly, enemy, all, random, adjacent, damaged, keyword-filtered, type-filtered, zone-filtered, and custom predicate targets.
4. Implement basic effects: damage, heal, draw, discard, summon, destroy, buff, debuff, silence, transform, generate, shuffle, freeze, and resource modification.
5. Add conditional effect blocks and choice/select-one effects.
6. Implement triggered abilities such as battlecry, deathrattle, start-turn, end-turn, on-draw, on-summon, on-damage, and on-spell-cast.
7. Implement persistent enchantments and auras.
8. Add tests for every effect and target selector.

### Deliverables

- Event bus and resolution queue.
- Modular effect library.
- Targeting framework.
- Trigger and aura systems.
- Effect test suite.

## Phase 6: Combat and Board Rules

### Goals

Create a complete tactical board model for minion-style combat and configurable board constraints.

### Steps

1. Implement board slots, maximum board size, summon positions, adjacency, and board ordering.
2. Implement attack rules for ready state, attack counts, summoning sickness, charge/rush behavior, frozen state, stealth restrictions, taunt/guard restrictions, and immune targets.
3. Implement damage, armor, healing, maximum health, temporary health, and death processing.
4. Implement simultaneous combat damage and post-combat triggers.
5. Add keyword framework support for Taunt, Charge, Rush, Lifesteal, Divine Shield, Poisonous, Stealth, Windfury, Freeze, Immune, and Spell Damage.
6. Add configurable rule profiles for projects that need different board sizes or combat rules.
7. Add combat-focused edit-mode tests and replay fixtures.

### Deliverables

- Board state and positioning.
- Combat validation and resolution.
- Keyword framework.
- Combat tests.

## Phase 7: Unity Match Presentation

### Goals

Connect the deterministic match core to a playable Unity scene with clear UI, drag-and-drop interactions, animations, and feedback.

### Steps

1. Build match scene layout with player hero panels, opponent panels, hand area, board area, deck indicators, graveyard indicators, mana/resources, and end-turn button.
2. Create card view prefabs for hand, board, preview, collection, deck list, and history entries.
3. Implement a presentation controller that observes match events and updates Unity views without owning core rules.
4. Implement drag-and-drop card play and click-to-select fallback controls.
5. Implement target selection arrows, legal target highlights, board slot highlights, invalid-action feedback, and confirmation prompts.
6. Add animation hooks for draw, play, summon, attack, damage, heal, death, transform, discard, and turn transition.
7. Add audio hooks and placeholder sound events.
8. Add play-mode tests for scene loading and key UI interactions where practical.

### Deliverables

- Playable local match scene.
- Reusable card view prefabs.
- Input and targeting UX.
- Presentation event bridge.

## Phase 8: Deck Builder and Collection UI

### Goals

Provide player-facing tools for browsing owned cards, filtering content, building decks, and validating deck legality.

### Steps

1. Build collection scene with card grid, search, filters, sort options, rarity/type/faction tabs, and full-card preview.
2. Build deck builder scene with deck list, card quantity controls, curve display, legality warnings, and save/delete/duplicate actions.
3. Connect UI to collection and deck persistence services.
4. Add starter deck selection and sample deck import.
5. Add card crafting/disenchanting placeholders if the target product needs an economy later.
6. Add responsive layout rules for common desktop and tablet aspect ratios.
7. Add play-mode smoke tests for opening collection, creating a deck, saving it, and loading it.

### Deliverables

- Collection browser.
- Deck builder.
- Deck persistence UI.
- Starter deck workflow.

## Phase 9: AI and Simulation

### Goals

Create opponents, tutorials, and balance tooling by implementing AI decision-making and automated simulation.

### Steps

1. Implement legal action generation for the current match state.
2. Create a random AI to prove command execution across full games.
3. Create heuristic AI profiles for aggressive, control, tempo, value, and tutorial behavior.
4. Implement target and combat scoring functions.
5. Add AI turn executor with configurable thinking delay for presentation scenes and instant execution for simulations.
6. Add headless simulation runner for batch matches.
7. Generate balance reports for win rate, average game length, cards played, cards drawn, resource usage, and lethal turns.
8. Add deterministic replay output for failed simulations.

### Deliverables

- AI action generator.
- Random and heuristic AI profiles.
- Simulation runner.
- Balance report output.

## Phase 10: Persistence, Replays, and Diagnostics

### Goals

Make development and live debugging easier through robust save data, replay capture, logs, and diagnostic tooling.

### Steps

1. Finalize save data schemas for settings, collection, decks, tutorial progress, and local account profile.
2. Add migration support for save schema version changes.
3. Implement match replay recording from seed, deck lists, commands, and choices.
4. Add replay playback controls for stepping, fast-forward, pause, and event inspection.
5. Add in-game debug overlays for match state, stack/queue contents, event history, and legal actions.
6. Add editor diagnostics for card database health, missing assets, invalid references, and package setup.
7. Add crash-safe logging around command validation and effect resolution.

### Deliverables

- Save system and migrations.
- Replay recorder/player.
- Debug overlays.
- Diagnostic editor tools.

## Phase 11: Multiplayer Architecture Preparation

### Goals

Prepare the toolkit for authoritative online play without forcing every project to use a specific networking provider.

### Steps

1. Separate local command submission from command authority and validation interfaces.
2. Define network-serializable command, match state, snapshot, and replay payloads.
3. Add hidden-information views so each player only receives legal private data.
4. Add server-authoritative validation interfaces.
5. Add reconnection snapshot and command-resume design.
6. Add deterministic desync detection using state hashes.
7. Create adapter interfaces for Unity Netcode, Photon, Mirror, custom backend services, or platform services.
8. Build a local mock network provider for integration testing.

### Deliverables

- Networking abstraction interfaces.
- Player-specific state views.
- State hash/desync checks.
- Mock network adapter.

## Phase 12: Tutorial, Onboarding, and Sample Game

### Goals

Ship a complete sample CCG experience that demonstrates the toolkit end-to-end.

### Steps

1. Create a sample game theme with placeholder factions, card frames, art, heroes, and UI skin.
2. Build guided tutorial matches using scripted AI actions and highlighted prompts.
3. Add onboarding flows for first deck selection, collection introduction, deck editing, and first AI match.
4. Add sample campaigns or challenge encounters to demonstrate custom rule variations.
5. Document how to clone the sample game into a new project.
6. Add tests that load all sample scenes and validate all sample card assets.

### Deliverables

- Complete sample card set.
- Tutorial flow.
- Sample AI encounters.
- End-to-end example project documentation.

## Phase 13: Polish, Packaging, and Release

### Goals

Prepare the toolkit for reuse by other Unity projects and teams.

### Steps

1. Profile runtime allocations and optimize hot paths in rules, effects, presentation, and simulation.
2. Audit public APIs and mark internal APIs appropriately.
3. Add XML documentation or generated API docs for core extension points.
4. Create package samples for basic match, card authoring, deck builder, and AI simulation.
5. Prepare release notes, known limitations, upgrade guides, and versioning policy.
6. Add automated validation for package import, sample import, and scene loading.
7. Run usability passes with designers creating new cards from scratch.
8. Tag the first preview release and define the post-preview backlog.

### Deliverables

- Packaged Unity toolkit.
- Sample imports.
- API documentation.
- Release notes and upgrade guide.

## Cross-Cutting Engineering Requirements

- The rules engine must remain deterministic and testable outside of Unity scenes.
- Match state, commands, events, and card definitions should be serializable where practical.
- All random outcomes must use an injected deterministic RNG source.
- Core systems should have edit-mode tests before being wired into presentation scenes.
- Unity presentation code should react to match events and snapshots rather than directly modifying rules state.
- Card content should be validated in editor tooling and in automated tests.
- Extension points should be documented as soon as they are introduced.
- Public APIs should avoid locking users into one backend, economy, input, or networking solution.

## Suggested Milestone Order

1. Data-only card definitions and validation.
2. Deterministic draw/play/end-turn loop in tests.
3. Simple match scene with hand, resources, and spell/minion play.
4. Combat and keywords.
5. Editor card database and deck validation.
6. Deck builder and collection browser.
7. AI opponent and headless simulations.
8. Replays and diagnostics.
9. Multiplayer-ready abstractions.
10. Complete sample game and package release.

## Definition of Done for Major Systems

A major system is complete only when it has:

- Runtime implementation.
- Editor or designer workflow if applicable.
- Sample content demonstrating the feature.
- Unit or play-mode tests covering normal and invalid cases.
- Documentation explaining how to use and extend it.
- Validation or diagnostics for common configuration mistakes.

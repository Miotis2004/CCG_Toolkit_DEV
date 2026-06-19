# Card Authoring

Phase 1 introduces the foundational card data model that designers use to create card assets.

## Runtime schema

- Runtime code lives under `Assets/CCGToolkit/Runtime` in the `CCGToolkit` namespace.
- Card assets are `CardDefinition` ScriptableObjects in the `CCGToolkit.Cards` namespace.
- Every card has a stable `CardIdentifier`; new assets generate a GUID-backed value during Unity validation when the field is empty.
- Core authoring fields include title, description, cost, card type, rarity, faction, set, target type, keywords, tags, art, frame, artist credit, flavor text, and optional stats.
- Optional stats are grouped in `CardStatBlock` and cover attack, health, durability, armor, and resource modifiers.

## Validation basics

Use `CardDefinition.Validate()` for single-card checks and `CardDefinitionValidator.ValidateCollection()` for collection-level checks.

Current validation covers:

- Missing card IDs.
- Missing titles.
- Negative costs.
- Missing art warnings.
- Minion/token health requirements.
- Weapon attack and durability requirements.
- Unusual spell stat combinations.
- Duplicate IDs and duplicate titles across a collection.

## Sample content

The Phase 1 sample pool lives in `Assets/CCGToolkit/Samples/SampleCards` and includes minions, spells, weapons, tokens, and generated-style prototype cards that exercise the shared schema.
Placeholder art and frame assets live in `Assets/CCGToolkit/Samples/ArtPlaceholders` as text-serialized ScriptableObjects so PR systems do not need to accept binary image files.

## Phase 2 card database workflow

Open **CCG Toolkit > Card Database** to use the designer-facing card browser. The window indexes every `CardDefinition` under `Assets`, supports text search, filters by type, rarity, faction, set, and keyword, and can open the selected asset in the Inspector.

Use the toolbar actions to:

- **Refresh** the asset index after adding, deleting, or moving card assets.
- **Validate** all indexed cards and print a severity-coded report to the Console.
- **Generate Missing/Duplicate IDs** for cards that have no ID or collide with another indexed card.
- **Export CSV** or **Export JSON** for spreadsheet review and external tooling.
- **Import CSV** or **Import JSON** to create new card assets in the sample card folder from external data.

The right-side preview shows the selected card's cost, type, rarity, rules text, stats, faction, set, keywords, and per-card validation messages. The bulk edit panel applies changes to the currently filtered result set, including set, rarity, faction, keyword, and simple balance deltas for cost, attack, and health.

Programmers can use `CardDatabaseService` for editor indexing and querying, `CardBulkEditUtility` for scripted bulk changes, and `CardImportExportUtility` for JSON/CSV export plus JSON import into a destination asset folder.

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

# Validation

Use these commands from the repository root to validate Phase 0 and later work in CI-friendly environments.

## Unity batch-mode tests

```bash
Unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults TestResults/EditMode.xml -quit
Unity -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults TestResults/PlayMode.xml -quit
```

## Project conventions checklist

- Runtime APIs compile in `CCGToolkit.Runtime`.
- Editor utilities compile in `CCGToolkit.Editor` and must not be referenced by runtime code.
- Edit-mode and play-mode tests remain in separate assemblies.
- New Unity assets should include their `.meta` files in source control.
- Generated build, library, log, and user-specific files should stay out of source control.

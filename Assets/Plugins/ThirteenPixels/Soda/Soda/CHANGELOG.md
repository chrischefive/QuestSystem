## [1.2.7] - 2021-01-15
- Re-enable inspector subtitles with meta information, rendered differently in Unity 2019.1 and newer

## [1.2.6] - 2020-11-27
- Added GlobalGameObjectWithComponentCache.onChangeComponentCache
- Properly display SodaEvent responses coming from ScopedVariables as proxies
- Use nameof keyword for DisplayInsteadInPlaymodeAttribute

## [1.2.5] - 2020-02-17
- Added GlobalVaiablePropertyDrawer
- Added unit tests
- Improved editor GUI for new Unity skin

## [1.2.4] - 2020-01-28
- Added parameterized GameEvent base class
- Fixed and improved GlobalGameObjectWithComponentCache

## [1.2.3] - 2020-01-07
- Improved SodaEvents to handle removal of responses by other responses

## [1.2.2] - 2019-12-02
- Prevent cyclic/recursive invocation in SodaEvents
- Let ScriptableObject editors display additional serialized fields

## [1.2.1] - 2019-11-08
- Redesigned icons
- Added changelog

## [1.2.0] - 2019-10-26
- Removed the need to pass a reference to the listener to SodaEvent.AddResponse for debugging
- Added uint support to DisplayInsteadInPlaymodeAttribute

## [1.1.0] -
- Enabled editor classes to work for subclasses of their targets, removing the need for editor subclasses
- Changed class creation templates to not include editor classes anymore
- Fixed CreateAssetMenu menuNames for RuntimeSets (including template)
- Added default implementations for GlobalGameObjectWithComponentCacheBase.TryCreateComponentCache and RuntimeSetBase.TryCreateElement
- Added package.json for package manager support

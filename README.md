# Steel Horizon — Unity 3D RTS Starter (Android-first)

This is a real Unity project scaffold rebuilt from the supplied Java/OpenGL SteelHorizon ZIP. It is a playable **prototype foundation**, not a finished Broken Arrow-level game.

## Included in this package
- Unity 2022.3 LTS project layout and Android application ID `com.steelhorizon.rts`.
- C# runtime battlefield generation: terrain, roads, blockout city buildings, trees and two objectives.
- Stylized placeholder tank, infantry, artillery and rocket units.
- Local AI skirmish modes selectable as 1v1, 2v2, 3v3 and 4v4 (team size means AI squad count; this is not online multiplayer).
- Basic AI movement, nearest-enemy targeting, damage and unit destruction.
- Touch selection and movement orders; mouse controls are available in the Unity Editor.
- Eight WAV effects copied from the supplied project. They are generated/prototype effects, not verified authentic recordings.
- Editor helper automatically creates the initial scene and enables it in Build Settings after Unity imports the project.

## Important limitations (honest status)
- No photorealistic vehicle meshes, animated soldiers, detailed terrain materials, real ballistic simulation, destructible city, strategic pathfinding, multiplayer networking, or campaign yet.
- Units are primitive blockout models so the project can be opened without downloading paid assets.
- The Android APK has **not** been built or tested here. This environment does not include Unity Editor or Android SDK/NDK, so no APK is included.
- The project is intended for Unity Editor 2022.3 LTS or a compatible newer LTS. Unity Editor does not run natively on Android phones. Phone-only development requires a cloud/remote computer with Unity installed and an appropriately licensed Unity build environment.

## Open and test
1. Download and extract this ZIP.
2. Open the `SteelHorizonUnity` folder as a Unity project in Unity Hub (Unity 2022.3 LTS recommended).
3. Wait for import/compile. The Editor helper creates `Assets/Scenes/SteelHorizon.unity` automatically.
4. Open that scene and press **Play**. Choose 1v1–4v4 at the top. Tap/click a blue unit, then tap/click empty terrain to move it. Units engage nearby enemies automatically.
5. For Android: install Android Build Support (SDK/NDK + OpenJDK) in Unity Hub, switch Build Settings to Android, set package ID if desired, then Build. Test the APK on a real phone.

## Phone-only route
Upload this ZIP to a GitHub repository from the phone if convenient, but GitHub Actions cannot build Unity Android projects without a Unity installation and valid licensing/activation credentials. Do not put Unity credentials or license files in a public repository. If you already have a trusted private cloud machine/CI runner with Unity licensed, use it for the Android build.

## Next development milestones
1. Replace blockout units with licensed detailed 3D assets and proper animations.
2. Implement touch drag-pan, pinch zoom, unit groups, formation movement and collision-aware pathfinding.
3. Add projectile travel, line of sight, armor types, reloads, ammo and damage feedback.
4. Add map objectives, reinforcements, aircraft behavior and smarter team AI.
5. Add multiplayer only after the single-player simulation is stable.

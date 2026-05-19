# Idle Tap Game — Unity temelj

Samostojen Unity (C#) projekt: mobilna **idle/clicker** "tap" igra za Android +
iOS. To je **temelj (scaffold)**, ne dokončana igra — delujoča igralna zanka
in arhitektura, pripravljena za grafiko v slogu Clash of Clans.

Projekt je neodvisen od FiveM launcherja v korenu tega repozitorija.

## Kaj je vključeno

- Tap dohodek z nadgradljivo tap močjo
- Pasivni generatorji (cena raste geometrijsko)
- Formatiranje velikih števil (1.5K, 2.30M, 1.00B … aa, ab)
- Shranjevanje (JSON v `persistentDataPath`) + autosave + lifecycle
- Offline napredek z "Dobrodošel nazaj" popupom (kapica 8 h)
- Celoten UI zgrajen iz kode (brez prefabov / asset GUID-ov) → projekt se po
  odprtju "samo zažene"
- Safe-area podpora za zaslone z zarezo

## Zahteve

- **Unity 6 LTS** (`6000.0.x`) prek Unity Hub. `ProjectSettings/ProjectVersion.txt`
  predlaga konkreten patch; če imaš drug 6000.0 patch, ga Unity Hub odpre brez
  težav.
- Moduli ob namestitvi: **Android Build Support** (SDK/NDK/JDK),
  za iOS še **iOS Build Support** (build iOS zahteva macOS + Xcode).

## Zagon (lokalno, v Unity)

1. Unity Hub → Add → izberi mapo `idle-tap-game/`, odpri z Unity 6 LTS.
2. Počakaj, da Package Manager razreši pakete iz `Packages/manifest.json`.
3. Meni **IdleTapGame → Create Main Scene** (ustvari `Assets/Scenes/Main.unity`
   in jo nastavi kot build sceno).
   - _Fallback:_ nova prazna scena → dodaj prazen GameObject → komponenta
     `GameBootstrap`.
4. Odpri `Assets/Scenes/Main.unity` in pritisni **Play**:
   - tap gumb veča zlato; kupi generator → raste dohodek/s;
   - ustavi in znova zaženi Play → stanje ostane;
   - po simuliranem premoru se pokaže "Dobrodošel nazaj".

> Opomba: ker projekt ne vklju­čuje Input System paketa, naj v
> **Edit → Project Settings → Player → Active Input Handling** ostane
> _Input Manager (Old)_ ali _Both_ (privzeto je v redu).

## Nastavitve za mobilni build (Player Settings)

- **Company / Product Name** in **Bundle Identifier**
  (npr. `com.tvojeime.idletap`)
- **Default Orientation: Portrait**
- Android: **Minimum API Level 23+**, **Scripting Backend: IL2CPP**,
  **Target Architectures: ARM64**
- iOS: nastavi Apple Team / signing v Xcode po exportu

## Build

- **Android:** File → Build Settings → Android → Switch Platform → Build
  (APK ali AAB za Play Store).
- **iOS:** File → Build Settings → iOS → Build (na macOS) → odpri nastali
  Xcode projekt → Run/Archive.

## Arhitektura (`Assets/Scripts/`)

| Datoteka | Vloga |
|---|---|
| `Core/GameManager.cs` | singleton; tick pasivnega dohodka, save, offline |
| `Core/GameBootstrap.cs` | zgradi celoten UI in poveže komponente |
| `Economy/CurrencyManager.cs` | stanje valute + eventi |
| `Economy/Generator.cs` | model generatorja (cena/proizvodnja/owned) |
| `Economy/GeneratorCatalog.cs` | privzeta MVP vsebina |
| `Gameplay/TapController.cs` | tap moč + nadgradnja |
| `Save/SaveSystem.cs` / `SaveData.cs` | JSON persistenca |
| `Save/OfflineProgress.cs` | izračun offline zaslužka |
| `Util/BigNumberFormatter.cs` | formatiranje velikih števil |
| `UI/HudController.cs` | veže HUD na GameManager, refresh |
| `UI/GeneratorRowUI.cs` | vrstica generatorja |
| `UI/SafeAreaFitter.cs` | safe-area za zarezne zaslone |
| `Editor/SceneBootstrapper.cs` | meni za izdelavo scene |

## Grafika v slogu Clash of Clans (naslednji korak)

CoC-stil je v prvi vrsti **umetnostni** zalogaj; ta temelj ga podpira:

- Skeletalne animacije: **Spine 2D** (uradni Unity runtime) ali DragonBones —
  za enote/zgradbe z mehkimi animacijami.
- Dodaj prek **Window → Package Manager**: `2D Sprite`, `2D Animation`
  (za sprite-e/skeletal), po želji `Mobile Notifications` (idle "vrni se"
  obvestila). `manifest.json` je namerno minimalen za zanesljiv prvi import.
- Uporabi **Sprite Atlas** za batch/performance.
- Izometrična/2.5D postavitev, particle efekti (Particle System), DOTween
  (Asset Store) za "juicy" animacije gumbov/nagrad.
- Canvas Scaler je že nastavljen (referenca 1080×1920); placeholder UI le
  zamenjaj s sprite-i.

## Verifikacija

Verifikacija (Play, Android/iOS build) se izvede lokalno v Unity Hub —
v oblačnem okolju brez Unity Editorja prevedbe/builda ni mogoče izvesti.
Koda je samostojno-konsistentna in brez asset GUID referenc, da prevedba ob
prvem odprtju uspe.

## Izven obsega (zavestno)

Dejanske CoC umetnine/animacije, monetizacija (oglasi/IAP),
backend/multiplayer, prestige/meta sistemi — kasnejše iteracije;
arhitektura je zasnovana razširljivo.

# エディタ拡張（Editor Scripting）一覧と使い方

本プロジェクトは「エディタ拡張×AI自動化」を主軸にしています。ここでは、現在の拡張メニューと目的、実行手順、出力結果をまとめます。

---

## メニューの場所
- Unity メニュー: `Tools > Project Setup`

## 現在の拡張

### Apply Standard Settings
- パス: `Assets/Editor/ProjectSetup/ApplyStandardSettings.cs`
- メニュー: `Tools > Project Setup > Apply Standard Settings`
- 目的: プロジェクト標準の設定をワンクリックで適用（冪等）
- 処理:
  - Tags/Layers 追加（`Enemy`, `Projectile`, `Pickup`, `Boss` / `Player`, `Enemy`, `Projectile`, `Pickup`, `Environment`）
  - Physics2D: `Queries Hit Triggers` 有効、Layer Collision の基本設定
  - Time/Quality: `fixedDeltaTime=0.02`, `vSyncCount=0`
  - URP: `UniversalRP.asset` を複製して PC/Mobile プロファイルを準備（PC適用）

### Prepare Current Scene
- メニュー: `Tools > Project Setup > Prepare Current Scene`
- 目的: 現在のシーンを起動用に整備
- 処理:
  - `Bootstrap` GameObject 追加（60fps固定）
  - `EventSystem` 生成、`StandaloneInputModule` → `InputSystemUIInputModule` へ切替
  - 開いているシーンを Build Settings に登録

### Create > Player With Camera
- メニュー: `Tools > Project Setup > Create > Player With Camera`
- 目的: プレイヤー雛形を設置してカメラを追従設定
- 処理:
  - `Player` 生成（`Rigidbody2D`/`PlayerController2D`）
  - `SpriteRenderer` を自動付与（内蔵スプライト相当を割当）
  - `PlayerInput` を `Assets/InputSystem_Actions.inputactions` に接続、`Player`マップをSendMessagesで通知
  - `Main Camera` に `CameraFollow2D` を付与し、ターゲットを `Player` に設定

### Create > Background Grid
- メニュー: `Tools > Project Setup > Create > Background Grid`
- 目的: 視認性のための簡易チェック柄背景を生成
- 処理:
  - `Background` を作成し、`ProceduralGridBackground` でタイル背景を生成

### Create > Enemy Spawner
- メニュー: `Tools > Project Setup > Create > Enemy Spawner`
- 目的: 周囲に敵を出現させてプレイヤーを追尾させる
- 処理:
  - `EnemySpawner` を設置し、プレイヤーを追尾する `EnemyController2D` を一定間隔でスポーン

---

## スクリプト一覧（主要）
- `Assets/Editor/ProjectSetup/ApplyStandardSettings.cs`: すべてのメニュー定義
- `Assets/Scripts/Core/Bootstrap/Bootstrap.cs`: 起動時に `Application.targetFrameRate` を設定
- `Assets/Scripts/Core/Camera/CameraFollow2D.cs`: カメラ追従
- `Assets/Scripts/Core/Visual/ProceduralGridBackground.cs`: 背景生成
- `Assets/Scripts/Player/PlayerController2D.cs`: プレイヤー移動（Input System）
- `Assets/Scripts/Enemies/EnemyController2D.cs`: 敵追尾
- `Assets/Scripts/Enemies/EnemySpawner.cs`: スポーン制御

---

## よくある注意
- Playモード中は多くのメニューが実行不可（安全のため）。停止してから再実行してください。
- 何度実行しても壊れない（冪等）ように実装しています。おかしくなったら再実行で整います。
- 設定値は `Docs/Settings_Snapshots.md` にも記録し、変更理由を短く残します。

---

## 今後の拡張候補
- URPプロファイルのPC/モバイル切替メニュー
- シーン自動作成（検証用/チュートリアル）
- アセット取り込みテンプレ（アイコン/SFXの一括登録）
- ビルド/パッケージングのワンクリック化

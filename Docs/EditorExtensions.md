# エディタ拡張（現状）

本プロジェクトのエディタ操作は最小構成です。セットアップは1メニューのみで完結します。

---

## メニュー
- Unity メニュー: `Tools > Shooter`

### Setup (Reset & Apply Assets)
- 目的: 1クリックでプロジェクトを縦スクロールSTG用に初期化
- 処理（冪等）:
  - 新規シーン作成 → `Stage_VerticalShoot.unity` として保存・起動設定に登録
  - カメラ: `orthographic=true`, `orthographicSize=6` + `AspectEnforcerPortrait(9:16)` 付与
  - 背景: `ProceduralGridBackground` を配置し、`Space Shooter Template FREE` の `Backgrounds` があれば子に配置
  - システム: `AutoScrollSystem`
  - プレイヤー: `PlayerController2D`（shooterMode=ON）、`PlayerInput`（Actions=`Assets/InputSystem_Actions.inputactions`, Map=`Player`, Behavior=SendMessages）、機体/弾Prefabがあれば自動配線
  - スポーン: `FormationSpawner`（spawnYOffset=14）
  - デバッグ: `ConsoleLogRecorder`（`Logs/latest.log`を生成）、`RuntimeDebugOverlay`（オーバレイはフラグで制御）

### Open Shooter Scene（補助）
- 目的: `Stage_VerticalShoot.unity` を開くショートカット
- 備考: 不要なら削除可能（機能上の依存は無し）

---

## ログ/デバッグ
- `Logs/latest.log`: Play開始ごとに作り直し、`Debug.Log/Warning/Error` を追記
- オーバレイ: `RuntimeDebugOverlay`（カメラ位置/HP/速度など）。表示は `Game.Core.Debugging.DebugFlags.Overlay` の値で切替（既定OFF）

---

## 主要スクリプト
- `Assets/Editor/ProjectSetup/ShooterSetup.cs`: セットアップの実体（唯一の運用メニュー）
- `Assets/Scripts/Core/Camera/AspectEnforcerPortrait.cs`: 9:16 アスペクト強制
- `Assets/Scripts/Core/Visual/ProceduralGridBackground.cs`: 背景生成
- `Assets/Scripts/Shooter/*`: スクロール/スポーン/背景ユーティリティ
- `Assets/Scripts/Player/PlayerController2D.cs`: 入力・移動・射撃（InputActionsイベント＋ポーリングの両対応）
- `Assets/Scripts/Core/Debug/ConsoleLogRecorder.cs`: コンソールログの自動書き出し


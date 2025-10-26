# エディタ拡張（シンプル版）

本プロジェクトのエディタ操作は最小構成です。セットアップは1メニューで完結します。

---

## メニュー
- Unity メニュー: `Tools > Shooter`

### Setup (Space Shooter Preset)
- 目的: 1クリックでSpace Shooter Template FREEのデモシーンを開く
- 処理:
  - `Assets/Space Shooter Template FREE/Scenes/Demo_Scene.unity` を開く
  - スタートシーンに設定（ビルド設定にも登録）
  - Main Camera に `AspectEnforcerPortrait` を追加（9:16アスペクト比強制）
  - `ConsoleLogRecorder` を初期化（ログ出力）
  - セットアップサマリを `Logs/setup_summary.txt` に出力

### Open Demo Scene（補助）
- 目的: デモシーンを開くショートカット
- Space Shooterのデモシーンを直接開きます

### Open Player Settings（補助）
- 目的: プロジェクト設定（Player）を開く
- Input Handling の確認・変更に使用

### Setup Sound Effects
- 目的: 効果音を自動設定
- 処理:
  - プレイヤー死亡音を設定（DM-CGS-09.wav）
  - 敵撃破音を爆発VFXに設定（DM-CGS-48.wav）
  - パワーアップ取得音を設定（DM-CGS-07.wav）
  - シーン内の全オブジェクトとプレハブに適用
- 前提: `Casual Game Sounds U6` がインポート済み

---

## ログ/デバッグ
- `Logs/latest.log`: Play開始ごとに作り直し、`Debug.Log/Warning/Error` を追記
- `Logs/setup_summary.txt`: セットアップ実行時のサマリ

---

## 主要スクリプト

### エディタ拡張
- `Assets/Editor/ProjectSetup/ShooterSetup.cs`: セットアップの実体（全メニュー）

### ランタイムスクリプト
- `Assets/Scripts/Core/Camera/AspectEnforcerPortrait.cs`: 9:16 アスペクト強制
- `Assets/Scripts/Core/Debug/ConsoleLogRecorder.cs`: コンソールログの自動書き出し
- `Assets/Scripts/Core/Audio/*.cs`: 音声システム（自動設定）
  - `SoundEffectPlayer.cs`: 汎用サウンド再生
  - `PlayerSoundBridge.cs`: プレイヤー音声
  - `BonusSoundBridge.cs`: パワーアップ音声
  - `AudioCleanupHelper.cs`: シーン終了時の音声クリーンアップ

---

## カスタム実装の削除について
以前のバージョンでは、以下のカスタム実装がありましたが、**全て削除されました**：
- Player/（PlayerController2D、PlayerMover、PlayerShooter等）
- Combat/（DamageSource、Damageable、Health等）
- Enemies/（EnemyController2D、EnemySpawner等）
- Systems/（経験値システム、レベルアップ等）
- UI/（カスタムHUD等）
- Shooter/（AutoScrollSystem、FormationSpawner等）
- Art/（VfxRegistry、SpriteSet等）
- Preset/（カスタム入力スクリプト等）

現在は**Space Shooter Template FREEのプリセットをそのまま使用**しています。

---

## 変更履歴
- 2025-10-26: カスタム実装を全削除し、Space Shooterプリセット版に移行
- Input Handling を "Both" に設定（旧Input APIとInput System両方を有効化）
- ログ機能とアスペクト比強制のみを残す
- 音声システムを追加（Setup Sound Effectsメニュー、自動設定）

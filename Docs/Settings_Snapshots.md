# 設定スナップショット（プリセット版）

この文書はSpace Shooter Template FREEのプリセットを使用するための設定メモです。

---

## 重要: Input Handling

### Active Input Handling
- 参照: `ProjectSettings/ProjectSettings.asset`
- 設定値: **Both** (activeInputHandler: 2)
- 理由: Space Shooter Template FREEは旧Input API (UnityEngine.Input) を使用
- 変更方法: `Edit > Project Settings > Player > Other Settings > Active Input Handling`

**この設定が "Input System Package (New)" になっているとエラーが出ます！**

```
InvalidOperationException: You are trying to read Input using the UnityEngine.Input class,
but you have switched active Input handling to Input System package in Player Settings.
```

### 設定値
- 0 = Both（旧Input APIとInput System両方）← **推奨**
- 1 = Input Manager (Old)（旧Input APIのみ）
- 2 = Input System Package (New)（新Input Systemのみ）← **Space Shooterで使えない**

---

## URP 設定
- 参照: `Assets/Settings/UniversalRP.asset`
- Space Shooter Template FREEのデフォルト設定をそのまま使用

---

## Time
- 参照: `ProjectSettings/TimeManager.asset`
- デフォルト設定:
  - `fixedDeltaTime = 0.02` (50 FPS)
  - `maximumDeltaTime = 0.33`
  - `timeScale = 1`
  - `targetFrameRate = -1` (VSync)

---

## Physics2D
- 参照: `ProjectSettings/Physics2DSettings.asset`
- Space Shooter Template FREEのデフォルト設定をそのまま使用
- Gravity: (0, -9.81)
- Layer Collision Matrix: Space Shooterの設定

---

## Tags & Layers
- 参照: `ProjectSettings/TagManager.asset`
- Space Shooter Template FREEのデフォルトタグ・レイヤーをそのまま使用

---

## カスタム設定（追加分）

### AspectEnforcerPortrait (オプション)
- 目的: 9:16のアスペクト比を強制（縦長画面）
- 場所: Main Camera
- 使用: Setup実行時に自動追加
- 削除: 不要な場合は手動で削除可能

### ConsoleLogRecorder
- 目的: プレイ時のログを `Logs/latest.log` に出力
- 場所: 静的初期化（自動起動）
- 使用: Setup実行時に初期化
- 無効化: スクリプトを削除

---

## 変更時のルール
- プリセットの設定はできるだけ変更しない
- 変更した場合はここに記録する
- Input Handlingは **"Both"** を維持する

---

## 変更履歴
- 2025-10-26: プリセット版に移行
- Input Handling を "Both" に変更
- カスタム設定を最小限に

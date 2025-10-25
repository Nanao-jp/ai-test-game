# 設定スナップショット（PC/モバイル） v0.1

この文書はUnityエディタで投入した設定の“最終値”メモです。意図が変わらない限り、ここに記載の値を再現してください。

---

## URP 品質プリセット
- 参照: `Assets/Settings/UniversalRP.asset`, `Assets/UniversalRenderPipelineGlobalSettings.asset`

PC（Target 60fps）
- Shadow: On（Distance: 25, Resolution: 2048）
- Post-processing: Minimal（Bloom Off, Vignette Off, ColorAdjust On）
- Anti-aliasing: FXAA
- Render Scale: 1.0

Mobile（Target 60fps）
- Shadow: Off（または Distance: 10, Resolution: 1024）
- Post-processing: Off
- Anti-aliasing: FXAA or None
- Render Scale: 0.8〜1.0（端末により 0.9 推奨）

## Time
- 参照: `ProjectSettings/TimeManager.asset`
- `fixedDeltaTime = 0.02`
- `maximumDeltaTime = 0.33`
- `timeScale = 1`
- `targetFrameRate = 60`

## Physics2D
- 参照: `ProjectSettings/Physics2DSettings.asset`
- Gravity: (0, -9.81) → サバイバーズ系は重力未使用想定
- Queries Hit Triggers: On（弾のTrigger判定を拾う）
- Layer Collision Matrix: `Player x Enemy` / `Projectile x Enemy` 有効、`Projectile x Player` 無効
- Queries Hit Triggers: On（`Damageable`でTrigger接触を拾う）

## Tags & Layers
- 参照: `ProjectSettings/TagManager.asset`
- Tags: `Enemy`, `Projectile`, `Pickup`, `Boss`
- Layers: `Player`, `Enemy`, `PlayerProjectile`, `Pickup`, `Environment`

## Input System（Control Schemes）
- 参照: `Assets/InputSystem_Actions.inputactions`
- Schemes: `Keyboard&Mouse`, `Gamepad`, `Touch`
- 備考: Touchは仮想スティック（左移動）/攻撃は常時オート

---

変更時のルール
- 値を変えたらここに反映し、理由を1行で残す
- モバイル実機で60fps未満が継続する場合、まずはURP Mobile側のShadow/Post/Render Scaleを見直す

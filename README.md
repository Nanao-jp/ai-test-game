## ai-test-game02 — 縦スクロールシューティング（Shooter Template）

このリポジトリは「縦スクロールSTG」のテンプレートです。エディタ拡張は最小構成で、1クリックのセットアップと、再生ごとのログ収集を備えています。前バージョン（サバイバー系）の情報は削除し、現構成のみ記載しています。

### 目的（ゴール）
- 1クリックで縦スクロールSTGの検証シーンを生成・起動
- Space Shooterアセットに自動対応（見た目/弾/爆発の自動配線）
- 9:16の縦長表示を強制し、ログを毎回 `Logs/latest.log` に出力

### コア体験（最小仕様）
- 縦スクロール（自動スクロール）
- プレイヤーは移動操作、射撃はオート（固定上方向）
- シンプルな敵スポーン（画面上端の外から流入）

### スコープ（初期スプリント）
- ステージ: 1種（固定タイル背景）
- 敵: 4種（歩行/突進/範囲/タンク）
- 武器: 6種（近接/飛び道具/範囲/設置/追尾/バフ）
- パッシブ: 6種（攻撃力/攻速/移動/回復/磁力/クールダウン）
- UI: タイトル/ゲーム/レベルアップ/結果/ポーズ

---

## 開発環境
- Unity: 6000.2.9f1
- レンダリング: Universal Render Pipeline (URP) 17.2.0
- 入力: Input System 1.14.2（`Assets/InputSystem_Actions.inputactions`）
- 2D関連: 2D Animation/Tilemap/SpriteShape 等
- AI関連（制作支援目的・ランタイム非必須）:
  - com.unity.ai.assistant: 1.0.0-pre.12
  - com.unity.ai.generators: 1.0.0-pre.20
  - com.unity.ai.inference: 2.3.0

推奨エディタ: Rider 2024 以降 / Visual Studio 2022 以降

---

## ディレクトリ方針（要点）
```
Assets/
  Scripts/
    Core/           // ゲームループ、時間、イベントバス
    Player/         // 入力、移動、装備スロット
    Combat/         // 武器、弾、ダメージ、当たり判定
    Enemies/        // 敵AI（単純なステアリング）、スポーナー
    Systems/        // 経験値、レベルアップ、ドロップ、メタ
    UI/             // HUD、レベルアップ、結果画面
    Data/           // ScriptableObject の定義
  Art/
    Sprites/
    VFX/
  Audio/
    BGM/
    SFX/
  Addressables/     // 後日導入予定
  Settings/
```

命名規約（例）
- クラス: PascalCase（例: `EnemySpawner`）
- フィールド: `_camelCase`（SerializeFieldは先頭に`_`）
- ScriptableObject: `So`接尾（例: `WeaponSo`）
- プレハブ: `Pfb_`接頭、スプライト: `Spr_`接頭、VFX: `Vfx_`接頭

---

## 画面/入力/ログ
- 表示: 9:16 縦長を `AspectEnforcerPortrait` で強制（ピラーボックス）
- 入力: `Input System`（`Player` マップ）。イベント（OnMove）とポーリング双方に対応
- ログ: `Logs/latest.log` に毎回書き出し（`ConsoleLogRecorder`）

---

## AI活用ポリシー（制作フロー）
AIは“提案→出力→検証→採用”のループで使う。ゲーム内ランタイムAIは不要。

### コード
- 雛形生成: MonoBehaviour/ScriptableObject/Editor拡張のテンプレをAIに生成させる
- リファクタ提案: クラス分割/責務分離/命名/コメント最小主義の観点でレビュー
- 自動ドキュメント: 依存グラフ/UML/メッセージフローをAIで図化

### アート/UI
- スタイル固定プロンプトを策定し、武器/敵アイコン、UIアイコンを一括生成
- 9スライスUIフレーム/ボタン/パネル素材のバリエーション出力→Unityで配色差分
- スプライトはアトラス化、レイアウトはUI Toolkit/UGUIのどちらでも可

### サウンド
- SFXはテキストプロンプトから複数案生成→波形正規化/ループ調整をAI支援
- BGMはテンポ/BPM指定で数曲生成し、難易度帯に割当て

### ローカライズ/文言
- 日本語→英語の初稿をAIで生成、UI制約（文字数/改行）を指示
- ストア説明、チュートリアル、クレジット文面もドラフトをAIで作成

### バランス/分析
- 実プレイログ（csv）をAIに渡し、武器DPS/被弾率/クリア率から調整提案を受ける
- 提案はPRに添付し、変更理由を短文化して記録

---

## ワークフロー（エディタ）
1. `Tools > Shooter > Setup (Reset & Apply Assets)` を実行
2. `Play` で動作確認（操作不可時は `Logs/latest.log` を確認）
3. 見た目を変えたい場合は Space Shooter のプレハブを追加して再度 Setup を実行

コミット規約（例）
- `feat: 武器「ブーメラン」実装`
- `balance: 近接系の基礎DPSを+12%`
- `ui: レベルアップ画面の候補UI更新`

---

## 主要コンポーネント
- `ShooterSetup`（唯一のセットアップメニュー）
- `AutoScrollSystem`（縦スクロール）
- `FormationSpawner`（上端スポーン）
- `PlayerController2D`（移動/射撃、InputActions＋ポーリング）
- `AspectEnforcerPortrait`（9:16固定）
- `ConsoleLogRecorder`（ログ収集）

---

## 実行
- 既定シーン: `Assets/Scenes/Stage_VerticalShoot.unity`
- 表示: 9:16縦長（ピラーボックス）
- 入力: WASD/矢印/ゲームパッド左スティック。射撃は自動（上方向）

### Quickstart（見た目の適用）
1. `Tools > Shooter > Setup (Reset & Apply Assets)` を実行
2. Space Shooter の `Player.prefab` / `Player_Short_Lazer.prefab` / `Backgrounds.prefab` があれば自動適用
3. Play

### 現状（2025-10-26）
- 9:16縦長、縦スクロール、上方向オート射撃
- Space Shooterアセットの機体/弾/背景に自動対応
- Playごとに `Logs/latest.log` を生成

---

## モバイル対応（PCテスト前提）
- 結論: 本プロジェクトはiOS/Androidビルドに対応可能。日常のプレイテストはPCで実施し、必要時にモバイル用ビルドを生成します。

### 入力切替
- Input Systemの`Control Scheme`で `Keyboard&Mouse` / `Gamepad` / `Touch` を用意
- タッチ操作は仮想スティック（左移動）+ オート攻撃維持（追加ボタン不要）

### 解像度/アスペクト
- 9:16（1080x1920）基準で安全域を設定。UGUIの`Canvas Scaler`を`Scale With Screen Size`に設定
- 16:9向けはPCでUI配置、モバイルでは縦長に最適化したHUDレイアウトを別Canvasで差し替え

### パフォーマンス
- URPのクオリティ設定を`Mobile`（影オフ/ポスト最小）に分岐
- スプライトアトラス化、ドローコールを抑制、Updateの分割（固定更新/可変更新）

### ビルド設定メモ
- Android: `Project Settings > Player > Other Settings` → `IL2CPP`、`ARM64`、`Target API Level`は最新推奨
- iOS: `IL2CPP`、`Scripting Backend`一貫。Xcodeから署名/ビルド

### 操作方針の差分
- PC: WASD/左スティック移動、マウス不要
- モバイル: 仮想スティック/スワイプ移動、攻撃は自動（ボタン負荷を避ける）

### QA
- PCでコアループ/バランス/密度を検証→週1で実機ビルド（入力/UI視認性/発熱）を確認

## 品質ゲート（目安）
- 60 FPS維持（ターゲットPCで）
- 可読性基準: メソッド < 60行、早期return、例外の乱用禁止
- プレイ計測: 5本の10分ランで致命的不具合なし
- UI可読性: 100%スケールで文字/色コントラスト達成

---

## ライセンス/クレジット
- AI生成アセットは商用利用可のモデル/設定のみ採用（出典管理）
- 外部アセット使用時は`Credits.md`にライセンス表記を追記

---

## トラブルシュート（よくある）
- 操作できない: `Logs/latest.log` の先頭に `[PlayerInput] present=True ...` が無い → PlayerInputが無効。`Setup` を再実行
- 横長に戻る: `AspectEnforcerPortrait` が外れている → `Setup` を再実行
- HPが減り続ける: スポーンが近すぎる → `FormationSpawner.spawnYOffset` を上げる（14〜16）

---

## ドキュメント
- 制作チェックリスト（目的と意味）: `Docs/Development_Checklist.md`
- 設定スナップショット: `Docs/Settings_Snapshots.md`
 - エディタ拡張一覧: `Docs/EditorExtensions.md`

---

## 開発スタイル（重要）: エディタ拡張 × AI自動化
- **方針**: “エディタ拡張（Editor Scripting）による自動化”を開発の主軸にし、AIで雛形/差分/ドキュメントを量産→ツール化→再現性確保。
- **一般用語**: Tooling / Automation / Scaffolding / Configuration as Code
- **メリット**:
  - 冪等なセットアップ（ワンクリックで同じ状態を再現）
  - 属人性の低減（設定や手順をコード化）
  - 横展開の高速化（新メンバー/新PCですぐ同じ開発状態）
- **適用範囲**: プロジェクト設定、シーン初期化、プレハブ雛形、レベル生成、アセット導入、ビルド設定、ドキュメント出力
- 使い方や現在の実装は `Docs/EditorExtensions.md` を参照

---

## 実装目標（短期）
- 入力の確実化: `PlayerInput` の自動付与・`defaultActionMap=Player` 強制・Action購読＋ポーリングの二重化で「常に動く」状態にする
- HPバー追従の安定化: 機体Prefab（`Visual`）のバウンズ上端に自動追従（ズレ最小）
- 敵/爆発の見た目統一: Space Shooterの `Enemy Explosion` を標準爆発として全敵に付与
- 背景の拡張: パララックス層（惑星/星屑レイヤ）追加の雛形
- 弾のプール: `Player` 弾を簡易プーリングへ置換（GC削減）
- HUD最小版: スコア/残機/ボムのラベル表示

## 既知の問題（2025-10-26）
- 操作不可（入力0）
  - 事象: `Logs/latest.log` 先頭に `[PlayerInput] present=False` が出る／`[Action] Move=...` が出ない
  - 現状: Setupで `PlayerInput` 付与とMap固定を行うが、再現個体で未付与のケースあり
  - 暫定: `PlayerController2D` がAction購読＋WASD/矢印/ゲームパッドのポーリングを実施（ログに `[OnMove]`/`[Action]`/`[PollInput]` を出力）
  - 次対応: Setup時に`PlayerInput`の存在を検証→なければ強制追加、実行後に`Logs/latest.log`へ確定行を記録

- HPが自然減する
  - 事象: 画面に敵が居ない状態でもHPが減少
  - 可能性: 画面外スポーンとの接触/古いコライダの残骸/レイヤ誤設定
  - 暫定: 敵は `DamageSource(DamageLayers=Player)`、弾は `Enemy` のみ。`spawnYOffset` を 14 に設定
  - 次対応: 一時的に接触ダメージをオフにできるデバッグフラグを追加し、原因層を切り分け

- 表示が横長に戻ることがある
  - 対応済: `AspectEnforcerPortrait` で9:16を強制（ピラーボックス）。Setupが自動付与

## ログの見方（抜粋）
- 起動直後の1行: `[PlayerInput] present=... actions=... map=... behavior=...`
  - present=True, map=Player であることを確認
- 入力検知: `[OnMove] value=...` / `[Action] Move=...` / `[PollInput] used value=...`
- ファイル: プロジェクト直下 `Logs/latest.log`（Playごとにリセット）



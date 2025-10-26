# AI駆動ゲーム開発ガイド

このドキュメントは**「ゲーム開発初心者 + 高品質アセット + AI支援」による開発手法**の完全ガイドです。  
本プロジェクト（ai-test-game02）だけでなく、今後のゲーム開発プロジェクト全般で参照できます。

---

## 📘 目次

1. [開発哲学](#開発哲学)
2. [開発環境の理想像](#開発環境の理想像)
3. [推奨アセット一覧](#推奨アセット一覧)
4. [AI活用戦略](#ai活用戦略)
5. [開発フェーズ戦略](#開発フェーズ戦略)
6. [実装パターン集](#実装パターン集)

---

## 🎯 開発哲学

### コアコンセプト

**「開発者は最低限しかUnityに触れず、ゲームデザインとテストプレイに集中する」**

### 3つの柱

#### 1. アセットファースト
- 車輪の再発明をしない
- 実績のあるアセットに投資し、開発時間を短縮
- カスタマイズは最小限に

#### 2. AI駆動開発
- コード生成はAIに任せる
- 素材生成もAIで
- デバッグ支援もAI
- 開発者は設計と判断に集中

#### 3. データ駆動設計
- ScriptableObjectでパラメータ管理
- プログラマー以外でも調整可能
- バランス調整が高速化

---

## 🛠️ 開発環境の理想像

### 目標の開発フロー

```
開発者の作業:
1. ゲームデザインを考える（紙・テキスト）
2. AIにコード生成を依頼（Claude / Cursor）
3. アセットを組み合わせる（ドラッグ&ドロップ）
4. AI生成素材を適用
5. プレイモードでテストプレイ ← ここに時間を使う
6. バランス調整（ScriptableObject編集）
7. 繰り返し

コーディングは最小限。Unityエディタを触るのも最小限。
```

### 実現のための構成

```
【レイヤー1: ゲームコア】
└ 高品質アセット（Space Shooter, Game Creator等）

【レイヤー2: カスタムロジック】
└ AI生成コード + 手動調整

【レイヤー3: データ】
└ ScriptableObject（キャラ、武器、ステージ等）

【レイヤー4: 見た目】
└ AI生成素材（キャラ、UI、エフェクト）

【レイヤー5: サウンド】
└ AI生成音楽・効果音
```

各レイヤーは疎結合。破綻しても他に影響しない。

---

## 📦 推奨アセット一覧

### 🎮 ゲームコア・システム

#### キャラクター育成・RPGシステム

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **Game Creator 2** | $80 | RPGシステム全般 | ★★★★★ | ビジュアルスクリプティング、育成・インベントリ統合 |
| **GameKit** | 無料 | RPGシステム基礎 | ★★★☆☆ | 学習用、軽量 |
| **RPG Builder** | $100 | RPGオールインワン | ★★★☆☆ | 重いが高機能 |
| **PlayMaker** | $65 | ビジュアルスクリプティング | ★★★★☆ | ノーコード開発 |

#### セーブ/ロード

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **Easy Save 3** | $45 | セーブ/ロード | ★★★★★ | 最優先購入推奨 |
| JSON + PlayerPrefs | 無料 | セーブ/ロード | ★★☆☆☆ | 試作用、暗号化なし |

#### インベントリ・装備

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **Inventory Pro** | $70 | インベントリ全般 | ★★★★★ | 商業レベル |
| **Ultimate Inventory System** | $60 | インベントリ | ★★★★☆ | Inventory Proの代替 |
| Simple Inventory System | 無料 | 基本機能のみ | ★★☆☆☆ | 学習用 |

#### ミッション・クエスト

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **Quest System Pro** | $35 | クエスト管理 | ★★★★☆ | デイリー/ウィークリー対応 |
| 自作（SO + Manager） | 無料 | カスタムクエスト | ★★★★☆ | 推奨：仕様が独自になりやすい |

#### 会話システム

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **Yarn Spinner** | 無料 | 会話・ノベル | ★★★★★ | 無料で高機能 |
| **Dialogue System for Unity** | $70 | 会話全般 | ★★★★☆ | 有料だが最高峰 |
| 自作（テキスト表示） | 無料 | シンプル会話 | ★★☆☆☆ | 学習用 |

---

### 🎨 UI・ビジュアル

#### UI素材・フレームワーク

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **Modern UI Pack** | $20 | UI素材一式 | ★★★★☆ | 即戦力 |
| **Unity UI Extensions** | 無料 | UI拡張機能 | ★★★★☆ | GitHub、必須級 |
| **UI Builder + UI Toolkit** | 無料 | Unity標準 | ★★★☆☆ | 学習コストあり |
| **TextMesh Pro** | 無料 | テキスト表示 | ★★★★★ | Unity標準、必須 |

#### アニメーション

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **DOTween (Free)** | 無料 | トゥイーン | ★★★★★ | 必須、超簡単 |
| **DOTween Pro** | $15 | 高度なトゥイーン | ★★★★☆ | Freeで十分な場合多い |
| **Amplify Shader Editor** | $60 | シェーダー作成 | ★★★☆☆ | 見た目こだわる場合 |

#### VFX・パーティクル

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **Cartoon FX Remaster** | $25 | 汎用VFX | ★★★★☆ | カラフル、カートゥーン |
| **Epic Toon FX** | $35 | トゥーンVFX | ★★★★☆ | アニメ調 |
| Unity VFX Graph | 無料 | VFX作成 | ★★★☆☆ | 自作する場合 |

---

### 🔊 サウンド

| ツール | 価格 | 用途 | 推奨度 | 備考 |
|--------|------|------|--------|------|
| **Suno AI** | 無料〜 | BGM生成 | ★★★★★ | AI生成、商用可 |
| **ElevenLabs** | 無料〜 | ボイス生成 | ★★★★★ | AI音声 |
| **Epidemic Sound AI** | $15/月 | 効果音・BGM | ★★★★☆ | サブスク、高品質 |
| **Master Audio** | $60 | サウンド管理 | ★★★☆☆ | Unityアセット |

---

### 🎯 ジャンル別：シューティング

| アセット | 価格 | 用途 | 推奨度 | 備考 |
|---------|------|------|--------|------|
| **Space Shooter Template FREE** | 無料 | 2Dシューター | ★★★★★ | 本プロジェクトで使用中 |
| **Shoot 'Em Up Kit** | $50 | 2Dシューター | ★★★★☆ | 有料版、高機能 |
| **Top Down Engine** | $60 | トップダウン全般 | ★★★★☆ | 視点切替可能 |

---

### 💰 試作フェーズの推奨構成

#### 完全無料プラン（$0）

```
✅ Space Shooter Template FREE  - シューティングコア
✅ DOTween (Free)               - アニメーション
✅ Unity UI Extensions          - UI拡張
✅ TextMesh Pro                 - テキスト表示
✅ Yarn Spinner                 - 会話システム
✅ JSON + PlayerPrefs           - セーブ/ロード
✅ AI生成素材                   - ビジュアル/サウンド

開発期間: やや長め
品質: 十分実用的
```

#### 最小投資プラン（$45）

```
✅ Space Shooter Template FREE  - シューティングコア
✅ Easy Save 3 ($45)            - セーブ/ロード ← これだけ買う
✅ DOTween (Free)               - アニメーション
✅ Unity UI Extensions          - UI拡張
✅ Yarn Spinner                 - 会話システム
✅ AI生成素材                   - ビジュアル/サウンド

開発期間: 短縮（30-40%削減）
品質: 商業レベル入口
コスパ: 最高
```

#### 本格プラン（$200-300）

```
✅ Space Shooter Template FREE  - シューティングコア
✅ Easy Save 3 ($45)            - セーブ/ロード
✅ Game Creator 2 ($80)         - RPGシステム全般
✅ Inventory Pro ($70)          - インベントリ/装備
✅ Modern UI Pack ($20)         - UI素材
✅ DOTween Pro ($15)            - アニメーション
✅ Yarn Spinner (無料)          - 会話システム
✅ AI生成素材                   - ビジュアル/サウンド

開発期間: 大幅短縮（60-70%削減）
品質: 商業レベル
```

---

## 🤖 AI活用戦略

### 1. コード生成

#### 推奨ツール
- **Cursor**: VSCodeベース、AI統合エディタ（最推奨）
- **Claude 3.5 Sonnet**: コード生成最強
- **ChatGPT-4**: 汎用性高い
- **GitHub Copilot**: リアルタイム補完

#### 使い方（例）

```
【プロンプト例1: キャラクター育成システム】

「ScriptableObjectベースのキャラクター育成システムを作ってください。

要件:
- キャラクター名、レベル、経験値、HP、攻撃力、防御力、移動速度
- レベルアップ時に各ステータスが成長（AnimationCurveで調整可能）
- 経験値テーブルは指数的成長
- セーブ/ロード対応（JSON）
- C#、Unity 6000対応

namespaceはGame.Characterで。」
```

```
【プロンプト例2: ガチャシステム】

「確率ベースのガチャシステムを実装してください。

要件:
- レアリティ（N/R/SR/SSR）ごとに排出率設定
- 重み付き抽選
- 天井システム（100回でSSR確定）
- 排出履歴記録
- ScriptableObjectでガチャプール管理

C#、Unity対応。」
```

#### AI生成コードの扱い方

1. **そのまま使えることは稀**（70-90%の完成度）
2. **たたき台として優秀**
3. **理解して調整**（ブラックボックスにしない）
4. **テストしながら修正**

---

### 2. 画像生成

#### キャラクター

##### Midjourney / DALL-E 3

```
【プロンプト例: キャラクタースプライト】

"anime style character sprite sheet, 
cute female pilot in blue sci-fi uniform, 
multiple poses (idle, walk, attack, victory),
front view and side view,
white background, 
flat colors with simple shading,
game asset, 2D illustration,
high resolution"

追加指示:
- --ar 16:9 (横長のシート)
- --v 6 (Midjourney V6)
- --style raw (よりゲーム的に)
```

##### Stable Diffusion（ローカル）

```
モデル: Anything V5 / AbyssOrangeMix
プロンプト: 
"1girl, pilot uniform, blue and white, full body,
sprite sheet, multiple angles, white background,
game asset, flat colors, simple shading,
high quality, 4k"

Negative: 
"3d, realistic, photograph, blur, low quality"
```

#### UI素材

```
【プロンプト例: ボタンセット】

"mobile game UI button set,
sci-fi futuristic style,
blue and purple gradient with neon glow,
glossy metallic effect,
rounded corners,
5 variations (normal, pressed, disabled, highlighted, special),
PNG with transparency,
game asset, clean design,
4k resolution"
```

#### 背景

```
【プロンプト例: ゲーム背景】

"space station interior background,
sci-fi anime style,
blue purple color scheme,
cyberpunk elements,
2D game background, parallax layers,
high resolution 4k,
horizontal composition"
```

---

### 3. サウンド生成

#### BGM（Suno AI）

```
【プロンプト例: バトルBGM】

"upbeat electronic game music,
fast tempo 140-160 bpm,
energetic and intense,
sci-fi atmosphere,
synthesizer lead with heavy bass,
loop-able structure,
no vocals"

スタイルタグ: 
[electronic], [game], [intense], [sci-fi]
```

```
【プロンプト例: ホーム画面BGM】

"calm ambient game music,
slow tempo 80-100 bpm,
relaxing and peaceful,
soft piano with light synth pad,
atmospheric and dreamy,
loop-able, seamless"

スタイルタグ:
[ambient], [calm], [game], [atmospheric]
```

#### 効果音（ElevenLabs / Epidemic Sound）

- ボタン音、決定音
- レベルアップ音
- ガチャ演出音
- ダメージ音、爆発音

**推奨**: Asset Storeの無料効果音パック + 必要に応じてAI生成

---

### 4. デバッグ・最適化支援

#### Claude / ChatGPTに依頼

```
【プロンプト例: エラー解決】

「以下のエラーが出ています。原因と修正方法を教えてください。

エラーメッセージ:
[エラー内容をコピペ]

関連するコード:
[該当部分のコードをコピペ]」
```

```
【プロンプト例: 最適化】

「以下のコードをモバイル向けに最適化してください。
特にGC Allocとドローコールを削減したいです。

[コードをコピペ]」
```

---

## 📅 開発フェーズ戦略

### フェーズ0: 準備（1日）

```
□ Unity 6000インストール
□ 推奨エディタ（Cursor / Rider）セットアップ
□ AI（Claude / ChatGPT）アカウント
□ 画像生成AI（Midjourney / DALL-E）準備
□ 音楽生成AI（Suno）準備
□ プロジェクト作成
```

---

### フェーズ1: 試作（2-3週間）

#### 目標
「これはいける」という確信を得る

#### 実装
1. **コアゲームループ**（1週間）
   - アセット導入（Space Shooter等）
   - 基本動作確認
   - AI生成素材でビジュアル差し替え

2. **育成システム骨格**（1週間）
   - ScriptableObjectでデータ設計
   - レベル/経験値システム
   - セーブ/ロード

3. **統合**（3-5日）
   - 育成値をゲームプレイに反映
   - 簡易ホーム画面
   - シーン遷移

#### 成果物
- 動作するゲームループ
- データ駆動開発の実感
- AI素材の統一感確認
- 拡張性の確信

#### 判断基準
✅ アセット活用が有効
✅ AI生成が実用的
✅ 開発速度が速い
✅ 統一感が出せる

→ **次フェーズへ。本格アセット購入決定**

---

### フェーズ2: 本格開発（1-2ヶ月）

#### 購入するアセット
```
- Game Creator 2 / Inventory Pro
- Modern UI Pack
- Quest System Pro
- その他必要に応じて
```

#### 実装
1. **フル育成システム**
   - 複数キャラクター
   - 装備システム
   - スキルツリー

2. **ガチャ・ショップ**
   - ガチャシステム
   - 課金導線
   - 通貨管理

3. **ミッション・デイリー**
   - クエストシステム
   - 報酬システム

4. **会話パート**
   - Yarn Spinner統合
   - キャラクター会話
   - ストーリー分岐

5. **UI全画面**
   - ホーム、ガチャ、育成、設定、ヘルプ
   - 統一デザイン

---

### フェーズ3: 仕上げ（2-3週間）

```
□ バランス調整
□ バグ修正
□ パフォーマンス最適化
□ サウンド調整
□ ストアページ作成
□ ビルド・テスト配信
```

---

## 💡 実装パターン集

### パターン1: ScriptableObjectでのデータ管理

```csharp
// CharacterData.cs
[CreateAssetMenu(fileName = "Character", menuName = "Game/Character")]
public class CharacterData : ScriptableObject {
    public string characterName;
    public Sprite icon;
    public int baseHP;
    public int baseAttack;
    public float baseSpeed;
    public AnimationCurve hpGrowth;
    public AnimationCurve attackGrowth;
}
```

### パターン2: アセット統合（ラッパー方式）

```csharp
// ShooterBridge.cs（Space Shooterと育成システムの橋渡し）
public class ShooterBridge : MonoBehaviour {
    void Start() {
        var shooter = GetComponent<PlayerMoving>(); // プリセット
        var character = GameManager.Instance.CurrentCharacter; // 自作
        
        // 育成値を反映
        shooter.moveSpeed = character.CurrentSpeed;
        shooter.GetComponent<Player>().maxHealth = character.CurrentHP;
    }
}
```

### パターン3: イベント駆動（疎結合）

```csharp
// GameEvents.cs
public static class GameEvents {
    public static event System.Action<int> OnLevelUp;
    public static event System.Action<int> OnExpGained;
    
    public static void LevelUp(int newLevel) => OnLevelUp?.Invoke(newLevel);
    public static void ExpGained(int amount) => OnExpGained?.Invoke(amount);
}

// 使用例
GameEvents.OnLevelUp += (level) => {
    Debug.Log($"Level Up! New level: {level}");
    // UI更新、サウンド再生等
};
```

---

## 🎯 成功のチェックリスト

### 開発開始前
- [ ] AI（コード、画像、音楽）の準備完了
- [ ] 使用アセットの選定完了
- [ ] ゲームデザインの骨子完成

### 試作フェーズ
- [ ] コアループが動作
- [ ] データ駆動設計を実現
- [ ] AI素材で統一感
- [ ] 拡張性を確認

### 本格開発フェーズ
- [ ] 高品質アセット導入
- [ ] 全機能実装
- [ ] バランス調整
- [ ] モバイルビルド成功

### リリース前
- [ ] パフォーマンス最適化
- [ ] バグゼロ（クリティカル）
- [ ] ストアページ完成
- [ ] 法務チェック（特にガチャ）

---

## 📝 最後に

このガイドは**「ゲーム開発の新しい形」**の実践記録です。

### 証明したいこと
「2025年、ゲーム開発初心者でも、高品質アセット + AI活用で商業レベルのゲームを作れる」

### このガイドの使い方
- プロジェクト開始時の指針に
- アセット選定の参考に
- AI活用のプロンプト集として
- 開発フェーズの進行管理に

### 更新予定
このガイドは実証実験の進行に合わせて更新されます。

**Let's make games with AI! 🚀**


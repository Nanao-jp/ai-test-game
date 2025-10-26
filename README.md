# ai-test-game02 — モバイルシューティング（実証プロジェクト）

## プロジェクトの目的

このプロジェクトは**「ゲーム開発初心者でも、高品質アセット + AI支援で商業レベルのゲームを作れる」という仮説の実証実験**です。

### コアコンセプト
- ゲーム開発未経験者が主導
- 既製アセット（無料/有料）を最大活用
- AI（コード生成、素材生成、アシスタント）を全面活用
- 開発者はUnityに最低限しか触れず、**ゲームデザインとテストプレイに集中**

### 最終目標
縦スクロールシューティング + キャラ育成 + 会話パート + ガチャ を統合したモバイルゲーム

---

## 現在の状態（試作フェーズ）

### 完成済み
- ✅ Space Shooter Template FREEのプリセット導入
- ✅ 入力システム設定（Both: Legacy + Input System）
- ✅ ログ機能（ConsoleLogRecorder）
- ✅ アスペクト比調整（AspectEnforcerPortrait）

### 次のステップ
1. キャラクター育成システム（ScriptableObjectベース）
2. セーブ/ロードシステム
3. 育成値のシューティングパートへの反映
4. 簡易ホーム画面
5. AI生成素材での統一感の実現

---

## 技術スタック

### 開発環境
- **Unity**: 6000.2.9f1
- **レンダリング**: Universal Render Pipeline (URP) 17.2.0
- **入力**: Legacy Input + Input System（Both）
- **ターゲット**: iOS / Android（PCでテスト）

### 使用アセット
| アセット | 用途 | コスト | 状態 |
|---------|------|--------|------|
| Space Shooter Template FREE | シューティングコア | 無料 | ✅ 導入済み |
| Casual Game Sounds U6 | 効果音 | 無料 | ✅ 導入済み |
| ConsoleLogRecorder | ログ出力 | 自作 | ✅ 導入済み |
| AspectEnforcerPortrait | 9:16アスペクト強制 | 自作 | ✅ 導入済み |
| Audio System | 音声管理 | 自作 | ✅ 導入済み |
| DOTween (Free) | アニメーション | 無料 | 📋 予定 |
| Easy Save 3 | セーブ/ロード | $45 | 🔄 検討中 |

### AI活用
- **コード生成**: Claude / Cursor / ChatGPT
- **画像生成**: Midjourney / DALL-E 3
- **サウンド生成**: Suno AI / ElevenLabs
- **アシスタント**: Claude（設計、デバッグ、アドバイス）

---

## セットアップ

### 1. プロジェクトを開く
Unity 6000.2.9f1 以降でプロジェクトを開く

### 2. Space Shooterのセットアップ
```
Tools > Shooter > Setup (Space Shooter Preset)
```

これで：
- Demo_Scene が開く
- スタートシーンに設定
- ログ機能が初期化
- アスペクト比調整が適用

### 3. 音声のセットアップ（オプション）
```
Tools > Shooter > Setup Sound Effects
```

効果音が自動設定されます：
- プレイヤー死亡音（DM-CGS-09）
- 敵撃破音（DM-CGS-48）
- パワーアップ取得音（DM-CGS-07）

### 4. プレイ
プレイボタンを押すとSpace Shooterのデモが動作

### 操作
- **マウスドラッグ**: 機体移動
- **クリック**: 射撃

---

## プロジェクト構成

```
Assets/
  Scripts/
    Core/
      Audio/               # 音声システム（自動設定）
      Camera/              # AspectEnforcerPortrait（9:16強制）
      Debug/               # ConsoleLogRecorder（ログ出力）
      LayerUtil.cs         # ユーティリティ
  Editor/
    ProjectSetup/          # ShooterSetup（1クリックセットアップ）
  Space Shooter Template FREE/
    Prefabs/               # プリセットプレハブ
    Scenes/                # デモシーン
    Scripts/               # プリセットスクリプト
  Casual Game Sounds U6/   # 効果音パック
Logs/
  latest.log             # プレイごとのログ
Docs/
  AI_Driven_Development.md  # 開発手法・アセット集・哲学
```

---

## 開発原則

### 1. アセットファースト
車輪の再発明をしない。実績のあるアセットを活用し、開発時間を短縮。

### 2. AI駆動開発
コード生成、素材生成、デバッグ支援をAIに任せ、開発者はゲームデザインに集中。

### 3. データ駆動
ScriptableObjectでパラメータ管理。プログラマー以外でも調整可能に。

### 4. 疎結合
各システム（シューティング、育成、UI）を独立させ、破綻を防ぐ。

### 5. 段階的開発
試作で確信 → 本格アセット投資 → フル開発

---

## トラブルシュート

### Input エラー
```
InvalidOperationException: You are trying to read Input using the UnityEngine.Input class...
```

**解決策**:
1. `Edit > Project Settings > Player`
2. `Active Input Handling` を **"Both"** に設定
3. Unity を再起動

### コンパイルエラー
Safe Modeの警告が出る場合、削除したスクリプトへの参照が残っている可能性があります。
`Logs/latest.log` でエラー内容を確認してください。

---

## ドキュメント

- **開発手法・哲学**: [Docs/AI_Driven_Development.md](Docs/AI_Driven_Development.md)
  - AI駆動開発の戦略
  - 推奨アセット一覧（無料/有料）
  - 開発フェーズごとの指針
  - 素材生成のプロンプト集

- **設定スナップショット**: [Docs/Settings_Snapshots.md](Docs/Settings_Snapshots.md)
- **エディタ拡張**: [Docs/EditorExtensions.md](Docs/EditorExtensions.md)

---

## ライセンス

- **Space Shooter Template FREE**: Asset Storeのライセンスに従う
- **カスタム拡張部分**: MIT License
- **AI生成素材**: 各生成ツールの商用利用規約に従う

---

## 実証実験の進捗

このプロジェクトは**「新しいゲーム開発手法の実証」**です。

### 検証項目
- [ ] アセット活用でコア機能を短期間で実装できるか
- [ ] AIでコード/素材生成が実用レベルか
- [ ] 初心者でも商業レベルの品質に到達できるか
- [ ] 統一感のある見た目を維持できるか
- [ ] 拡張性を保ちながら開発できるか

### 最終的に証明したいこと
**「2025年以降、ゲーム開発の参入障壁は劇的に下がる。適切なアセットとAI活用で、初心者でもハイクオリティなゲームを作れる。」**

---

## 連絡先・フィードバック

このプロジェクトの手法や成果に興味がある方は、ぜひフィードバックをお願いします。

**このREADMEは実証実験の進行に合わせて更新されます。**

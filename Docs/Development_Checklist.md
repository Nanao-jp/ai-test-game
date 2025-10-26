# 開発チェックリスト（プリセット版）

このドキュメントは**Space Shooter Template FREEのプリセットをそのまま使う**ための最小限のチェックリストです。

---

## 初期セットアップ（5分）

### 1. プロジェクトを開く
- [ ] Unity 6000.2.9f1 以降でプロジェクトを開く
- [ ] Space Shooter Template FREE がインポートされていることを確認

### 2. Input Handling の確認
- [ ] `Edit > Project Settings > Player` を開く
- [ ] `Active Input Handling` が **"Both"** になっていることを確認
- [ ] なっていない場合は "Both" に変更して Unity を再起動

### 3. セットアップ実行
- [ ] `Tools > Shooter > Setup (Space Shooter Preset)` を実行
- [ ] Demo_Scene が開くことを確認
- [ ] `Logs/setup_summary.txt` が生成されることを確認

### 4. プレイテスト
- [ ] プレイボタンを押す
- [ ] マウスドラッグで機体が動くことを確認
- [ ] クリックで弾が発射されることを確認
- [ ] `Logs/latest.log` が生成されることを確認

---

## カスタム拡張を追加する場合（任意）

### ログ機能の活用
- [ ] `Logs/latest.log` でデバッグ情報を確認
- [ ] 必要に応じて `Debug.Log()` を追加

### アスペクト比の調整
- [ ] 9:16表示が必要な場合は `AspectEnforcerPortrait` をそのまま使用
- [ ] 不要な場合は Main Camera から削除

### カスタムスクリプトの追加
- [ ] `Assets/Scripts/` 配下に新しいフォルダを作成
- [ ] Space Shooterのスクリプトを継承または拡張
- [ ] プレハブをカスタマイズ

---

## トラブルシュート

### Input エラーが出る
- [ ] `Active Input Handling` を "Both" に変更
- [ ] Unity を再起動

### Demo_Scene が見つからない
- [ ] `Assets/Space Shooter Template FREE/Scenes/Demo_Scene.unity` が存在することを確認
- [ ] Space Shooter Template FREE が正しくインポートされているか確認

### ログが出力されない
- [ ] `Logs/` フォルダが存在することを確認
- [ ] `ConsoleLogRecorder` が初期化されているか確認（Setup実行時に自動）

---

## 今後の拡張方針

このプロジェクトは**シンプルさを重視**しています。
カスタム機能を追加する場合は、以下の原則に従ってください：

1. **最小限の追加**: 必要な機能だけを追加
2. **プリセットを優先**: Space Shooterの機能をまず活用
3. **ドキュメント化**: 追加した機能は README に記載
4. **独立性**: プリセットの動作を邪魔しない

---

## 変更履歴
- 2025-10-26: プリセット版に移行、カスタム実装を削除
- Input Handling を "Both" に設定

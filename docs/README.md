# Documentation

このディレクトリは、設計資料・Mermaid 図・実装計画をコードから分離して管理するためのドキュメント置き場です。

## 構成

- `design/` - 全体設計やアーキテクチャの説明
- `diagrams/` - Mermaid によるクラス図・シーケンス図
- `plans/` - 実装方針や改善計画

## Diagram の分類

旧 `Assets/Scripts/Diagram` の資料は、責務ごとに以下へ再配置しています。

- `diagrams/application/` - コマンド処理、入力変換、Factory / Composition Root
- `diagrams/domain/` - ドメインモデル、イベント、ステータスシステム
- `diagrams/presentation/` - Presenter / View / ViewModel
- `diagrams/runtime/` - MatchSession / GameFlow / UnityIntegration

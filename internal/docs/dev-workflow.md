# 開発ワークフロー（M3 完了後）

**切替タイミング:** MidiSynth **M3（定数 Expr）** をテスト＋版上げで閉じた直後から移行する。  
M3 完了前の先取り（Milestone／Issue 作成）は可。

**目的:** 就活作品・OSS として見られる履歴を残す。枚数稼ぎではなく、**普通のチーム開発に見える粒度**にする。  
**役割:** 実装は人手（原則）。**ブランチ作成・Issue／Milestone・PR・コメント編集・（委任時）マージはエージェント**が行う。

関連: [ROADMAP.md](ROADMAP.md) / [refactor-plan.md](refactor-plan.md) / [tickets/midisynth-tang-primer25k-requests.md](tickets/midisynth-tang-primer25k-requests.md)

**拠点（2026-10-22〜）:** 開発は GitLab [sharphdl/SharpHDL](https://gitlab.com/sharphdl/SharpHDL)（リモート `origin`、操作は `glab`）。GitHub `yukari6559/CsharpHdl`（リモート `github`）は `main` とタグだけを同期するコピーで、Actions・Issue・Wiki は無効。GitHub からのインポートで番号は保たれており、移行前の「PR #N」は GitLab の MR `!N` と同じもの。以下の「PR」は GitLab の **マージリクエスト（MR）** と読み替える。GitLab では Issue（`#N`）と MR（`!N`）の番号は別々に振られる。

**GitLab 側の設定（Ultimate / OSS プログラム）:** `main` は保護（push 不可、マージは Maintainer）、パイプライン成功とスレッド解決が必須、merge commit のみ（squash 不可）、`v*` タグは保護。プッシュルールでブランチ名（`<type>/<issue>-<short>`）とコミットメッセージ（Conventional）を検査し、シークレットのプッシュを拒否する。CI は build / test（JUnit・カバレッジ）、Secret Detection、GitLab Advanced SAST、Dependency Scanning、タグで NuGet 公開と GitLab Release。

---

## 0. 階層の正（取り込む例と捨てる例）

採用するイメージ（提案を業務／OSS 王道に合わせて調整したもの）:

```
Milestone …… テーマ全体（例: リファクタキャンペーン、M4、版 v0.3.9）
  └─ Issue …… レビュー可能な 1 ステップ（≒ 後で 1 PR）
        ├─ 本文チェックリスト …… 細かい手順（Issue にしない）
        └─ PR
              └─ コミット …… PR 内の論理単位（Issue にしない）
```

| 層 | 役割 | リファクタ例 |
|----|------|----------------|
| **Milestone** | テーマ全体を束ねる | `refactor-emitter`（R-E1〜E2 など関連をまとめる） |
| **Issue** | マージ可能な成果 1 つ | `#15 Unify dual Emitter paths (R-E1)` |
| **チェックリスト** | Issue 内の手順メモ | `- [ ] extract helpers` `- [ ] delete duplicate` |
| **PR** | Issue を閉じる差分 | `Closes #15` |
| **コミット** | PR 内の読みやすい区切り | Core 共通化 → 二重削除 → テスト緑確認 |

### 取り込んだ点（元の提案）
- リファクタなど **大きな仕事は Milestone に載せる**
- その中を **Issue（ステップ）に分割**する
- 作業を GitHub 上に残し、`main` 直コミット運用から離れる

### 採用しない点（細かすぎ／非王道）
- **サブIssue をコミット単位まで切る**
- **サブIssue 完了＝1コミットで Issue を閉じる**

理由（就活・業務の見え方）:
- 評価者／採用側が見るのは主に **PR の質・説明・テスト** と **Issue の意図**。Issue の枚数ではない
- OSS／業務の定石は **Milestone → Issue → PR**。コミットは PR の内部履歴
- サブIssue＝1コミットは管理コストが高く、ノイズに見えやすい

### サブIssueを使ってよい例外
別 PR になりうる・長いブロッカー・明確に並列できるときだけ。それでも **1 サブIssue ≈ 1 PR 候補** であり、1 コミットではない。

---

## 1. 新しいタスクを立てるときのフロー（必須）

エージェント／作者が「次の仕事」を切るときは、毎回この順で組む。

1. **テーマを決める** → Milestone があるか確認。無ければ作成  
   - 版: `v0.3.8`  
   - 機能束: `m4-local-signals`  
   - リファクタ束: `refactor-emitter`（[refactor-plan.md](refactor-plan.md) の関連 ID をまとめる）
2. **レビュー可能な塊に分割** → 各塊を **Issue** にする  
   - 目安: 半日〜数日、1 PR で見られる差分  
   - リファクタ: 原則 **R-\* の 1 ID ≈ 1 Issue**（密接なら 2 ID まで）
3. **細かい手順** → Issue 本文の **チェックリスト**（サブIssueにしない）
4. **実装** → `feat|refactor|…/<issue>-<short>` ブランチ → 論理単位でコミット → **PR**（`Closes #N`）
5. **完了** → PR マージで Issue クローズ。内部 tickets／refactor-plan の状態も更新

「タスク生成」の出力は、少なくとも次を含む文案にする:  
Milestone 名、Issue タイトル、本文（内部 ID・完了条件・チェックリスト案）、ブランチ名案、想定コミット分割（PR 内）。

---

## 2. 役割分担

| 置き場 | 役割 |
|--------|------|
| `internal/docs/`（Git で追跡。`internal/` のそれ以外はローカル専用） | 仕様・ロードマップ・詳細チケット・R-\* ID |
| **GitLab Milestone** | テーマ／版の束 |
| **GitLab Issue** | 公開される作業単位（1 Issue ≈ 1 MR） |
| **MR + コミット** | 実装履歴（コミットは Issue にしない） |
| タグ `v*.*.*` | GitLab CI で nuget 公開と GitLab Release |

仕様の正本 = internal。作業トラッキングの正本 = GitLab Issue／MR。

---

## 3. Milestone

| 種類 | 例 |
|------|-----|
| 版 | `v0.3.8` — M3 完了 |
| 機能テーマ | `m4-local-signals` |
| リファクタテーマ | `refactor-emitter`（中に R-E1, R-E2 の Issue） |

- オープン Issue はいずれかの Milestone に属させる
- リファクタ「全体」を Milestone にし、中の R-\* を Issue にする（元提案の良い部分）

---

## 4. Issue

### 立て方
- タイトル例: `refactor: Unify dual VerilogEmitter paths (R-E1)`
- 本文: 内部 ID、完了条件、触るパッケージ、**チェックリスト（細ステップ）**
- ラベル: `feat` / `refactor` / `docs` / `test` / `chore` / `sim` / `emit` / `core`
- Milestone 必須

### 粒度
| ダメ | よい |
|------|------|
| Milestone 無しで巨大 Issue 1 本 | テーマは Milestone、中身は複数 Issue |
| 「helpers 抽出」をサブIssue＋1コミット | 同じ内容を親 Issue の `- [ ]` に書く |
| M4 全部を 1 Issue | M4a / M4b など PR 単位に割る |

### 閉じ方
- PR の `Closes #N`
- 内部表（tickets / refactor-plan）も更新

---

## 5. ブランチ

```
main
feat/<issue>-<short>
refactor/<issue>-<short>
fix/<issue>-<short>
docs/<issue>-<short>
chore/<issue>-<short>
```

- Issue 番号を含める
- `main` から切る。共有ブランチへの force push 禁止
- M3 移行後、機能・リファクタは原則 `main` 直コミットしない

---

## 6. Pull Request とコミット

**PR**
- 1 PR ≈ 1 Issue
- Summary / Test plan / `Closes #N` / 内部 ID
- 自分のみレビューでも PR を残す（履歴が目的）
- **マージは merge commit のみ**（squash / rebase はリポで無効）。グラフ上で feature が `main` に合流して見えるようにする。`glab mr merge` を使う（マージ方式はプロジェクト設定で固定）。マージ後は `git push github main`（新しいタグも）で GitHub のコピーに同期する

**コミット（PR 内）**
- **1 コミット = 説明できる一塊**。複数単位なら分ける（例: Core → Sim → Emit → tests）
- ユーザーが「コミットして」だけでも、エージェントは差分を見て **必要なだけ複数コミット**する（都度確認しない）
- Conventional 寄り: `feat:` / `refactor:` / `fix:` / `docs:` / `chore:` / `test:`
- **コミットごとに Issue／サブIssueを増やさない**（分割するのは git commit のみ）
- 版上げは `chore: bump …` ＋タグ `vX.Y.Z`（実装 PR と分ける）

### `main` 直コミットの例外
- typo・1 行 docs（できれば Issue）
- ユーザーが「main に直接」と明示した場合

---

## 7. 具体例

### リファクタ（Emitter）
1. Milestone: `refactor-emitter`
2. Issue `#15` R-E1 統合（チェックリストに extract / merge / テスト）
3. Issue `#16` R-E2 wire/reg（意味論変更の可能性 → 別 Issue）
4. 各 Issue ごとにブランチ＋PR。`#15` の PR 内で複数コミット可

### 機能（M4）
1. Milestone: `m4-local-signals`
2. Issue: M4a 宣言、M4b Emit+Sim+テスト（または 1 Issue＋チェックリスト）
3. 同様に PR

---

## 8. エージェント向け

`no-concrete-code`（実装は明示依頼があるまで人手）は継続。実装の案内で新しい変数・クラス・メソッドなどが要るときは、名前の候補（2〜3 案、おすすめと理由つき）を出す。  
**リポジトリ操作（GitLab / ブランチ / GitHub への同期）はエージェントが担う**（ユーザーが都度指示しなくてよい。区切りが来たら実行する）。

### エージェントが実行する操作

| タイミング | 内容 |
|------------|------|
| **作業開始** | Milestone の有無を確認し、無ければ作成。対応 Issue を作成または既存に紐づけ。`feat|refactor|fix|docs|chore/<issue>-<short>` を **作成して checkout** |
| **マージ可能な品質になったとき** | リモートへ push → **PR 作成**（タイトル Conventional、本文に Summary / Test plan / `Closes #N` / 内部 ID）→ 必要に応じて PR 説明やレビューコメントを編集 → ユーザーが本運用で委任済みなら **マージ**（`glab mr merge`）→ GitHub へ同期。Issue／Milestone の状態を確認 |
| **版上げ** | 実装 PR と分け、chore Issue＋PR（例: bump 0.3.8）。時期の見積もりと実行もエージェントが持つ（§10「版上げの見込み」） |
| **「次へ」** | 次 Milestone／Issue を案内し、着手するならブランチまで切る |

### ユーザーが言うと特に動くフレーズ（必須ではない）
「checkout」「PR して」「マージまで」「Issue 切って」— 無くても区切りで上記を行う。

### やらない／推さない
| 内容 | 代わり |
|------|--------|
| サブIssue＝1コミット | Issue チェックリスト＋PR 内コミット |
| 勝手な `main` 直コミット（機能） | ブランチ＋PR |
| 実装の先回り | 明示「実装して」等があるまで案内のみ |

### 従来どおりの表

| 依頼 | 動作 |
|------|------|
| 新しいタスク／「次へ」 | §1 フロー＋ **ブランチ checkout**。サブIssue＝コミット案は出さない |
| 「実装して」 | 現ブランチ上で実装可（範囲内） |
| 「コミットして」 | ブランチ上で、**論理単位が複数なら複数コミット**。続けて PR を提案／作成 |
| 「サブIssueでコミット単位に」 | **推さない** |

---

## 9. 移行チェックリスト（M3 閉じた直後）

- [ ] M3 テスト緑・版上げ・タグ
- [ ] Milestone 作成（例: `refactor-emitter`、`m4-local-signals`）
- [ ] 直近 Issue 化（少なくとも R-E1）
- [ ] 以降はブランチ＋PR
- [ ] （任意）公開 README／CONTRIBUTING に Issue・PR 運用を一言

---

## 10. GitLab で回している仕組み（2026-10-22〜）

| 仕組み | どこにあるか | いつ手を入れるか |
|--------|--------------|------------------|
| **エピック** | グループ `sharphdl` の作業アイテム（#1 Stage 1 / #2 HC1 / #3 SX11 / #4 リファクタ / #5 GitLab 移行） | 新しい Issue を作ったら、該当するエピックを親にする。Stage や HC が増えたらエピックを足す |
| **Issue の関係** | Issue の「ブロックする / される」 | hdl-coverage-plan の「前提」と同じ関係を付ける |
| **要件** | 作業アイテムの Requirement（#120〜#126 = R1〜R7、#150 = R8、#155 = R9、#158 = R10、#161 = R11）、対応表 `.gitlab/requirements.tsv` | 仕様の大事な規則を足したら要件を作り、対応表に検証するテスト名を書く。テスト名を変えたら対応表も直す（見つからないテストは不合格になる） |
| **Verilog の検証** | `tools/VerilogSamples`（出力するモジュールの一覧）、CI の `verilog-lint`（`templates/verilog-lint.yml` のコンポーネント） | 正しい回路を表すテスト用モジュールを足したら一覧に足す。lint のエラーはパイプラインを止める |
| **開発版パッケージ** | GitLab のパッケージレジストリ（`main` で Core / Emit / Sim か `Directory.Build.props` が変わったときに `X.Y.(Z+1)-dev.<パイプライン番号>`） | 何もしない。正式版の前に利用側で試してもらうときに案内する（docs/consumers.md）。パッケージに入るプロジェクトを足したら `publish-dev-package` の `changes` にも足す |
| **リリースノート** | タグのパイプラインで `tools/release-notes.sh` が生成し、GitLab Release に載せる（前の版からマージした MR ごとに 1 行。閉じた Issue と MR へのリンク付き。ci / docs / test などは折りたたみ） | MR のタイトルがそのまま載るので、Conventional の形で利用者に伝わる言葉にする。破壊的変更は MR のタイトルに `!` を付ける（`fix(emit)!:` など）。MR の説明に `Closes #N` を書く。スクリプトを変えたら MR の `release-notes-check` で出力を確かめる |
| **ドキュメントサイト** | https://sharphdl.gitlab.io/SharpHDL （`docs-site/`、`docs/`、XML ドキュメントコメント） | 公開 `docs/` を変えたら MR の `pages-preview` が通ることを確かめる。リンク切れは失敗になる。`main` のサイトは docs・`src/`・CI 定義が変わったときだけデプロイされる（`&docs-paths`） |
| **定期実行** | パイプラインのスケジュール（毎週月曜 3:00 JST） | 失敗したら原因（SDK の更新、新しい脆弱性など）を Issue にする |
| **警告の扱い** | `Directory.Build.props`（コードスタイルの規則もビルドで検査、CI では警告をエラー扱い） | 新しい警告を入れるとパイプラインが失敗する。意図して残す指摘は `[SuppressMessage]` に理由を書いて抑制する（抑制したものはレポートにも出ない） |
| **コード品質のレポート** | CI の `code-quality`（ビルドの SARIF を `tools/SarifToCodeQuality` で変換） | 何もしない。MR の画面と差分に、増えた / 減った指摘が出る |
| **作業の状態** | スコープ付きラベル `workflow::ready` / `in-progress` / `review` / `blocked`、ボード「Workflow」（エピックごとのスイムレーン表示も可） | 着手で `in-progress`、MR を作ったら `review`、前提が未完了なら `blocked`。ラベルは 1 つだけ付く。Issue が閉じたら `workflow::` ラベルは外す（閉じた Issue はボードの Closed 列に出る） |
| **重みと健康状態** | Issue の重み（1 / 2 / 3 / 5 / 8）、健康状態（順調 / 要注意 / 危険） | Issue を作るときに重みを付ける（最初の値は目安なので、分かった時点で直す）。作業中の Issue は健康状態を更新する |
| **自動マージ** | MR の「パイプライン成功で自動マージ」 | MR を作ったら予約してよい。失敗すると外れる。マージ後に GitHub へ同期する |
| **MR ごとのドキュメントのプレビュー** | CI の `pages-preview`（`https://sharphdl.gitlab.io/SharpHDL/mr-<iid>/`、環境 `docs/mr-<iid>`） | 何もしない。MR を閉じると消える |
| **CI の高速化** | NuGet のキャッシュ（ロックファイルで鍵）、ジョブの中断可（公開系は除く）、古いパイプラインの自動キャンセル | 依存パッケージを変えるとキャッシュが作り直される |
| **リリース環境の保護** | 環境 `release`（NuGet 公開）は Maintainer だけが実行できる | 協力者を Developer で迎えた場合も公開は Maintainer のみ |
| **DORA 指標** | 環境 `release` は本番（`deployment_tier: production`）。「分析」→ CI/CD・Value Stream にデプロイ頻度と変更のリードタイムが出る | 何もしない。リリース（タグ）ごとに 1 回のデプロイとして数えられる |
| **使わない機能** | プロジェクト設定でスニペット・フィーチャーフラグ・インフラ・Monitor・Wiki・コンテナレジストリを無効 | 必要になったら設定の「可視性・機能」で有効にする |
| **脆弱性の自動解決** | グループの公開プロジェクト `sharphdl/security-policies`（MIT）のポリシー | 検出されなくなった脆弱性は自動で解決済みになる。ポリシー用プロジェクトも公開・OSS のまま保つ（グループ内はすべて公開が OSS プログラムの条件） |
| **CI/CD コンポーネント** | `templates/verilog-lint.yml`（CI/CD Catalog に登録済み。次のタグから版が載る） | 入力や挙動を変えたら README の表も直す。互換を壊す変更はコミット件名に `!` |
| **セキュリティ** | MR のセキュリティ欄と脆弱性レポート（Advanced SAST / 依存関係スキャン / シークレット検出、プッシュ時のシークレット拒否） | 指摘が出たら Issue にする。依存パッケージを変えたら `packages.lock.json` も更新してコミットする |
| **依存パッケージの自動更新** | Renovate（`renovate.json`、CI の `renovate` ジョブ。定期実行のパイプラインで動く。手動で回すときはスケジュールを「実行」）。一覧は Issue「Dependency Dashboard (Renovate)」 | マイナー / パッチはパイプラインが通れば自動マージ。メジャーは人が見る（テストの件数が減っていないかも確認）。.NET の SDK イメージは 10.x に固定しているので、対象フレームワークを上げるときに `renovate.json` の `allowedVersions` も一緒に上げる |
| **変更のないジョブの省略** | `.gitlab-ci.yml` の `.code-rules`（MR では `src/` `tools/` `templates/` `examples/`、ソリューション、`Directory.Build.props`、`.editorconfig`、CI 定義が変わったときだけビルド・テスト・lint）。`pages-preview` は docs と `src/` の変更で動く。main・タグ・定期実行は常に全部 | ビルドに効く場所（新しいトップレベルのフォルダ、`global.json` など）を足したら `.code-rules` の一覧にも足す。セキュリティスキャンは常に動く |
| **例の検査** | CI の `examples-lint`（`tools/emit-examples.sh` が `examples/<名前>/` を 1 つずつビルドし、`tools/ExampleVerilog` が中の Module を `<名前>-<型>.v` に出力して lint） | 新しい例はフォルダを足すだけで対象になる。Module には引数なしのコンストラクタを付ける。lint のエラーはパイプラインを止める |
| **リリース** | Issue テンプレート「Release」（版上げ MR・タグ・nuget.org と GitLab Release の確認・GitHub への同期） | 版を出すときはこのテンプレートで Issue を作る |
| **版上げの見込み** | Issue「Release vX.Y.Z」（テンプレート「Release」。時期の条件の Issue に「ブロックされる」関係、`workflow::blocked`） | 版を出したらエージェントが次の版の中身と時期（「#N がマージされたら」）を見積もって Issue を作る。条件の Issue が閉じたら、依頼がなくてもエージェントが版上げを進める。壊れた Verilog や Sim との食い違いを直す修正、破壊的変更が溜まったら前倒しする。1.0 までは破壊的変更か新機能があればマイナー、修正だけならパッチ |
| **グループのトップページ** | 公開プロジェクト `sharphdl/gitlab-profile`（README と MIT の LICENSE） | 紹介文やリンクが変わったら README を直す。グループ内のプロジェクトはすべて公開・OSS ライセンスのまま |

---

## 改版履歴

| 版 | 日付 | 変更 |
|----|------|------|
| 1.11 | 2026-10-23 | §10 リリースノートは MR ごと（Issue へのリンク付き）に変更 |
| 1.10 | 2026-10-23 | §10 版上げの見込み（エージェントが次の版の時期を見積もり、条件がそろったら進める）。§8 名前の候補を出す |
| 1.9 | 2026-10-22 | §10 開発版パッケージとサイトは元が変わったときだけ公開。DORA 指標・使わない機能を追加 |
| 1.8 | 2026-10-22 | §10 に依存パッケージの自動更新・変更のないジョブの省略・例の検査・リリース・グループのトップページを追加。閉じた Issue の作業ラベルを外す。`docs-check` を `pages-preview` に訂正 |
| 1.7 | 2026-10-22 | §10 に作業の状態ラベル・重み・自動マージ・MR プレビュー・CI の高速化・リリース環境の保護・脆弱性の自動解決・CI/CD コンポーネントを追加 |
| 1.6 | 2026-10-22 | §10 に警告の扱い（CI でエラー）とコード品質のレポートを追加 |
| 1.5 | 2026-10-22 | §10 GitLab で回している仕組み（エピック・要件・Verilog の検証・開発版・リリースノート・サイト・定期実行・セキュリティ） |
| 1.4 | 2026-10-22 | 拠点を GitLab に移行（`glab`、MR、GitHub は同期コピー）。GitLab 側の保護・プッシュルール・CI を記載 |
| 1.3 | 2026-09-18 | 「コミットして」でも論理単位で複数コミット分割 |
| 1.2 | 2026-09-18 | エージェントがブランチ／Issue／PR／（委任時）マージを担当すると明記 |
| 1.1 | 2026-09-18 | Milestone→Issue→PR を正とする。元提案の取り込む／捨てるを明記。タスク生成フロー固定 |
| 1.0 | 2026-09-18 | 初版 |

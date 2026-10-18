# DSL 仕様 v0.10

CsharpHdl が提供する **ハードウェア記述 API** の契約です。  
実装のクラス名は変えてよいが、意味論はここに合わせる。

---

## 1. 設計原則

1. **明示的な時間**: 組み合わせ（`Comb`）と同期（`Seq`）を分ける  
2. **幅が必須**: すべての信号にビット幅がある  
3. **r0 のような暗黙魔法は DSL 側に持たない**（消費者が書く）  
4. **1 モジュール = 1 Verilog module**  

---

## 2. 中核型（概念）

| 型 | 意味 |
|----|------|
| `Module` | ハードウェアモジュール。ポートと内部論理を持つ |
| `Signal` | ワイヤまたはレジスタの抽象 |
| `In<T>` / `Out<T>` / `InOut<T>` | ポート |
| `Wire` / `Reg` | モジュール内ローカル信号（ポートではない。M4a、下記 §2.1） |
| `UIntN` / `Bits` | 符号なしビットベクタ（幅 N） |
| `Clock` / `Reset` | クロック・リセット（Seq で使用） |

幅の表し方（実装選択）:

- `Signal.UInt(32, "pc")`  
- または `UInt32` エイリアス + ポート属性  

### 2.1 ローカル信号 `Wire` / `Reg`（M4a + M4b）

中間値を `Out` にせずモジュール内に閉じて持つための信号。幅と名前は必須。

```csharp
// 凡例
public Wire Sum { get; } = Wire.UInt(8, "sum");
public Reg Acc { get; } = Reg.UInt(8, "acc");

public MyModule()
{
    SetPorts([Clk, Rst, A, B, Y]);
    SetLocals([Sum, Acc]);   // 1 個ずつなら SetLocal
}
```

- 方向は `SignalDirection.Internal`。ポート一覧（`GetPorts`）には入らず、`GetLocals` で取得する
- `SetLocal` / `SetLocals` には `Internal` 以外（`In` / `Out`）を渡せない。逆に `SetPort` / `SetPorts` に `Wire` / `Reg` は渡せない。違反は `SignalDeclarationException`
- `Wire` / `Reg` の区別は **代入できる文脈** を表す（下表）。Verilog の `wire` / `reg` キーワードとは一致しない（Comb の `If` 内で代入される `Wire` は emit 上 `reg` 宣言になりうる）
- Describe 後、文中で使った `Internal` 信号が `GetLocals` に無い場合は `SignalDeclarationException`（Sim の `Run` と Emit の両方で検査）。ポート（`In` / `Out`）の `SetPorts` 漏れは別 Issue（#99）

**代入できる文脈**（`Signal.Assign` の 2 オーバーロード）:

| 型 | `Comb` 内 `Assign(expr)` | `Seq` 内 `Assign(reset, next)` |
|----|---------------------------|--------------------------------|
| `In` | 不可 | 不可 |
| `Out` | 可 | 可 |
| `Wire` | 可 | 不可 |
| `Reg` | 不可 | 可 |

不可の組み合わせは Describe 時に `DescribeContextException`（幅チェックより先に判定）。

**Emit / Sim（M4b）:** 内部 `wire` / `reg` 宣言とシミュレーション上の登録・`Get` は対応済み。詳細は [verilog-emit-spec.md](verilog-emit-spec.md)・[sim-spec.md](sim-spec.md)。

---

## 3. モジュール記述のイメージ

```csharp
public sealed class Alu : Module
{
    public In A { get; } = In.UInt(32, "A");
    public In B { get; } = In.UInt(32, "B");
    public In Op { get; } = In.UInt(2, "Op");
    public Out Y { get; } = Out.UInt(32, "Y");

    protected override void Describe()
    {
        Comb(() =>
        {
            Switch(Op,
                (0, () => Y.Assign(A + B)),
                (1, () => Y.Assign(A - B)),
                (2, () => Y.Assign(A & B)),
                (3, () => Y.Assign(A | B)));
        });
    }
}
```

---

## 4. 文・演算（Phase 1〜2 で必須）

### 組み合わせ `Comb`

- 内部の代入は Verilog `assign` または `always_comb` 相当
- 同一信号への複数駆動はエラー

### 同期 `Seq(clock, reset)`

- 代入は非ブロッキング `<=`
- リセット時の初期値を指定できること

### 制御

| 構文 | 意味 | 状態 |
|------|------|------|
| `If` / `Else` | 条件分岐（下記 API） | **M1b 完了**（`0.3.7`） |
| `Switch` / `Case` | 多分岐（定数セレクタ） | 実装済み |
| 算術 `+ -` | 左右は同じ幅（違えば `WidthMismatchException`）。結果も同じ幅で、桁あふれは切り捨て | 実装済み |
| 乗算 `*` | 左右は同じ幅で、結果も同じ幅（積の上位は切り捨て）。全幅の積は先に広げる: `a.ZeroExtend(16) * b.ZeroExtend(16)`（8 × 8 → 16）。広げ忘れて広い左辺に代入すると幅の不一致で例外になるので、黙って桁が落ちることはない | 実装済み（M2a）。全幅の積の補助 `MulFull` は #114、符号付きは HC6 |
| ビット `&` / `|` / `^` / `Xnor` | ビット演算。左右は同じ幅で、結果も同じ幅。`a ^ b` は Verilog `^`、`a.Xnor(b)` は `~^` | 実装済み（`^` / `Xnor` は M2c、PR #103）。単項 `~` は #101 |
| 論理シフト `<<` / `>>` | 量は定数（`x << 2`）か信号・式（`x << amt`）。結果は左の幅。量が左の幅以上なら 0。**左右の幅の規則の対象外**（量の幅は自由）。定数量は負なら `ArgumentOutOfRangeException`、値を表せる最小幅の定数になる（`x << 2` は Verilog `x << 2'd2`） | 実装済み（M2b）。算術シフト `>>>` は符号付き型と一緒に HC6 |
| 比較 | 結果 **1 bit**（下記） | **M1a 完了**（`0.3.6`） |
| 連結 / スライス | **T2b**（下記）。メソッド API | 完了 |

**既知の制約（演算の Sim / Emit 一致）:** 次の点は Sim と Verilog の結果がずれうる。#9 の残りの演算より先に直す。

| 制約 | 内容 | Issue |
|------|------|-------|
| 入れ子の演算の括弧 | **解消済み**。演算の左右が演算なら括弧で囲んで出す（verilog-emit-spec §5c） | #102 |
| 左右の幅 | **解消済み**。二項演算と比較（`Eq` / `Neq` / `Xnor` を含む）は左右の幅が違うと Describe 時に `WidthMismatchException`。幅を揃えるときは `ZeroExtend` / `SignExtend` を明示する（例: `A + B.ZeroExtend(8)`） | #105 |
| 式のスライス・符号拡張 | **解消済み**。`Slice` / `SignExtend` の対象は信号か信号のスライスに限り、それ以外は `UnsupportedSelectException`。`Slice` の `Slice` は 1 段にまとめ、`Slice` の `SignExtend` は符号ビットを元の信号から取る（下記 T2b / T2e） | #104 |

#### 比較（M1a）

C# の `Expr` に対する `==` / `!=` は参照・null 等と衝突するため、**等価はメソッド**にする。

| API | 意味 | 結果幅 |
|-----|------|--------|
| `a.Eq(b)` / `a.Neq(b)` | 等価 / 非等価 | 1 |
| `a < b` / `a <= b` / `a > b` / `a >= b` | 符号なし比較 | 1 |

比較も左右は同じ幅（#105）。シフトだけは例外で、量の幅は左と違ってよい。

Comb / Seq の条件や `If` の cond に使える。Emit / Sim 同一意味論。

#### `If` / `Else`（M1b・完了 `0.3.7`）

C# に文としての `if` を埋め込めないため、**名前付き `Action` 引数**を正とする。

```csharp
// 凡例（API イメージ）
If(A.Eq(B),
    then: () => Y.Assign(C),
    @else: () => Y.Assign(D));

If(A.Eq(B), then: () => Y.Assign(C)); // @else 省略可
```

- 引数名は `then` / `@else`（`else` は予約語）
- 位置引数でも呼べるが、**テスト・ドキュメントは名前付き前提**
- Comb / Seq コンテキストに積む（CurrentWrite）。ネスト可（Core）。Emit／Sim 初版は **Comb + 枝内 `AssignStmt`**（ネスト If・Seq 内 If の Emit/Sim は後回し可）
- `@else` 省略時、偽枝では当該信号を更新しない（Sim は前値維持。完全な組み合わせには else か全経路代入を推奨）

#### `Switch` / `Case`

セレクタ `Signal`（現状 API は `In`）と **定数** `uint` ケースの対応。各ケースは `Action` で枝の代入を積む。

```csharp
// 凡例
Switch(Op,
    (0, () => Y.Assign(A + B)),
    (1, () => Y.Assign(A - B)),
    (2, () => Y.Assign(A & B)),
    (3, () => Y.Assign(A | B)));
```

Comb の Emit は同一出力への三項 `assign` チェーン（現行）。ケース間で左辺信号が食い違うと例外。

#### いつ `If` / いつ `Switch`（M1c）

| | **If** | **Switch** |
|--|--------|------------|
| 条件 | 述語 `Expr`（`Eq` / 比較など、通常 1 bit） | セレクタ信号と **定数**ケース |
| 向くもの | ゲート、2 枝、ネストしうる制御、閾値比較 | Op コードなど **多定数**の一択分岐 |
| 書かない方がよい例 | 0..N の定数テーブルを If の連鎖だけで書く | 比較結果や任意 Expr をセレクタ代わりにする（API 上も定数ケース前提） |

迷ったら: **「定数ラベルで分岐」→ Switch、「真偽・比較で分岐」→ If。**

### スライス / 連結（T2b）

方針: **メソッド**（インデクサ＋レンジ型は採用しない）。

| API（概念名） | 意味 | 結果幅 |
|---------------|------|--------|
| `Slice(hi, lo)` | Verilog の `x[hi:lo]`（両端 inclusive） | `hi - lo + 1` |
| `Concat(a, b, …)` または 2 引数の入れ子 | Verilog の `{a, b, …}`（左が MSB） | 各オペランド幅の和 |

ルール:

- インデックスは **定数**（動的ビット選択は対象外）
- `hi >= lo`、かつ対象の幅に収まる（`hi < width`）。違反は `WidthMismatchException`。対象がスライスのときは、そのスライスの幅で測る
- `Expr` / `GetWidth` で結果幅が分かるようにする
- 代入は従来どおり左辺幅と一致
- 初版は **右辺のみ**（`x[7:0] = …` のような部分代入左辺は後回し）
- 切り出す対象は **信号か信号のスライス** に限る。演算・連結・定数・拡張に `Slice` を付けると `UnsupportedSelectException`。演算の結果を切り出したいときはローカル `Wire` を経由する（#104）
- `Slice` の `Slice` は作る時点で元の信号の 1 段の `Slice` にまとめる（何段でも同じ）。例: `x.Slice(15, 8).Slice(3, 0)` は `x.Slice(11, 8)`

呼び出しの置き場（実装選択）: `Expr` 拡張メソッド、または `Signal` 上のメソッド。どちらでも意味論は上表に合わせる。

### 符号拡張 / ゼロ拡張（T2e）

| API | 意味 | 結果幅 |
|-----|------|--------|
| `SignExtend(toWidth)` | 符号ビット複製で幅を広げる | `toWidth` |
| `ZeroExtend(toWidth)` | 上位を 0 埋め | `toWidth` |

ルール:

- 元幅が `GetWidth()` で分かること。`toWidth >= srcWidth`（縮小は例外）
- 右辺のみ。emit は複製連結（`$signed` は使わない）
- `toWidth == srcWidth` のときは内側式のみ
- LB/LBU や即値フィールド切り出しは消費者側（`Slice` + Extend）
- `SignExtend` の対象も **信号か信号のスライス** に限る（それ以外は `UnsupportedSelectException`、#104）。`Slice(31, 20).SignExtend(64)` のような即値の取り出しはそのまま書ける
- `ZeroExtend` は 0 埋めの連結なので対象を問わない

### メモリ `Mem`（Phase 3 / T2c / T2d）

- **1R1W:** 同期読み・同期書き（既存）。`addr` は読み書き共用
- **2R1W オーバーロード:** 同期書き + **非同期（組み合わせ）読み**。ポートは `we` / `waddr` / `wdata` + `raddr0`/`rdata0` + `raddr1`/`rdata1`
- **1R1W + `wstrb`（T2d）:** バイトイネーブル。`wstrb` 幅 = `width / 8`（`width % 8 == 0`）。`we && wstrb[i]` のときだけバイト `i` を更新。2R1W には付けない。LB/SB デコードは消費者側
- アドレス幅はすべて `ceil(log2(depth))`。データ幅は `width`
- 同一アドレスの読み書きは初版 **旧値読み**（バイパスなし）。x0 ゼロ化は消費者側
- **読みレイテンシ:** 1R1W / wstrb は同期読み（addr 提示の次拍で `rdata` 有効）。2R1W 読みは組み合わせ（同一拍 RAW は旧値、書き込みコミット後の評価から新値）。Sim も同じ（詳細は [sim-spec.md](sim-spec.md) §4.2）

---

## 5. 階層

```csharp
public sealed class Top : Module
{
    readonly Alu _alu = new();

    protected override void Describe()
    {
        // ポート接続 API（実装者が設計）
        Connect(_alu.A, someSignal);
    }
}
```

子モジュールは別 `module` として emit されること。

---

## 6. 禁止・非対応（v0.1）

| 禁止 | 理由 |
|------|------|
| ポインタ・参照型の自動回路化 | HDL ではない |
| `async` / `await` | 時間モデルが違う |
| GC オブジェクト | ハードウェアに無い |
| 暗黙のクロック | バグの元 |

---

## 7. 名前付け

- Verilog 識別子にそのまま出せる名前（ASCII、予約語回避）
- モジュール名はクラス名、または属性で上書き
- 1 モジュール内で **ポートとローカル信号の名前は合わせて一意**（大文字小文字は区別）。`SetPort(s)` / `SetLocal(s)` の登録時に検査し、重複は `SignalDeclarationException`。リスト登録で例外になった場合、そのリストの信号・名前はいずれも登録されない
- 文中で使ったローカル（`Internal`）が `SetLocal(s)` 未登録なら、Describe 後の静的検査で `SignalDeclarationException`（§2.1）。ポート未登録は #99

---

## 改版履歴

| 版 | 日付 | 変更 |
|----|------|------|
| 0.10 | 2026-10-18 | M2a: 乗算 `*`（左右は同じ幅、結果も同じ幅。全幅は `ZeroExtend` で広げる） |
| 0.9 | 2026-10-18 | M2b: 論理シフト `<<` / `>>`（定数量・可変量。幅の規則の対象外、量が幅以上なら 0） |
| 0.8 | 2026-10-17 | #104: `Slice` / `SignExtend` の対象の制限、`Slice` の畳み込み、範囲の検査（T2b / T2e、既知の制約） |
| 0.7 | 2026-10-17 | #105: 二項演算と比較の左右は同じ幅が必須（§4 の演算・比較・既知の制約） |
| 0.6 | 2026-10-17 | M2c: XOR `^` / XNOR `Xnor`（§4、PR #103）。単項 `~` は #101。演算の既知の制約（括弧 #102 / 左右の幅 #105 / 式のスライス #104）を §4・T2b・T2e に明記 |
| 0.5 | 2026-10-16 | M4b: Emit/Sim 対応と未登録ローカル検査を §2.1 / §7 に反映（#8）。ポート未登録は #99 |
| 0.4 | 2026-10-14 | M4a: ローカル信号 `Wire` / `Reg`（§2.1）、型ごとの代入文脈、ポート／ローカル名の一意性（§7） |
| 0.3 | 2026-09-17 | M1b 完了反映、M1c If/Switch 使い分け |
| 0.2 | 2026-09-17 | 比較（Eq/Neq）確定、If 名前付き API 方針、状態列を追加 |
| 0.1 | 2026-08-12 | 初版 |

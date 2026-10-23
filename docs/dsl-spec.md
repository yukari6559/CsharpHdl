# DSL 仕様

CsharpHdl が提供するハードウェア記述 API の契約です。  
実装の型名は変わり得ますが、意味論はこの文書に合わせます。

---

## 1. 設計原則

1. **明示的な時間**: 組み合わせ（`Comb`）と同期（`Seq`）を分ける  
2. **幅が必須**: すべての信号にビット幅がある  
3. **ISA 固有の暗黙魔法は DSL に持たない**（例: レジスタ x0 のゼロ化は消費者側）  
4. **1 モジュール = 1 Verilog module**  

---

## 2. 中核型（概念）

| 型 | 意味 |
|----|------|
| `Module` | ポートと内部論理を持つハードウェアモジュール |
| `Signal` / `In` / `Out` | 信号・ポート |
| `Wire` / `Reg` | モジュール内のローカル信号（ポートではない）。`SetLocal` / `SetLocals` で登録する |
| 幅付きベクタ | `In.UInt(width, name)` / `Out.UInt(width, name)` / `Wire.UInt` / `Reg.UInt` など |

**ローカル信号:** `Wire` は `Comb` 内の `Assign(expr)`、`Reg` は `Seq` 内の `Assign(reset, next)` でだけ代入できる（違反は `DescribeContextException`）。ポートとローカルの名前は合わせて一意。登録していない `Wire` / `Reg` を使うと `SignalDeclarationException`。Emit ではポート一覧の直後に内部 `wire` / `reg` として宣言される（If 内で代入される `Wire` は `reg` 宣言になる）。Sim では `Get` で観測できるが、テストベンチからの `Set` は `SimException`。

---

## 3. モジュール記述のイメージ

```csharp
public sealed class Alu : Module
{
    public In A { get; } = In.UInt(32, "A");
    public In B { get; } = In.UInt(32, "B");
    public In Op { get; } = In.UInt(2, "Op");
    public Out Y { get; } = Out.UInt(32, "Y");

    public Alu()
    {
        SetPorts([A, B, Op, Y]);
    }

    public override void Describe()
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

## 4. 文・演算

### 組み合わせ `Comb`

- 代入は Verilog の連続代入（`assign`）または組み合わせブロック相当
- 同一信号への複数駆動はエラー
- **Emit 形（現行）:** `Switch` → `assign` 三項連鎖（左辺は `output wire`）。各 case は単一 Assign・同一左辺。**最終 case の式が default**（未一致時）。空／複数／非 Assign は emit エラー。`If` → `always @(*)` 手続代入（左辺は `output reg`）。単独の `Assign` は `assign` + `wire`

### 同期 `Seq(clock, reset)`

- 代入は非ブロッキング `<=`
- リセット時の初期値を指定できる

### 制御・演算

| 構文 | 意味 |
|------|------|
| `If` / `Else` | 述語分岐（`Eq` や比較など）。`If(cond, then: …, @else: …)`（`@else` 省略可） |
| `Switch` / `Case` | **セレクタは `Expr`**（`In`/`Signal`/Slice など）と**定数**ケースの多分岐 |
| 比較 | `Eq`/`Neq` メソッドと `< <= > >=`（結果 1 bit）。`==`/`!=` は使わない |
| 算術 `+ -` | 左右は同じ幅。結果も同じ幅（桁あふれは切り捨て） |
| 乗算 `*` | 左右は同じ幅。結果も同じ幅（積の上位は切り捨て）。全幅の積は `a.ZeroExtend(16) * b.ZeroExtend(16)` のように先に広げる |
| ビット `& \| ^` / `Xnor` | 左右は同じ幅。`a ^ b` は Verilog `^`、`a.Xnor(b)` は `~^` |
| 単項 `~ - !` | `~x` はビット反転、`-x` は 2 の補数（どちらも結果は `x` の幅）。`!x` は `x` が 0 なら 1（結果 1 bit。2 ビット以上の `x` は Verilog `~|x` で出る） |
| リダクション | `x.ReduceAnd()` / `ReduceOr()` / `ReduceXor()` / `ReduceNand()` / `ReduceNor()` / `ReduceXnor()`。`x` の全ビットを 1 ビットにたたみ込む（Verilog `&x` `\|x` `^x` `~&x` `~\|x` `~^x`）。例: 0 でないか `x.ReduceOr()`、パリティ `x.ReduceXor()` |
| 論理シフト `<< >>` | 量は定数（`x << 2`）か信号・式（`x << amt`）。結果は左の幅。量が左の幅以上なら 0。算術シフトは未対応 |
| `Slice(hi, lo)` | `x[hi:lo]`（両端 inclusive）。結果幅 `hi - lo + 1` |
| `Concat(...)` | `{a, b, …}`（左が MSB）。結果幅は各幅の和 |
| `SignExtend(toWidth)` / `ZeroExtend(toWidth)` | 符号／ゼロ拡張。`toWidth >=` 元幅。同じ幅なら何もしない（`ZeroExtend` は式そのものを返し、入れ子の演算の括弧も保たれる） |

**幅の規則:** 二項演算と比較は左右の幅が違うと `WidthMismatchException`（シフトの量だけは例外）。幅を揃えるときは `ZeroExtend` / `SignExtend` を明示する（例: `A + B.ZeroExtend(8)`）。入れ子の演算は Emit で括弧付きになる（`(A ^ B) & C`、`~(A & B)`）。単項演算とリダクションは中身の幅に制限がない。

**`Slice` / `SignExtend` の対象:** 信号か信号のスライスに限る（演算・連結・定数・拡張には `UnsupportedSelectException`）。`Slice` の `Slice` は 1 段にまとまり（`x.Slice(15, 8).Slice(3, 0)` は `x[11:8]`）、範囲外は `WidthMismatchException`。`instr.Slice(31, 20).SignExtend(64)` は `{{52{instr[31]}}, instr[31:20]}` と出る。演算の結果を切り出したいときはローカル `Wire` を経由する。

**If と Switch:** 定数ラベルの一択（`Op` や `Instr.Slice(...)` など）は `Switch`、真偽・比較の分岐は `If`。  
`Switch` のセレクタは `Expr`（旧 API の `In` 専用ではない）。case 値は定数のまま。  
`If` の Emit／Sim は Comb と枝内代入が中心。Emit では上記のとおり Switch と If で Verilog の形が異なる（合成可能な wire/reg 対応）。

スライス・連結・拡張は **右辺向け**。インデックスは定数。幅不一致は例外になります。

### メモリ `Mem`

- **1R1W:** 同期読み・同期書き。`addr` は読み書き共用  
- **2R1W:** 同期書き + 組み合わせ読み（`raddr0/1`, `rdata0/1`）  
- **1R1W + `wstrb`:** バイトイネーブル（`wstrb` 幅 = `width / 8`）  

アドレス幅は `ceil(log2(depth))`。同一アドレスの読み書きは初版 **旧値読み**（バイパスなし）。

**Emit 配列名:** optional 引数 `name`（省略可）。省略時は `mem_{rdata}`（2R1W は `mem_{rdata0}`）。明示時はその識別子を Verilog 配列に使う。

### 階層

子モジュールをインスタンス化し、ポート接続する。子は別 `module` として emit されます。

---

## 5. 禁止・非対応

| 禁止 | 理由 |
|------|------|
| ポインタ・参照型の自動回路化 | HDL ではない |
| `async` / `await` | 時間モデルが違う |
| GC オブジェクト | ハードウェアに無い |
| 暗黙のクロック | バグの元 |

---

## 6. 名前付け

- Verilog 識別子として使える名前（ASCII、予約語回避）
- モジュール名の既定はクラス名

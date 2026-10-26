# 消費者ガイド — 他プロジェクトからの利用

CsharpHdl は独立リポジトリです。回路は **消費者側の C# プロジェクト** に `Module` として書き、Emitter で Verilog を出します。

```
CsharpHdl → nuget.org（SharpHdl.Core / SharpHdl.Emit / SharpHdl.Sim）
    ↑ PackageReference
消費者プロジェクト  ← 自前 Module → Emitter → *.v
                     ← または SharpHdl.Sim で C# テスト
```

ISA 固有の記述は CsharpHdl には置きません。消費者側に置いてください。

---

## NuGet（nuget.org）— 推奨

```xml
<PackageReference Include="SharpHdl.Core" Version="0.9.0" />
<PackageReference Include="SharpHdl.Emit" Version="0.9.0" />
<PackageReference Include="SharpHdl.Sim" Version="0.9.0" />
```

`Sim` は回路を xUnit 等で回すときだけ足せば足ります（emit のみなら Core + Emit）。

```bash
dotnet restore
dotnet build
```

公開中の版は [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) / [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit) / [SharpHdl.Sim](https://www.nuget.org/packages/SharpHdl.Sim) を参照してください。

### メンテナー向け: nuget.org への公開

開発の拠点は GitLab（[sharphdl/SharpHDL](https://gitlab.com/sharphdl/SharpHDL)）です。GitHub（`yukari6559/CsharpHdl`）は `main` とタグだけを同期するコピーで、Actions は無効にしています。

版の決定は人手、pack / push は GitLab CI（**Trusted Publishing / OIDC**）です。長期 API キーは不要です。

1. nuget.org で GitLab 用の Trusted Publishing ポリシーを作成済みであること  
   - Namespace path: `sharphdl` / Project path: `SharpHDL` / Environment: `release`（Ref は空のまま。タグで公開するため）  
   - Pattern 例: `SharpHdl.*`（Sim を含むこと）  
2. `main` を公開したい状態にする  
3. タグを打って GitLab に push し、GitHub のコピーにも送る（例: `0.9.0`）

```bash
git push origin main
git tag v0.9.0
git push origin v0.9.0
git push github main v0.9.0
```

`.gitlab-ci.yml` の `publish-nuget` ジョブがテスト → pack（タグの版）→ OIDC で一時キー取得 → nuget.org へ push します。  
同じ版は nuget.org 上で上書きできません。直すときは版を上げて新しいタグを付けてください。

csproj の `<Version>` はローカル表示用です。タグ付きリリースでは CI が `-p:Version=` で上書きします。揃えておくと分かりやすいです。

---

## 開発版パッケージ（未リリースの変更を試す）

`main` でライブラリ（Core / Emit / Sim）に変更が入るたびに、GitLab のパッケージレジストリへ開発版（例: `0.5.1-dev.42`。次の版 + `-dev.` + パイプライン番号）を公開しています。nuget.org の正式版より先に新しい機能を試したいときに使ってください。

```bash
dotnet nuget add source https://gitlab.com/api/v4/projects/87940359/packages/nuget/index.json --name sharphdl-dev
```

```xml
<PackageReference Include="SharpHdl.Core" Version="0.5.1-dev.*" />
```

開発版は予告なく変わります。正式な依存には nuget.org の版を使ってください。

---

## プロジェクト参照（本リポジトリ開発・未公開版の試し用）

```xml
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Core\SharpHdl.Core.csproj" />
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Emit\SharpHdl.Emit.csproj" />
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Sim\SharpHdl.Sim.csproj" />
```

常用の消費者参照は NuGet を推奨します。

---

## 最小の流れ

1. Core / Emit（必要なら Sim）を参照する  
2. 消費者側に `Module` を 1 つ置く  
3. Program またはテストから Emitter を呼び、`.v` を書く — または `Run` / `Settle` / `Advance` でシミュする  
4. 生成物を自分のシミュレーション / FPGA フローへ渡す（emit 利用時）  

書き方の参考: リポジトリの `examples/`（製品 ISA は含みません）。`Sim` は **S2 完了**。比較・If/Else・**Lit.Bits（M3）**・ローカル `Wire` / `Reg`（M4）・XOR / XNOR・論理シフト・乗算・除算・剰余・定数のべき乗 `Pow`・単項演算（`~` `-` `!`、リダクション）・全幅の積 `MulFull`・論理積・論理和 `LogicAnd` / `LogicOr` まで。

**`0.9.0` の変更:**
- 追加: 定数のべき乗 `a.Pow(n)`（Verilog `**`。指数は `int` か定数。信号の指数は `ArgumentException`。結果は底の幅。指数 0 は 1。`0 ** 0` も 1。幅に収まらない値は切り捨て。`int` の指数はシフト量と同じ最小幅で、負の指数は `ArgumentOutOfRangeException`）

**`0.8.0` の変更:**
- 追加: 符号なしの除算・剰余 `a / b` と `a % b`（左右は同じ幅、結果は左の幅。切り捨ては 0 方向。変数の除数も可）。`a / 3` の `3` は左の幅の定数。定数の除数 0 は作る時点で例外。信号や式の除数が 0 のときは Sim が例外。入力の初期値は 0 なので、変数で割る式は値を入れてから実行する

**`0.7.0` の変更:**
- 追加: 全幅の積 `a.MulFull(b)`（左右の幅は違ってよく、結果は幅の和。64 ビットまで）。論理積・論理和 `a.LogicAnd(b)` / `a.LogicOr(b)`（Verilog `&&` / `||`。結果は 1 ビット。0 が偽、0 以外が真。左右の幅は違ってよい）
- 変更: インスタンスのポート接続は 1 行に 1 つ、閉じ `);` は単独の行。0.6.0 までは同じ行にタブ区切り

**`0.6.0` の変更:**
- 追加: 論理シフト `<<` / `>>`、乗算 `*`、単項演算 `~x` / `-x` / `!x`、リダクション `x.ReduceAnd()` / `ReduceOr()` / `ReduceXor()` / `ReduceNand()` / `ReduceNor()` / `ReduceXnor()`
- 修正: `Instance` を使う回路の Verilog で `endmodule` と次の `module` がくっついていた（0.5.0 までは Verilator / Icarus Verilog が受け付けない）。3 段以上の階層で孫モジュールと子のインスタンス行が欠けていた。同じ幅への `ZeroExtend` で入れ子の演算の括弧が消えていた
- 破壊的変更: `OpExpr` / `UnaryExpr` は DSL（演算子・メソッド）からだけ作れる（`new OpExpr { … }` は不可、プロパティは読み取り専用）。`VerilogEmitter.EmitOneModule` / `CollectInstances` と `InstanceVisitor` のコンストラクタの引数が変わった（`VerilogEmitter.Emitter` は変わらない）

**`0.5.0` の注意（挙動の変更）:**
- 二項演算と比較（`+ - & | ^ < <= > >=`、`Eq` / `Neq` / `Xnor`）は左右の幅が同じでないと `WidthMismatchException`。幅を揃えるときは `ZeroExtend` / `SignExtend` を明示する
- `Slice` / `SignExtend` は信号か信号のスライスにだけ付けられる（演算・連結・定数・拡張には `UnsupportedSelectException`）。`Slice` は範囲外で `WidthMismatchException`。`instr.Slice(31, 20).SignExtend(64)` のような即値の取り出しはそのまま書ける
- テストベンチからローカル信号（`Wire` / `Reg`）への `Set` は `SimException`（`Get` は可）。`SetLocal(s)` していない `Wire` / `Reg` を使うと `SignalDeclarationException`

いまの本線は演算子の穴埋めの残り（`Mux`・繰り返し連結、#9）（内部 ROADMAP / tickets 参照）。

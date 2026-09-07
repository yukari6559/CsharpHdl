# 消費者ガイド — 他プロジェクトからの利用

CsharpHdl は **独立リポジトリ** です。OS や CPU リポジトリはこちらを参照します。

## 本線: ライブラリとして使う

回路は **消費者側の C# プロジェクト**に `Module` として書く。  
`dotnet build` で型・DSL API・幅不一致などが効く。これが C# を使う利点の本体。

```
 CsharpHdl (Core / Emit)   ← 下流の ISA を知らない
     ↑ ProjectReference 等
 MyOsProject 等            ← 自前 Module → Emitter → rtl/generated/*.v
```

CLI で examples を吐くのはデモ・CI 用の脇役。本線ではない（[cli-spec.md](cli-spec.md)）。

CsharpHdl のコードや docs に MyOs 固有の ISA を埋め込まない。  
ISA 固有の Module は **消費者側**（例: `MyOsProject/hdl/`）に置く。

---

## 取り込み方（どれか）

### A. プロジェクト参照（推奨・開発初期）

```
workspace/
  CsharpHdl/
  MyOsProject/
```

消費者の csproj:

```xml
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Core\SharpHdl.Core.csproj" />
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Emit\SharpHdl.Emit.csproj" />
```

### B. git submodule

```bash
cd MyOsProject
git submodule add <url-to-CsharpHdl> third_party/CsharpHdl
```

参照パスを submodule 配下の Core / Emit に向ける。

### C. NuGet（後回しで可）

隣ディレクトリ参照で足りるうちは不要。配布を楽にしたくなったら:

```bash
dotnet pack src/SharpHdl.Core
dotnet pack src/SharpHdl.Emit
# 私有フィードまたはローカル nupkg
```

---

## 最小の使い方（確認用）

1. 上記 A で Core / Emit を参照する  
2. 消費者側に空に近い `Module` を 1 つ置く（ポートと簡単な Comb / Seq で可）  
3. 小さな Program またはテストから Emitter を呼び、`.v` を 1 本書く  
4. 生成物を自分のシミュレーション / FPGA / TT フローへ渡す  

例の書き方はリポジトリの `examples/` を参考にする（製品 ISA は置かない）。

---

## MyOsProject での使い方（想定）

1. CsharpHdl の DSL / Emitter を参照できる状態にする  
2. MyOs 側に `hdl/SimpleRisc/` など **CPU 記述**を置く  
3. ビルドまたは小さなホストから `rtl/generated/*.v` を出力  
4. MyOs 側のハードウェアフロー（例: `docs/hardware-path.md`）に乗せる  

MyOs 側ドキュメントは `docs/csharp-hdl.md` が本リポジトリへのポインタになる想定。

---

## 別件で使うとき

1. このリポジトリを clone（または参照）  
2. 自分のリポジトリに Module を書く  
3. Emitter で `.v` を出し、自分のフローへ  

TinyTapeOut 提出物に「CsharpHdl で生成した」と明記し、生成 Verilog をレビューすること。

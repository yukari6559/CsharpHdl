# 消費者ガイド — 他プロジェクトからの利用

CsharpHdl は **独立リポジトリ** です。OS や CPU リポジトリはこちらを参照します。

## 本線: ライブラリとして使う

回路は **消費者側の C# プロジェクト**に `Module` として書く。  
`dotnet build` で型・DSL API・幅不一致などが効く。これが C# を使う利点の本体。

```
 CsharpHdl → nuget.org（SharpHdl.Core / SharpHdl.Emit）
     ↑ PackageReference
 RISC-Sharp / MyOsProject 等   ← 自前 Module → Emitter → *.v
```

推奨の取り込みは **nuget.org**（下記 C）。  
隣ディレクトリの ProjectReference は、pack 前の暫定や本リポジトリ開発用。

CLI で examples を吐くのはデモ・CI 用の脇役（[cli-spec.md](cli-spec.md)）。

CsharpHdl のコードや docs に消費者固有の ISA を埋め込まない。  
ISA 固有の Module は **消費者側**に置く。

宿題チケット: [tickets/riscv-sharp-requests.md](tickets/riscv-sharp-requests.md)（**T1 完了** → 次は T2）。

---

## 取り込み方

### C. NuGet（nuget.org）— 推奨

#### CsharpHdl 側（公開するとき）

1. Core / Emit の csproj にパッケージ用メタ（`PackageId` / `Version` / 説明・ライセンス等）を入れる  
2. pack する（出力先は任意。例: `artifacts/nuget`）  

```bash
dotnet pack src/SharpHdl.Core -c Release -o artifacts/nuget
dotnet pack src/SharpHdl.Emit -c Release -o artifacts/nuget
```

3. nuget.org に push（**API キーはリポジトリに置かない**。環境変数や CI シークレット）  

```bash
dotnet nuget push artifacts/nuget/SharpHdl.Core.*.nupkg --source https://api.nuget.org/v3/index.json --api-key "$NUGET_API_KEY"
dotnet nuget push artifacts/nuget/SharpHdl.Emit.*.nupkg --source https://api.nuget.org/v3/index.json --api-key "$NUGET_API_KEY"
```

同じ `Version` は nuget.org 上で上書きできない。直すときは版を上げる。

ローカルだけの試し push 確認には、一時的にフォルダソースでもよい（本番消費者は nuget.org）。

#### 消費者側（RISC-Sharp 等）

1. ProjectReference を外す  
2. PackageReference を付ける（版は公開したものに合わせる）  

```xml
<PackageReference Include="SharpHdl.Core" Version="0.1.0" />
<PackageReference Include="SharpHdl.Emit" Version="0.1.0" />
```

3. `dotnet restore` → `dotnet build`  

Emit だけ参照して Core が推移的に入るかは Restore 次第。入らなければ Core も明示する。

特殊な `nuget.config` は不要（nuget.org が既定ソース）。

---

### A. プロジェクト参照（本リポジトリ開発向け）

CsharpHdl 本体の開発や、未公開版の試し用。消費者の常用は **C（nuget.org）**。

```
workspace/
  CsharpHdl/
  RISC-Sharp/
```

```xml
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Core\SharpHdl.Core.csproj" />
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Emit\SharpHdl.Emit.csproj" />
```

### B. git submodule

```bash
cd RISC-Sharp
git submodule add <url-to-CsharpHdl> third_party/CsharpHdl
```

参照パスを submodule 配下の Core / Emit に向ける。nuget.org 利用時は通常不要。

---

## 最小の使い方（確認用）

1. C（または暫定 A）で Core / Emit を参照する  
2. 消費者側に空に近い `Module` を 1 つ置く  
3. 小さな Program またはテストから Emitter を呼び、`.v` を 1 本書く  
4. 生成物を自分のシミュレーション / FPGA / TT フローへ渡す  

例の書き方はリポジトリの `examples/` を参考にする（製品 ISA は置かない）。

---

## RISC-Sharp / MyOs での使い方（想定）

1. nuget.org の SharpHdl パッケージを参照する（T1 完了・推奨）  
2. 消費者側に CPU / 周辺の Module を置く  
3. ビルドまたは小さなホストから生成 Verilog を出力  
4. 消費者側のハードウェアフローに乗せる  

---

## 別件で使うとき

1. nuget.org からパッケージを取る（または本リポジトリを clone）  
2. 自分のリポジトリに Module を書く  
3. Emitter で `.v` を出し、自分のフローへ  

TinyTapeOut 提出物に「CsharpHdl で生成した」と明記し、生成 Verilog をレビューすること。

# 消費者ガイド — 他プロジェクトからの利用

CsharpHdl は独立リポジトリです。回路は **消費者側の C# プロジェクト** に `Module` として書き、Emitter で Verilog を出します。

```
CsharpHdl → nuget.org（SharpHdl.Core / SharpHdl.Emit）
    ↑ PackageReference
消費者プロジェクト  ← 自前 Module → Emitter → *.v
```

ISA 固有の記述は CsharpHdl には置きません。消費者側に置いてください。

---

## NuGet（nuget.org）— 推奨

```xml
<PackageReference Include="SharpHdl.Core" Version="0.2.0" />
<PackageReference Include="SharpHdl.Emit" Version="0.2.0" />
```

```bash
dotnet restore
dotnet build
```

公開中の版は [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) / [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit) を参照してください。

### メンテナー向け: nuget.org への公開

版の決定は人手、pack / push は CI です。

1. GitHub リポジトリの Secrets に `NUGET_API_KEY`（nuget.org の API キー）を設定する  
2. `main` を公開したい状態にする  
3. タグを打って push する（例: `0.2.0`）

```bash
git tag v0.2.0
git push origin v0.2.0
```

`Release NuGet` workflow がテスト → pack（タグの版）→ nuget.org へ push します。  
同じ版は nuget.org 上で上書きできません。直すときは版を上げて新しいタグを付けてください。

csproj の `<Version>` はローカル表示用です。タグ付きリリースでは workflow が `-p:Version=` で上書きします。揃えておくと分かりやすいです。

---

## プロジェクト参照（本リポジトリ開発・未公開版の試し用）

```xml
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Core\SharpHdl.Core.csproj" />
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Emit\SharpHdl.Emit.csproj" />
```

常用の消費者参照は NuGet を推奨します。

---

## 最小の流れ

1. Core / Emit を参照する  
2. 消費者側に `Module` を 1 つ置く  
3. Program またはテストから Emitter を呼び、`.v` を書く  
4. 生成物を自分のシミュレーション / FPGA フローへ渡す  

書き方の参考: リポジトリの `examples/`（製品 ISA は含みません）。

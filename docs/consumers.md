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
<PackageReference Include="SharpHdl.Core" Version="0.1.0" />
<PackageReference Include="SharpHdl.Emit" Version="0.1.0" />
```

```bash
dotnet restore
dotnet build
```

公開中の版は [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) / [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit) を参照してください。

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

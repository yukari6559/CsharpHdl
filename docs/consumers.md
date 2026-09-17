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
<PackageReference Include="SharpHdl.Core" Version="0.3.4" />
<PackageReference Include="SharpHdl.Emit" Version="0.3.4" />
<PackageReference Include="SharpHdl.Sim" Version="0.3.4" />
```

`Sim` は回路を xUnit 等で回すときだけ足せば足ります（emit のみなら Core + Emit）。

```bash
dotnet restore
dotnet build
```

公開中の版は [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) / [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit) / [SharpHdl.Sim](https://www.nuget.org/packages/SharpHdl.Sim) を参照してください。

### メンテナー向け: nuget.org への公開

版の決定は人手、pack / push は CI（**Trusted Publishing / OIDC**）です。長期 API キーは不要です。

1. nuget.org で Trusted Publishing ポリシーを作成済みであること  
   - Repository Owner: `yukari6559` / Repository: `CsharpHdl` / Workflow: `release-nuget.yml`  
   - Pattern 例: `SharpHdl.*`（Sim を含むこと）  
2. `main` を公開したい状態にする  
3. タグを打って push する（例: `0.3.4`）

```bash
git push origin main
git tag v0.3.4
git push origin v0.3.4
```

`Release NuGet` workflow がテスト → pack（タグの版）→ OIDC で一時キー取得 → nuget.org へ push します。  
同じ版は nuget.org 上で上書きできません。直すときは版を上げて新しいタグを付けてください。

リポジトリが Private のとき、ポリシーは初回成功 publish まで一時有効（数日）になることがあります。一度成功すれば恒久化されます。

csproj の `<Version>` はローカル表示用です。タグ付きリリースでは workflow が `-p:Version=` で上書きします。揃えておくと分かりやすいです。

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

書き方の参考: リポジトリの `examples/`（製品 ISA は含みません）。`Sim` は S1（Comb/Seq）＋ Instance / 1R1W まで。**S2 完了後**に MidiSynth 向け原語（M1–M5）が本線になる（内部 ROADMAP / tickets 参照）。

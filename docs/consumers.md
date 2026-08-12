# 消費者ガイド — 他プロジェクトからの利用

CsharpHdl は **独立リポジトリ** です。OS や CPU リポジトリはこちらを参照します。

## 依存の向き（重要）

```
 CsharpHdl          ← 下流を知らない
     ↑
 MyOsProject        ← 生成 Verilog を rtl/generated に置く
 OtherCpuProject
 TinyTapeOutRepo
```

CsharpHdl のコードや docs に MyOs 固有の ISA を埋め込まない。  
ISA 固有の Module は **消費者側**（例: `MyOsProject/rtl/` または `MyOsProject/hdl/`）に置く。

---

## 取り込み方（どれか）

### A. プロジェクト参照（開発初期向け）

```
workspace/
  CsharpHdl/
  MyOsProject/
```

MyOs の csproj:

```xml
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Core\SharpHdl.Core.csproj" />
<ProjectReference Include="..\CsharpHdl\src\SharpHdl.Emit\SharpHdl.Emit.csproj" />
```

### B. git submodule

```bash
cd MyOsProject
git submodule add <url-to-CsharpHdl> third_party/CsharpHdl
```

### C. NuGet（Phase 4 以降）

```bash
dotnet pack src/SharpHdl.Core
# 私有フィードまたはローカル nupkg
```

---

## MyOsProject での使い方（想定）

1. CsharpHdl で DSL / Emitter を完成させる  
2. MyOs 側に `hdl/SimpleRisc/` など **CPU 記述**を置く（CsharpHdl への参照付き）  
3. ビルドで `rtl/generated/*.v` を出力  
4. [MyOsProject/docs/hardware-path.md](../../MyOsProject/docs/hardware-path.md) のフローに乗せる  

MyOs 側ドキュメントは `docs/csharp-hdl.md` が本リポジトリへのポインタになる。

---

## 別件で使うとき

1. このリポジトリを clone  
2. `examples/` を参考に自分の Module を追加（別リポジトリでも可）  
3. emit → 自分の FPGA / TT フローへ  

TinyTapeOut 提出物に「CsharpHdl で生成した」と明記し、生成 Verilog をレビューすること。

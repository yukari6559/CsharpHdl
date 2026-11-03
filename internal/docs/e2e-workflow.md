# ボード別の完走手順

SharpHDL が Verilog を出したあと、実機へ書くまでの記録。最初のボードは Sipeed Tang Primer 25K。Gowin IDE は使わない。

2026-10-14 に、手書きの点滅回路で READY LED の点滅まで確認した。確認に使った一式はリポジトリの外（`tang25k-blink`）にある。この文書が SharpHDL 側の記録である。

## Tang Primer 25K

FPGA は Gowin GW5A-25A。パッケージは `GW5A-LV25MG121NES`。電源は Dock の USB-C で、書き込みも同じ口から行う。

| 段階 | ツール | このボードでの形 |
|------|--------|------------------|
| シミュレーション | GHDL 6.0.0 | 確認用の回路は VHDL。SharpHDL の出力は Verilog なので、ここは GHDL を通さない |
| 合成 | Homebrew の `yowasp-yosys` | `synth_gowin -family gw5a` |
| 配置配線 | nextpnr-himbaechel 0.11.1 | デバイスは `GW5A-25A` だけを有効にしてソースからビルドした |
| ビットストリーム | Apicula 0.34 の `gowin_pack` | `-d GW5A-25A` |
| 書き込み | openFPGALoader（Homebrew 1.1.1） | `-b tangprimer25k`。書き込み先は FPGA の SRAM |

Yosys は VHDL を読まない。確認用の VHDL は `ghdl --synth` で Verilog にしてから `synth_gowin` に渡す。SharpHDL が出す Verilog は、その変換なしで `yowasp-yosys` に渡す。

### 使わないもの

- Homebrew の `nextpnr-ice40`。Lattice iCE40 専用で、この FPGA のチップデータを持たない。
- PyPI の `yowasp-nextpnr-gowin`。Apicula 0.15 固定で、GW5A-25A のチップデータがない。
- システムに入れた素の `yosys`。合成は `yowasp-yosys` を使う。

配置配線のバイナリは nextpnr 0.11.1 を、`ARCH=himbaechel`、`HIMBAECHEL_UARCH=gowin`、`HIMBAECHEL_GOWIN_DEVICES=GW5A-25A` でビルドする。`gowin_pack` は `apycula==0.34` を入れた仮想環境のものを使う。

### デバイスとピン

| 項目 | 値 |
|------|-----|
| nextpnr `--device` | `GW5A-LV25MG121NES` |
| `gowin_pack -d` | `GW5A-25A` |
| クロック | オンボード 50 MHz、ピン E2 |
| LED | READY、ピン E8 |
| 制約 | `.cst`。タイミング用の `.sdc` は Gowin 向けで、nextpnr 0.11.1 は読まない |

E2 と E8 は専用ピンを兼ねる。`gowin_pack` では `--cpu_as_gpio` と `--ready_as_gpio` を付ける。nextpnr 0.11.1 の Gowin 向けオプションにあるのは `--vopt sspi_as_gpio` までなので、配置配線にはそれだけを渡す。

### 書き込み

Dock の USB-C で電源が入っている状態で、SRAM に書く。電源を切るとコンフィギュレーションは消える。Flash への書き込みは、この確認では使っていない。

```bash
openFPGALoader -b tangprimer25k build/blink.fs
```

成功するとコア基板の READY LED が約 1 秒周期で点滅する。POWER は電源、DONE はコンフィギュレーション完了。

## まだ書いていないボード

他のボードの手順は、同じ形でこの文書に足す。

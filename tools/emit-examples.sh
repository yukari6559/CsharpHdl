#!/usr/bin/env bash
# Builds each examples/<dir> on its own and writes its modules' Verilog to OUT_DIR as <dir>-<Type>.v.
# usage: tools/emit-examples.sh [OUT_DIR]   (default artifacts/examples)
set -euo pipefail

out_dir="${1:-artifacts/examples}"
mkdir -p "$out_dir"

for dir in examples/*/; do
	name=$(basename "$dir")
	ls "$dir"*.cs > /dev/null 2>&1 || continue
	echo "== examples/$name"
	dotnet run --project tools/ExampleVerilog -c Release -p:Example="$name" -- "$out_dir" "$name"
done

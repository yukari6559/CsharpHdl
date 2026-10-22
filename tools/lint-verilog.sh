#!/bin/sh
# Lints every .v file in a directory with Verilator and Icarus Verilog.
# Errors fail the script; warnings are printed. Writes a JUnit report so each
# file shows up in the merge request test report.
# Usage: tools/lint-verilog.sh <dir> [junit-path]
set -u

dir="$1"
junit="${2:-$dir/verilog-lint.junit.xml}"
failures=0
total=0
cases=""

xml_escape() {
	sed -e 's/&/\&amp;/g' -e 's/</\&lt;/g' -e 's/>/\&gt;/g'
}

for file in "$dir"/*.v; do
	name=$(basename "$file" .v)
	total=$((total + 1))
	verilator_out=$(verilator --lint-only -Wall -Wno-fatal "$file" 2>&1)
	verilator_status=$?
	iverilog_out=$(iverilog -g2005 -Wall -o /dev/null "$file" 2>&1)
	iverilog_status=$?

	if [ -n "$verilator_out" ] || [ -n "$iverilog_out" ]; then
		echo "=== $name"
		[ -n "$verilator_out" ] && echo "$verilator_out"
		[ -n "$iverilog_out" ] && echo "iverilog: $iverilog_out"
	fi

	if [ "$verilator_status" -ne 0 ] || [ "$iverilog_status" -ne 0 ]; then
		failures=$((failures + 1))
		message=$(printf '%s\n%s\n' "$verilator_out" "$iverilog_out" | xml_escape)
		cases="$cases<testcase classname=\"verilog-lint\" name=\"$name\"><failure message=\"lint error\">$message</failure></testcase>"
	else
		cases="$cases<testcase classname=\"verilog-lint\" name=\"$name\"/>"
	fi
done

printf '<?xml version="1.0" encoding="UTF-8"?>\n<testsuites><testsuite name="verilog-lint" tests="%s" failures="%s">%s</testsuite></testsuites>\n' \
	"$total" "$failures" "$cases" > "$junit"

echo "verilog-lint: $total files, $failures with errors"
[ "$failures" -eq 0 ]

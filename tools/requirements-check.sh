#!/bin/sh
# Checks the requirements map against JUnit files. Requirements are YouTrack issues (SHDL-N, tag "requirement").
# A requirement passes when every listed test exists and none of its test cases failed.
# A listed test that is missing counts as a failure, so renamed tests are noticed before merge.
# Exits 1 when any requirement fails.
# Usage: tools/requirements-check.sh <map.tsv> <junit.xml>...
set -u

map="$1"
shift
# Paths without spaces (CI artifact paths); kept in a variable because the loop reuses "$@".
junits="$*"

# Without the inputs every requirement would look failed; fail the job instead.
for junit in $junits; do
	if [ ! -f "$junit" ]; then
		echo "missing JUnit file: $junit"
		exit 1
	fi
done

# Prints "<found> <failed>" for one test name or "suite:<classname>".
count() {
	key="$1"
	shift
	mode=name
	case "$key" in
		suite:*) mode=suite; key="${key#suite:}" ;;
	esac
	KEY="$key" MODE="$mode" perl -0777 -ne '
		while (/<testcase\b(.*?)(?:\/>|>(.*?)<\/testcase>)/gs) {
			my ($attrs, $body) = ($1, defined $2 ? $2 : "");
			my $hit = $ENV{MODE} eq "suite"
				? $attrs =~ /classname="\Q$ENV{KEY}\E"/
				: $attrs =~ /\bname="(?:[^"]*\.)?\Q$ENV{KEY}\E[("]/;
			if ($hit) { $found++; $failed++ if $body =~ /<failure\b/; }
		}
		END { printf "%d %d\n", $found || 0, $failed || 0 }
	' "$@"
}

total=0
failures=0
while IFS='	' read -r key tests; do
	case "$key" in
		'' | \#*) continue ;;
	esac
	total=$((total + 1))
	case "$key" in
		SHDL-[0-9]*) ;;
		*)
			echo "$key: not a YouTrack key (SHDL-N)"
			failures=$((failures + 1))
			continue
			;;
	esac
	status=passed
	for test in $tests; do
		# shellcheck disable=SC2086
		set -- $(count "$test" $junits)
		found="$1"
		failed="$2"
		if [ "$found" -eq 0 ] || [ "$failed" -ne 0 ]; then
			echo "$key: $test found=$found failed=$failed"
			status=failed
		fi
	done
	echo "$key: $status"
	if [ "$status" != passed ]; then
		failures=$((failures + 1))
	fi
done < "$map"

echo "requirements: $total checked, $failures failed"
[ "$failures" -eq 0 ]

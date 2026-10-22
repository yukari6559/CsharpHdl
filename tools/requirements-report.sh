#!/bin/sh
# Builds a GitLab requirements report from JUnit files.
# A requirement passes when every listed test exists and none of its test cases failed.
# A listed test that is missing counts as a failure, so renamed tests are noticed.
# Usage: tools/requirements-report.sh <map.tsv> <out.json> <junit.xml>...
set -u

map="$1"
out="$2"
shift 2
# Paths without spaces (CI artifact paths); kept in a variable because the loop reuses "$@".
junits="$*"

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

entries=""
while IFS='	' read -r iid tests; do
	case "$iid" in
		'' | \#*) continue ;;
	esac
	status=passed
	for test in $tests; do
		# shellcheck disable=SC2086
		set -- $(count "$test" $junits)
		found="$1"
		failed="$2"
		if [ "$found" -eq 0 ] || [ "$failed" -ne 0 ]; then
			echo "requirement $iid: $test found=$found failed=$failed"
			status=failed
		fi
	done
	echo "requirement $iid: $status"
	entries="$entries${entries:+,}\"$iid\":\"$status\""
done < "$map"

printf '{%s}\n' "$entries" > "$out"
cat "$out"

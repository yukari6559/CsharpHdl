#!/bin/sh
# Writes release notes for a tag from Conventional commit subjects since the previous tag.
# Needs full history and tags (GIT_DEPTH: 0 in CI).
# Usage: tools/release-notes.sh <tag>
set -eu

tag="$1"
version="${tag#v}"
project_url="https://gitlab.com/sharphdl/SharpHDL"
prev=$(git describe --tags --abbrev=0 --match 'v[0-9]*' "$tag^" 2>/dev/null || true)
range="${prev:+$prev..}$tag"

section() {
	title="$1"
	pattern="$2"
	lines=$(git log --no-merges --format='%s (%h)' "$range" | grep -E "$pattern" || true)
	if [ -n "$lines" ]; then
		printf '\n### %s\n\n' "$title"
		echo "$lines" | sed 's/^/- /'
	fi
}

echo "SharpHdl $version"
section "Breaking changes" '^[a-z]+(\([^)]*\))?!: '
section "Features" '^feat(\([^)]*\))?: '
section "Fixes" '^fix(\([^)]*\))?: '
section "Other changes" '^(refactor|perf|build|ci|docs|test|chore|style)(\([^)]*\))?: '

printf '\n### Packages\n\n'
for p in Core Emit Sim; do
	echo "- [SharpHdl.$p $version](https://www.nuget.org/packages/SharpHdl.$p/$version)"
done

if [ -n "$prev" ]; then
	printf '\nFull diff: [%s...%s](%s/-/compare/%s...%s)\n' "$prev" "$tag" "$project_url" "$prev" "$tag"
fi

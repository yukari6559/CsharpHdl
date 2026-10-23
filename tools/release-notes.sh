#!/bin/sh
# Writes release notes for a tag: one line per merge request merged since the previous tag,
# with the issues it closed. Breaking changes, features and fixes are listed; the other
# Conventional types (ci, docs, test, chore, build, refactor, ...) and feat / fix scoped to
# ci, build, deps, docs or test go into a collapsed section.
# Reads the first-parent history: GitLab merge commits ("See merge request …!N") and the
# GitHub pull request merges imported with the same numbers ("Merge pull request #N").
# Needs full history and tags (GIT_DEPTH: 0 in CI).
# Usage: tools/release-notes.sh <tag>
set -eu

tag="$1"
version="${tag#v}"
project_url="https://gitlab.com/sharphdl/SharpHDL"
prev=$(git describe --tags --abbrev=0 --match 'v[0-9]*' "$tag^" 2>/dev/null || true)
range="${prev:+$prev..}$tag"

echo "SharpHdl $version"

git log --first-parent --format='%x1e%h%x1f%s%x1f%b' "$range" | awk '
BEGIN { RS = "\036"; FS = "\037" }
NF >= 2 {
	hash = $1; subject = $2; body = $3
	ref = ""
	if (subject ~ /^Merge branch / || subject ~ /^Merge pull request #[0-9]+/) {
		title = first_line(body)
		if (match(body, /See merge request [^ \n]*![0-9]+/)) {
			s = substr(body, RSTART, RLENGTH); sub(/.*!/, "", s); ref = "!" s
		} else if (match(subject, /#[0-9]+/)) {
			ref = "!" substr(subject, RSTART + 1, RLENGTH - 1)
		}
	} else {
		title = subject
		ref = hash
	}
	if (title == "") next

	issues = ""
	rest = body
	while (match(rest, /([Cc]lose[sd]?|[Ff]ix(e[sd])?|[Rr]esolve[sd]?) #[0-9]+/)) {
		m = substr(rest, RSTART, RLENGTH); sub(/.*#/, "#", m)
		issues = issues (issues == "" ? "" : ", ") m
		rest = substr(rest, RSTART + RLENGTH)
	}
	refs = issues (issues != "" && ref != "" ? ", " : "") ref

	type = ""; scope = ""; breaking = 0; text = title
	if (match(title, /^[a-z]+(\([^)]*\))?!?: /)) {
		prefix = substr(title, 1, RLENGTH); text = substr(title, RLENGTH + 1)
		breaking = (prefix ~ /!: $/)
		type = prefix; sub(/[(!:].*/, "", type)
		if (match(prefix, /\([^)]*\)/)) scope = substr(prefix, RSTART + 1, RLENGTH - 2)
	}
	line = "- " (scope != "" ? scope ": " : "") text (refs != "" ? " (" refs ")" : "")
	internal = (scope ~ /^(ci|build|deps|docs|test)$/)

	if (breaking) b = b line "\n"
	else if (type == "feat" && !internal) f = f line "\n"
	else if (type == "fix" && !internal) x = x line "\n"
	else { o = o line "\n"; on++ }
}
function first_line(s,   n, i, lines) {
	n = split(s, lines, "\n")
	for (i = 1; i <= n; i++) if (lines[i] != "") return lines[i]
	return ""
}
END {
	if (b != "") printf "\n### Breaking changes\n\n%s", b
	if (f != "") printf "\n### Features\n\n%s", f
	if (x != "") printf "\n### Fixes\n\n%s", x
	if (o != "") printf "\n<details><summary>Other changes (%d)</summary>\n\n%s\n</details>\n", on, o
}
'

printf '\n### Packages\n\n'
for p in Core Emit Sim; do
	echo "- [SharpHdl.$p $version](https://www.nuget.org/packages/SharpHdl.$p/$version)"
done

if [ -n "$prev" ]; then
	printf '\nFull diff: [%s...%s](%s/-/compare/%s...%s)\n' "$prev" "$tag" "$project_url" "$prev" "$tag"
fi

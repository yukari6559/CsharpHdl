## Version

<!-- vX.Y.Z and why (features merged since the last tag, breaking changes). -->

## Before the bump MR

- [ ] Milestone / epic items for this version are merged or moved
- [ ] main pipeline is green (scheduled run included)
- [ ] Try the latest dev package (`X.Y.Z-dev.N`) from a consumer if the change is user-facing

## Bump MR (`chore/<issue>-bump-X.Y.Z`)

- [ ] `<Version>` in SharpHdl.Core / Emit / Sim csproj
- [ ] Install snippets in README.md, docs/consumers.md and the package READMEs
- [ ] Behaviour changes noted in README.md / docs/consumers.md
- [ ] Commit subject `chore: bump packages to X.Y.Z`

## Tag

- [ ] `git tag vX.Y.Z` on the merge commit, `git push origin vX.Y.Z`
- [ ] `publish-nuget` and `release` jobs green
- [ ] Packages visible on nuget.org; GitLab Release notes read well
- [ ] `git push github main vX.Y.Z`

/label ~chore

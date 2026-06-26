# Branch Protection

Recommended `main` protection after the first green CI run:

- Require pull request before merging.
- Require approvals from at least one maintainer.
- Require status checks:
  - `CI / .NET 8.0.x`
  - `CI / .NET 10.0.x`
- Require branches to be up to date before merging.
- Require conversation resolution.
- Restrict direct pushes to release maintainers.
- Allow administrators to bypass only for emergency release recovery.

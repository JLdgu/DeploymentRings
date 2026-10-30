# Preparing a release

1. Make sure the changes intended for the release are merged into `main`.
2. Choose a unique release version. The version comes from the Git tag; use `vMAJOR.MINOR.PATCH`, optionally with a prerelease suffix such as `v1.2.3-rc.1`.
3. Check that the CI workflow passes for the commit being released.
4. Create and push the release tag from that commit:

   ```powershell
   git checkout main
   git pull
   git tag -a v1.2.3 -m "Release v1.2.3"
   git push origin v1.2.3
   ```

5. The **Release** GitHub Actions workflow runs tests, publishes the Windows application, packages it with Velopack, and creates a GitHub release with the generated packages.
6. Review the workflow run and the published release to confirm the version and assets are correct, then add or edit the release notes on GitHub.

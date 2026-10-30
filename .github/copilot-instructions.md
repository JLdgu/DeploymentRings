# Copilot instructions

- Add every non-ignored file to the solution unless it is included implicitly by convention.
- Put shared MSBuild configuration in `Directory.Build.props` and ensure the solution uses it.
- Use Central Package Management for NuGet dependencies, with package versions centrally managed.
- Use file-scoped namespaces in all C# projects.
- Use TUnit for all test projects.

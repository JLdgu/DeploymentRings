# Copilot instructions

- Add all non-ignored files and folders to the solution unless they are included implicitly by convention; organize solution items under matching solution folders.
- Put shared MSBuild configuration in `Directory.Build.props` and ensure the solution uses it.
- Use Central Package Management for NuGet dependencies, with package versions centrally managed.
- Use file-scoped namespaces in all C# projects.
- Use TUnit for all test projects.

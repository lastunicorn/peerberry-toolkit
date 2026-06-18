# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Build the solution
dotnet build ./PeerBerry.Toolkit.slnx -c Release --configfile ./nuget.config

# Run the demo (place a transactions.xlsx in the demo project directory first)
dotnet run --project sources/PeerBerry.Toolkit.Demo
```

There are no automated tests in this repository yet.

## Publishing

NuGet publishing is triggered by pushing a `v*.*.*` tag (e.g. `v1.2.3`). Version is injected at build time — do not commit version bumps to `Directory.Build.props`; the CI pipeline sets it from the tag.

## Architecture

This is a two-layer .NET 8 library that parses PeerBerry statement `.xlsx` exports:

- **Public API** (`sources/PeerBerry.Toolkit/`): `TransactionsDocument` is the entry point — callers use its static `LoadFromFileAsync` / `LoadAsync` methods. It owns `TransactionsSection` (header + list of `TransactionRecord`s) and the domain types (`TransactionType`, `LoanStatus`, `Currency`).

- **Internal parser** (`sources/PeerBerry.Toolkit/Xlsx/XlsxTransactionsDocument.cs`): Uses `DocumentFormat.OpenXml` to open the stream. It locates the sheet by matching the name pattern `"Investor {id} transactions"`, resolves shared strings, and maps columns A–H to `TransactionRecord` properties row by row.

- **Demo** (`sources/PeerBerry.Toolkit.Demo/`): Console app that reads `transactions.xlsx` from the working directory and prints a table via `DustInTheWind.ConsoleTools`.

`TransactionType`, `LoanStatus`, and `Currency` are all `sealed record class` value objects with implicit `string` conversions. Known values are exposed as static fields but the types accept any string, so unknown values from new PeerBerry exports won't throw.

**Namespace note:** `Currency.cs` is currently in the `DustInTheWind.Mintos.Toolkit` namespace (copied from a sibling project). `TransactionRecord.cs` has a corresponding `using DustInTheWind.Mintos.Toolkit;`. Both should eventually be moved to `DustInTheWind.PeerBerry.Toolkit`.

XML documentation is only required on public types (those shipped in the NuGet package). Internal types like `XlsxTransactionsDocument` must not have XML docs.

## Code Conventions

- No `var` — use explicit types everywhere.
- LINQ lambda parameters are named `x`.
- Prefer `new()` for object instantiation.
- Object initializers with multiple properties: one property per line.
- No braces for single-line `if`, `for`, or `using` bodies.
- Test naming pattern: `Having<...>_When<...>_Then<...>`.
- One test file per public method/constructor; test files grouped in a `<ClassName>Tests/` directory.

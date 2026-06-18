# PeerBerry Toolkit

[![GitHub Repo](https://img.shields.io/badge/github-repo-blue?logo=github)](https://github.com/lastunicorn/peerberry-toolkit) [![GitHub Build](https://img.shields.io/github/actions/workflow/status/lastunicorn/peerberry-toolkit/build-master.yml?logo=github)](https://github.com/lastunicorn/peerberry-toolkit/actions/workflows/build-master.yml) [![NuGet Version](https://img.shields.io/nuget/v/DustInTheWind.PeerBerry.Toolkit?logo=nuget)](https://www.nuget.org/packages/DustInTheWind.PeerBerry.Toolkit) [![NuGet Downloads](https://img.shields.io/nuget/dt/DustInTheWind.PeerBerry.Toolkit?logo=nuget)](https://www.nuget.org/packages/DustInTheWind.PeerBerry.Toolkit)

`PeerBerry Toolkit` is a .NET library for parsing `.xlsx` files exported from the PeerBerry.

**PeerBerry** is a loan investment platform.

- https://peerberry.com

## Installation

Package Manager:

```powershell
Install-Package DustInTheWind.PeerBerry.Toolkit
```

.NET CLI:

```bash
dotnet add package DustInTheWind.PeerBerry.Toolkit
```

## Runtime Requirements

- Library target framework: `.NET 8.0` (`net8.0`)

## Features

- **Parse PeerBerry Statement Documents** - Load and parse .xlsx files exported directly from the PeerBerry platform.

## Quick Start

### a) Export the Statement File

In PeerBerry web application:

1. Log in.
2. Select **Statement** item from the top-right menu.
3. Scroll to **Transactions** section.
4. Select the date interval you need.
5. Click **Download transactions** button

You will get a .xlsx spreadsheet file containing transaction rows that can be parsed with this toolkit.

### b) Parse the Exported Document

```csharp
using DustInTheWind.PeerBerry.Toolkit;

TransactionsDocument document = TransactionsDocument.LoadFromFile("transactions.xlsx");

foreach (TransactionRecord transactionRecord in document.Transactions)
{
	...
}
```

## Spreadsheet Statement Document

Each row is mapped to a `TransactionRecord` with the following columns:

| Spreadsheet Column | Type     | TransactionRecord Property | Description                                         |
|-----------------|----------|--------------------------|-----------------------------------------------------|
| `Id`         | `string` | `Id`                  |              |
| `Date` | `DateTime` | `Date`     |             |
| `Type`    | `TransactionType` | `Type`            |  |
| `Amount` | `decimal` | `Amount`         |                              |
| `Currency` | `Currency` | `Currency`        |          |
| `Loan Id` | `string` | `LoanId`        |                 |
| `Country` | `string` | `Country`     |  |
| `Loan Status` | `LoanStatus` | `Loan Status` | |

## Demo Project

The repository includes a sample CLI project in `sources/PeerBerry.Toolkit.Demo` that demonstrates:

- reading `transactions.xlsx`
- printing parsed data.

You can use this project as a reference implementation for your own importer/exporter tools.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

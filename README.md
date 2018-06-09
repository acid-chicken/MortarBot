# MortarBot

A Discord bot for Robocraft database.

[![CircleCI](https://img.shields.io/circleci/token/99773e659534bdfb0ae232268fd203ba230a7406/project/github/acid-chicken/MortarBot.svg?logo=circleci&colorA=161616&colorB=04aa51&style=for-the-badge)](https://circleci.com/gh/acid-chicken/Mortarbot)
[![Discord](https://img.shields.io/discord/448139568924065792.svg?logo=discord&colorA=697ec4&colorB=7289da&style=for-the-badge)](https://discord.gg/43cH7nk)
[![BitZeny](https://zny.pw/badge/ZtipUCycA38u2b8EB1MkbAJdCKDVoShZVS?style=for-the-badge&colorB=007ec6)](https://zny.pw/insight/address/ZtipUCycA38u2b8EB1MkbAJdCKDVoShZVS)
[![Monacoin](https://img.shields.io/badge/dynamic/json.svg?url=https%3A%2F%2Fmona.chainsight.info%2Fapi%2Faddr%2FMTipLaw4F6HfZPHB3rZApE6ThCMfannr5N&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAxOTg0IDE3ODMiPg0KPHBhdGggZmlsbD0iI2ZmZiIgZD0iTTk4OSAxMjI1bDEwNi0xODBIODg2bDEwMyAxODB6Ii8%2BDQo8cGF0aCBmaWxsPSIjZmZmIiBkPSJNMTc0MCA2MzVMMTU5OSAwbC0yNzggNDM3Yy0yMTYtNTItNDQxLTUyLTY1NyAwTDM4NyAwIDI0NSA2MzVDOTMgNzU3IDAgOTE2IDAgMTA5MWMwIDM4MiA0NDQgNjkyIDk5MiA2OTJzOTkyLTMxMCA5OTItNjkyYzAtMTc1LTkyLTMzNC0yNDQtNDU2ek00NDEgOTI2aC02NmwxNjEtMTg2aDExOEw0NDEgOTI2em01NDYgNDU0TDcxMyA4OThsNjgtMzkgNjEgMTA4aDMwMGw2My0xMDggNjggMzktMjg2IDQ4MnptNTU3LTQ1NGwtMjEzLTE4NmgxMTlsMTYxIDE4NmgtNjd6Ii8%2BDQo8L3N2Zz4NCg%3D%3D&label=tip&query=%24.balance&suffix=%20mona&style=for-the-badge&colorB=007ec6)](https://mona.chainsight.info/address/MTipLaw4F6HfZPHB3rZApE6ThCMfannr5N)

## Installation

1. Install [.NET Core SDK](https://dot.net/sdk) 2.1.300 or above.
2. Execute `dotnet user-secrets --project src/MortarBot/MortarBot.csproj set 'Discord:Token' '> YOUR BOT TOKEN GOES HERE <'`
3. Execute `dotnet run --project src/MortarBot/MortarBot.csproj`

## Configuration

You can create *appsettings.json* to customize bot configurations.

**Example:**

```json
{
  "Discord": {
    "Clock": {
      "Interval": 15000
    },
    "Command": {
      "Prefix": ";/"
    },
    "Delete": {
      "Reaction": [
        "\u23f9",
        "\u2611",
        "\u26d4",
        "\u2705",
        "\u2714",
        "\u274c",
        "\u274e",
        "\ud83c\udd97",
        "\ud83d\uddd1",
        "\ud83d\udeab",
        "\ud83d\uded1"
      ]
    }
  },
  "Log": {
    "Level": 0
  }
}
```

## License

[MIT License](https://github.com/acid-chicken/MortarBot/blob/master/LICENSE)

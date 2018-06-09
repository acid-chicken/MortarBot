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
      "Interval": 15000,
      "TimeZone": {
        "America/Argentina/Buenos_Aires": "\ud83c\udde6\ud83c\uddf7 Buenos Aires, Argentina",
        "Australia/Sydney": "\ud83c\udde6\ud83c\uddfa Sydney, Australia",
        "Europe/Minsk": "\ud83c\udde7\ud83c\uddfe Brussels, Belarus",
        "Europe/Brussels": "\ud83c\udde7\ud83c\uddea Brussels, Belgium",
        "America/Sao_Paulo": "\ud83c\udde7\ud83c\uddf7 Sao Paulo, Brazil",
        "America/Toronto": "\ud83c\udde8\ud83c\udde6 Toronto, Canada",
        "Asia/Shanghai": "\ud83c\udde8\ud83c\uddf3 Shanghai, China",
        "Europe/Copenhagen": "\ud83c\udde9\ud83c\uddf0 Copenhagen, Denmark",
        "Europe/Paris": "\ud83c\uddeb\ud83c\uddf7 Paris, France",
        "Europe/Berlin": "\ud83c\udde9\ud83c\uddea Berlin, Germany",
        "Europe/London": "\ud83c\uddec\ud83c\udde7 London, Great Britain",
        "Asia/Almaty": "\ud83c\uddf0\ud83c\uddff Almaty, Kazakhstan",
        "Atlantic/Reykjavik": "\ud83c\uddee\ud83c\uddf8 Reykjavik, Iceland",
        "Europe/Dublin": "\ud83c\uddee\ud83c\uddea Dublin, Ireland",
        "Europe/Rome": "\ud83c\uddee\ud83c\uddf9 Rome, Italy",
        "Asia/Tokyo": "\ud83c\uddef\ud83c\uddf5 Tokyo, Japan",
        "Europe/Amsterdam": "\ud83c\uddf3\ud83c\uddf1 Amsterdam, Netherlands",
        "Pacific/Auckland": "\ud83c\uddf3\ud83c\uddff Auckland, New Zealand",
        "Europe/Warsaw": "\ud83c\uddf5\ud83c\uddf1 Warsaw, Poland",
        "Asia/Seoul": "\ud83c\uddf0\ud83c\uddf7 Seoul, South Korea",
        "Europe/Moscow": "\ud83c\uddf7\ud83c\uddfa Moscow, Russia",
        "Europe/Madrid": "\ud83c\uddea\ud83c\uddf8 Madrid, Spain",
        "Europe/Stockholm": "\ud83c\uddf8\ud83c\uddea Stockholm, Sweden",
        "Europe/Istanbul": "\ud83c\uddf9\ud83c\uddf7 Istanbul, Turkish",
        "Europe/Kiev": "\ud83c\uddfa\ud83c\udde6 Kiev, Ukraine",
        "America/New_York": "\ud83c\uddfa\ud83c\uddf8 New York, USA"
      }
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

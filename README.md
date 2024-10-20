# maritaca-researcher

maritaca-researcher is a c# console app that mashes up sabiá-3 and tavily to give Maritaca web search capabilities.

## Features

- Talks to sabiá-3, maritaca's flagship LLM
- Searches the web using tavily's api
- Summarizes search results automatically
- Lets you chat with it through the command line

## Before you start

You'll need:
- .net 6.0 or newer
- a maritaca ai api key
- a tavily search api key

## Setting it up

1. clone the code:
   ```
   git clone https://github.com/aquiffoo/maritaca-researcher.git
   ```

2. enter into the project folder:
   ```
   cd maritaca-researcher
   ```

3. put your api keys in `config.txt`:
   ```
   your_maritaca_api_key
   your_tavily_api_key
   ```

## How to use it

1. build it:
   ```
   dotnet build
   ```

2. run it:
   ```
   dotnet run
   ```

3. Now you can:
   - Ask it stuff and get answers from maritaca
   - Type `.search` followed by what you want to look up on the web
   - Type `.exit` when you're done

Example:
```
Send your message (.search to search, .exit to exit):
what's the biggest planet?

Send your message (.search to search, .exit to exit):
.search latest mars discoveries

Send your message (.search to search, .exit to exit):
.exit
```

## Contributions

want to contribute? awesome! feel free to send an issue or pull request.

## Shoutouts

big thanks to [Maritaca AI](https://www.maritaca.ai/) and [Tavily](https://tavily.com/) for their cool apis!

## Disclaimer

this is a fan-made project. we're not officially connected to Maritaca or Tavily in any way.

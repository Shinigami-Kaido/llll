# MoonsecBot — Moonsec v3 deobfuscator Discord bot

This repo contains:
- MoonsecDeobfuscator: your deobfuscator code (library)
- MoonsecBot: a simple Discord bot that accepts obfuscated Lua (attachment or code block) and replies with cleaned Lua or serialized bytecode

Quick start (local)
1. Set environment variable:
   - Windows (PowerShell): `$env:DISCORD_TOKEN = "your_token_here"`
   - Linux/macOS: `export DISCORD_TOKEN="your_token_here"`
2. Build & run:
   - `dotnet build`
   - `dotnet run --project MoonsecBot`

Deploy
- Use Render / Fly / Railway / DigitalOcean App / Azure to run the Docker container or use the GitHub Actions workflow to build/push images.
- Add `DISCORD_TOKEN` as a secret in your host's environment variables.

Usage in Discord
- `!deob -dis` followed by a Lua code block or attach the obfuscated file — replies with cleaned Lua
- `!deob -dev` same input — replies with the serialized bytecode as a file

Security
- Limit who can use the bot or add rate limiting if you expose it publicly (deobfuscation can be CPU-heavy).

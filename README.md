# OpenTerraria
A small library aimed at recreating a Terraria client.

# Capabilities
In it's current state, this project can do basically anything a normal Terraria client except rendering, sound playback and control handling.

It has been built around the vanilla Terraria v1.3.0.8 (156 internally) server.
Tshock support has not been tested and is not planned.

# Prerequisites
* .Net 4.5.2
* C#6

# Usage
We have no docs at the moment. Refer to these implementations instead:
https://github.com/SSStormy/TerrariaBridgeTests/blob/master/Program.cs
https://github.com/SSStormy/Stormbot/blob/master/Bot.Core/Modules/Relay/TerrariaRelayModule.cs

# Big thanks
Without some public resources and people this wouldn't have been possible.
Thanks to:
* The TShock team, for https://tshock.atlassian.net/wiki/display/TSHOCKPLUGINS/Packet+Documentation
* Seancode, for http://seancode.com/terrafirma/net.html
* Voltana (RogueException), for introducing me to various design patterns I've never seen
* destruc7i0n, for inspiring me to make this in the first place
* X-tis and Rebbit for testing.

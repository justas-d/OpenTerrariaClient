# TerrariaBridge
A small library aimed at simulating a terraria client via a command line interface.

# Capabilities
In it's current state, this project can:
* Conenct 
* Log in (passwords are supported)
* Receive and send packets (receiving is event driven)


It has been built around the vanilla Terraria v1.3.0.8 (156 internally) server.
Tshock support hasn't been tested and is not planned.

# Prerequisites
* .Net 4.5.2
* C#6
* A Terraria server to connect to

# Usage
We have no docs at the moment. Refer to these implementations instead:
https://github.com/SSStormy/TerrariaBridgeTests/blob/master/Program.cs
https://github.com/SSStormy/Stormbot/blob/master/Bot.Core/Modules/TerrariaModule.cs

# Big thanks
Without some public resources and people this wouldn't have been possible.
Thanks to:
* The TShock team, for https://tshock.atlassian.net/wiki/display/TSHOCKPLUGINS/Packet+Documentation
* Seancode, for http://seancode.com/terrafirma/net.html
* Voltana (RogueException), for introducing me to various design patterns I've never seen
* destruc7i0n, for inspiring me to make this in the first place
* X-tis and Rebbit for testing.

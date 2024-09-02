# tshock-invsee
 View other players inventory
 
 ***

Usage: `/invsee <player name>`

Note: `<player name>` can just be the first couple letters of the players name

For example, to view inventory of a player named onusai you can type `/invsee onusai` or `/invsee onu` or `/onvsee o`

To view all players inventory use `/invsee all` 

To also see items in banks, add `banks` as the end of the command `/ivnsee onusai banks` or `/invsee all banks`

***

By default /invsee will only work on players on your team

If you want invsee ignore teams and work on all players, edit `tshock/InvSee.json` and set `OnlyShowIfOnTeam` to `false`

If a player has permission `tshock.admin` they will be able to view any player's inventory regardless of team

***

[Download InvSee.dll](https://github.com/onusai/tshock-invsee/raw/main/bin/Debug/net6.0/InvSee.dll)

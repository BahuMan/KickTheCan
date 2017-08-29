# KickTheCan
Online Multiplayer Variant of the well-known children's game

# done:
- local multiplayer host
- clients can join
- moving is synchronized (except for gun up/down)
- 1 player is designated "it"
- puff of smoke when shooting, players hit get tagged
- server assigns "it", other players are hiding (fixed)
- when hiding or tagged, the gun disappears
- added (stupid) bots
- rudimentary chat

# todo:
- HudControl chat can't call Commands because "it doesn't have local authority"
- bug, not glitch: in the 5 seconds "unlocked", players can put themselves on the wrong side of the fence (inside when they're not tagged, outside although they've been tagged (again, in the 5 seconds) ==> can we settle collisions with tags instead?
- cross hair
- HUD
- gun should follow look (up and down as well as rotation)
- camera shows vertical and horizontal black glitches
- actual player models? (with NetworkAnimator)
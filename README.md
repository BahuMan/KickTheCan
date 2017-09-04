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
- cross hair
- TagArea only locks players inside if they have been tagged (some layers magic)

# todo:
- sometimes nullpointer on TagArea  when tagging a player
- HUD
- gun should follow look (up and down as well as rotation)
- camera shows vertical and horizontal black glitches (probably due to 2 cameras active)
- actual player models? (with NetworkAnimator)
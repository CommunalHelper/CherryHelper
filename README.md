# Cherry Helper

A constantly-growing helper mod for Celeste that adds a lot of stuff! If you have a suggestion or a bug that needs fixing, either tell me via Discord (@aridai#3842) or open an issue here!

## Mechanics

Lightning Dash Switch - A special Dash Switch that turns off lightning within the room. Pair with a flag trigger with "disable_lightning" to make it global.

Shadow Dash - A powerup that gives the player access to an invincible dash (**NOTE: Green Boosters also trigger the Shadow Dash, currently looking for a fix)**

Shadow Dash Bumper - Bumpers that give a Shadow Dash when hit. Also have an option to disable the wiggling option.

Non-Return Kevin - What it says on the tin. A Kevin that doesn't return to original position after slamming into the wall. Can look like the standard variety, but also comes in vaporwave custom recolor variety!

Rotten Berry (+ Collection Trigger)  
A new variety of berry to spice up your berry rooms! If you collect it, you die. The only way to bank the strawberry is to enter a special trigger. (**NOTE: Trigger is invisible, sprites for a collection object coming soon.)**

Snowball Stop Trigger - Stops snowballs. Simple, yet useful. **(NOTE: Only removes itself if snowballs were present beforehand)**

Item Crystal and Item Crystal Pedestal - A throwable that, when placed upon the Item Crystal Pedestal, releases its' contents. And there's a lot of those:

- Refill (One Dash, Two Dash, Shadow Dash),
- Touch Switch,
- Jellyfish,
- Fireball,
- Summit Confetti (aka the troll Item Crystal)
- Seeker
- Badeline Chaser
- Cloud (Normal, Fragile)
- Booster (Green, Red)
- Core Toggle Switch
- Feather,
- Theo Crystal (aka the double layer Theo Crystal),
- Crystal Heart
- ????

Cat Eyes - Act like snowballs if snowballs didn't move. Have custom animations and sounds, as well as an option to refill two dashes or the Shadow Dash!

Item Toggle Field and Trigger - You ever wish to make items disappear and reappear whenever you want? By simply putting the field over the entities you want to toggle, and specifying what type of item you want to remove (such as Refil, ZipMover or CherryHelper/CatEye), you can summon anything you want at any time. The triggers can either enable the field with the same Night ID (making them spit out their contents) or disable it (making the items currently within the field disappear). Now, the Fields also come with an "Assist Rectangle On Solid" option, that places an indicator on a removed block (such as a Zip Mover or Space Jam)

Custom Teleport Mirrors - A snazzy way to reach any place you want! Wanna make the player disappear within a cutscene only to reappear in another room? You can do that! Wanna have a cool way to end off a chapter? You can do that? Wanna move to **a completely different chapter, modded or otherwise?!** Well, you can do that to, but **please read the tooltips in Ahorn first.** It's easy to get lost when you're being taken to a completely different location in time and space after all!

Door Fields - Gone are the days of unsollicited teleportation. Players now want to be able to choose when they're being teleported, even if they don't know where. With this, you can give them that power! Same options as Teleport Mirrors, keep the rules in mind. To activate, press C when in the range.

Save Data Flag Trigger - A trigger that saves the state of any session flag within a campaign. From letting your progress with flags get saved even if the player returns to map to letting flags impact other maps in the same campaign, you can use this mechanic for a lot of different things!

Outline Indicator - A handy-dandy rectangular indicator based on the appearence of disappearing crushing hazards in Assist Mode with customizable colors! Whenther you want to indicate triggers or just decorate your map with cool looking rectangles, Outline Indicators (also known as Assist Rectangles) have you covered.

Hostile Player Playback - A recordable Badeline Chaser! Same rules apply as in normal Player Playbacks, check out **[Kayden's Commands](https://gamebanana.com/gamefiles/10271)** if you wanna record them!

Non-Return Sokobans - Like Non-Return Kevins, but they move in the direction you dashed at them, rather than the opposite. Inspired by the [Sardine7](https://gamebanana.com/gamefiles/11275) Sokoban Blocks!

Time Limit Controller - Debuting in the NYC2021 entry "Anterograde", this entity allows you to set a mandatory time limit on a particular room, indicated by a colorgrade of your own choosing! Add a sense of intensity and a dated game design mechanic with a single click!

State Change Trigger (WARNING: EXPERIMENTAL) - A trigger that lets you change the player's state on a whim. Originally made to put the player in/out of Reflection's intro fall state. However, it can be used with whatever vanilla state you wish to use, with results of varying jankness. It's highly experimental, so don't blame me for crashes if you use this thing with weird states.

Audio Play Trigger - A simplified trigger for playing audio effects when you step in it. That's it!

Uninterrupted Non-Return Kevins - Like normal Non-Return Kevins, but they can't be interrupted as they move. Complete with their own reskin, and made customizable thanks to [BigKahuna](https://github.com/bigkahuna443)!

Time Limit Controller Revert Triggers - Zones of safety that reset the Time Limit Controller's time limit!

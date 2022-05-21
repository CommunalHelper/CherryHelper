local badelineBot = {}

local baseGameTutorials = {
    "combo", "superwalljump", "too_close", "too_far", "wavedash", "wavedashppt"
}

badelineBot.name = "CherryHelper/BadelineBot"
badelineBot.depth = 0
badelineBot.texture = "characters/player_badeline/sitDown00"
badelineBot.justification = {0.5, 1.0}
badelineBot.fieldInformation = {
    tutorial = {
        options = baseGameTutorials
    }
}
badelineBot.nodeLineRenderType = "line"
badelineBot.nodeLimits = {0, 2}
badelineBot.placements = {
    {
        name = "badelineBot",
        data = {
            tutorial = ""
        }
    }
}

return badelineBot
local shadowBumper = {}

shadowBumper.name = "CherryHelper/ShadowBumper"
shadowBumper.texture = "objects/shadowBumper/shadow22"
shadowBumper.nodeLineRenderType = "line"
shadowBumper.nodeLimits = {0, 1}
shadowBumper.placements = {
    {
        name = "shadowBumper",
        data = {
            wiggles = true
        }
    }
}

return shadowBumper
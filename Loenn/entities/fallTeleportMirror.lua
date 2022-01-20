local fallTeleportMirror = {}

local sides = {
    A = "A",
    B = "B",
    C = "C"
}

fallTeleportMirror.name = "CherryHelper/FallTeleport"
fallTeleportMirror.texture = "objects/temple/portal/portalframe"
fallTeleportMirror.justification ={0.5, 0.5}
fallTeleportMirror.fieldInformation = {
    Side = {
        options = sides,
        editable = false
    }
}
fallTeleportMirror.placements = {
    {
        name = "roomTeleport",
        data = {
            toLevel="",
            EndLevel=false,
            SIDOfMap="",
            LoadAnotherBin=false,
            Side="A",
            CustomFrameSprite=""
        }
    },
    {
        name = "endLevel",
        data = {
            toLevel="",
            EndLevel=true,
            SIDOfMap="",
            LoadAnotherBin=false,
            Side="A",
            CustomFrameSprite=""
        }
    },
    {
        name = "chapterChange",
        data = {
            toLevel="",
            EndLevel=false,
            SIDOfMap="",
            LoadAnotherBin=true,
            Side="A",
            CustomFrameSprite=""
        }
    }
}


return fallTeleportMirror
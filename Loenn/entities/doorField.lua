local doorField = {}

local sides = {
    A = "A",
    B = "B",
    C = "C"
}

doorField.name = "CherryHelper/DoorField"
doorField.justification ={0.5, 0.5}
doorField.borderColor = {0.2, 0.2, 0.6, 1.0}
doorField.fillColor = {0.0, 0.0, 0.0, 1.0}
doorField.fieldInformation = {
    Side = {
        options = sides,
        editable = false
    }
}
doorField.placements = {
    {
        name = "roomTeleport",
        data = {
            toLevel="",
            EndLevel=false,
            SIDOfMap="",
            LoadAnotherBin=false,
            Side="A",
            width=16,
            height=16,
            spawnOffsetX=0,
            spawnOffsetY=0
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
            width=16,
            height=16,
            spawnOffsetX=0,
            spawnOffsetY=0
        }
    },
    {
        name = "chapterChange",
        data = {
            toLevel="",
            EndLevel=false,
            SIDOfMap="Celeste/1-ForsakenCity",
            LoadAnotherBin=true,
            Side="A",
            width=16,
            height=16,
            spawnOffsetX=0,
            spawnOffsetY=0
        }
    }
}


return doorField
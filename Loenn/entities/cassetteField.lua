local cassetteField = {}

local colors = {
    Blue = 0,
    Rose = 1,
    Bright_Sun = 2,
    Malachite = 3
}

cassetteField.name = "CherryHelper/CassetteField"
cassetteField.placements = {}

for color, indx in pairs(colors) do
    table.insert(cassetteField.placements, {
        name = color,
        width = 16,
        height = 16,
        index = indx,
        tempo = 1.0,
        Type = "ZipMover",
        placementType = "rectangle"
    })
end

function cassetteField.fillColor(room, entity)
    if entity.index == 0 then
        return {0.0, 0.53, 1.0, 0.4}
    elseif entity.index == 1 then
        return {1.0, 0.23, 0.79, 0.4}
    elseif entity.index == 2 then
        return {1.0, 0.79, 0.0, 0.4}
    elseif entity.index == 3 then
        return {0.54, 1.0, 0.22, 0.4}
    end
end

function cassetteField.borderColor(room, entity)
    if entity.index == 0 then
        return {0.0, 0.53, 1.0}
    elseif entity.index == 1 then
        return {1.0, 0.23, 0.79}
    elseif entity.index == 2 then
        return {1.0, 0.79, 0.0}
    elseif entity.index == 3 then
        return {0.54, 1.0, 0.22}
    end
end

return cassetteField
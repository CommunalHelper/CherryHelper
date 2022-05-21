local dSprite = require("structs.drawable_sprite")

local lightningDashSwitch = {}

local sides = {
    "left",
    "right",
    "up",
    "down"
}

local textures = {
    "default",
    "mirror"
}

lightningDashSwitch.name = "CherryHelper/LightningDashSwitch"
lightningDashSwitch.placements = {}
lightningDashSwitch.fieldInformation = {
    texture = {
        options = textures
    },
    side = {
        options = sides
    }
}

for _, side1 in pairs(sides) do
    table.insert(lightningDashSwitch.placements, {
        name = side1,
        data = {
            side = side1,
            persistent = false,
            sprite = "default"
        }
    })
end

function lightningDashSwitch.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0

    local texture = "objects/temple/dashButton00"
    if entity.sprite == "mirror" then
        texture = "objects/temple/dashButtonMirror00"
    end
    local sprite = dSprite.fromTexture(texture)

    sprite:addPosition(x, y)

    if entity.side == "left" then 
        sprite.rotation = math.pi
    elseif entity.side == "right" then
        sprite.rotation = 0
        sprite:addPosition(8, 0)
    elseif entity.side == "down" then
        sprite.rotation = math.pi/2
        sprite:addPosition(0, 8)
    elseif entity.side == "up" then
        sprite.rotation = -math.pi/2
    end

    return sprite
end

return lightningDashSwitch
local dNPatch = require("structs.drawable_nine_patch")
local utils = require("utils")

local assistRectangle = {}

assistRectangle.name = "CherryHelper/AssistRect"
assistRectangle.placements = {
    {
        name = "assRect",
        data = {
            width = 16,
            height = 16,
            color = "ffffff"
        }
    }
}

local nPatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

function assistRectangle.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width,height = entity.width or 16, entity.height or 16

    local nPatch = dNPatch.fromTexture("objects/assistRectangle/assistRectangle", nPatchOptions, x, y, width, height)
    
    local sprites = nPatch:getDrawableSprite();
    local success, r, g, b = utils.parseHexColor(entity.color)
    if success then
        for _, sprite in pairs(sprites) do
            sprite:setColor({r,g,b})
        end
    end

    return sprites
end

return assistRectangle
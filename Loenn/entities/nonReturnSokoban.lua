local drawableNPatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")

local axisOptions = {
    Both = "both",
    Vertical = "vertical",
    Horizontal = "horizontal",
}

local nonReturnSokoban = {}

nonReturnSokoban.name = "CherryHelper/NonReturnSokoban"
nonReturnSokoban.depth = 0
nonReturnSokoban.minimumSize = {24, 24}
nonReturnSokoban.fieldInformation = {
    axes = {
        options = axisOptions,
        editable = false
    }
}
nonReturnSokoban.placements = {}

for _, axis in pairs(axisOptions) do
    table.insert(nonReturnSokoban.placements, {
        name = axis,
        data = {
            width = 24,
            height = 24,
            axes = axis,
            altTexture = false,
            chillout = false
        }
    })
end

for _, axis in pairs(axisOptions) do
    table.insert(nonReturnSokoban.placements, {
        name = "alt" .. axis,
        data = {
            width = 24,
            height = 24,
            axes = axis,
            altTexture = true,
            chillout = false
        }
    })
end

local frameTextures = {
    none = "objects/noReturnSokoban/block00",
    horizontal = "objects/noReturnSokoban/block01",
    vertical = "objects/noReturnSokoban/block02",
    both = "objects/noReturnSokoban/block03"
}

local altFrameTextures = {
    none = "objects/noReturnSokobanAlt/block00",
    horizontal = "objects/noReturnSokobanAlt/block01",
    vertical = "objects/noReturnSokobanAlt/block02",
    both = "objects/noReturnSokobanAlt/block03"
}

local nPatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

local sokobanColor = {195 / 255, 138 / 255, 6 / 255}
local smallFaceTexture = "objects/noReturnSokoban/idle_face"
local giantFaceTexture = "objects/noReturnSokoban/giant_block00"

local altSokobanColor = {36 / 255, 34 / 255, 98 / 255}
local altSmallFaceTexture = "objects/noReturnSokobanAlt/idle_face"
local altGiantFaceTexture = "objects/noReturnSokobanAlt/giant_block00"

function nonReturnSokoban.sprite(room, entity)
    local altTexture = entity.altTexture or false

    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local axes = entity.axes or "both"
    local chillout = entity.chillout

    local giant = height >= 48 and width >= 48 and chillout

    local faceTexture = giant and giantFaceTexture or smallFaceTexture
    if altTexture then
        faceTexture = giant and altGiantFaceTexture or altSmallFaceTexture
    end

    local frameTexture = frameTextures[axes] or frameTextures["both"]
    if altTexture then
        frameTexture = altFrameTextures[axes] or altFrameTextures["both"]
    end

    local ninePatch = drawableNPatch.fromTexture(frameTexture, nPatchOptions, x, y, width, height)

    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, sokobanColor)
    if altTexture then
        rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, altSokobanColor)
    end

    local faceSprite = drawableSprite.fromTexture(faceTexture, entity)
    faceSprite:addPosition(math.floor(width / 2), math.floor(height / 2))

    local sprites = ninePatch:getDrawableSprite()

    table.insert(sprites, 1, rectangle:getDrawableSprite())
    table.insert(sprites, 2, faceSprite)

    return sprites
end

return nonReturnSokoban
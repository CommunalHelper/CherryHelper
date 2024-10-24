local drawableNPatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local axisOptions = {
    Both = "both",
    Vertical = "vertical",
    Horizontal = "horizontal",
}

local nonReturnKevin = {}

nonReturnKevin.name = "CherryHelper/NonReturnKevin"
nonReturnKevin.depth = 0
nonReturnKevin.minimumSize = { 24, 24 }
nonReturnKevin.fieldInformation = {
    axes = {
        options = axisOptions,
        editable = false
    },
    fillColor = {
        fieldType = "color",
    }
}
nonReturnKevin.placements = {}

for _, axis in pairs(axisOptions) do
    table.insert(nonReturnKevin.placements, {
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
    table.insert(nonReturnKevin.placements, {
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

for _, axis in pairs(axisOptions) do
    table.insert(nonReturnKevin.placements, {
        name = "reskinnable" .. axis,
        data = {
            width = 24,
            height = 24,
            axes = axis,
            altTexture = true,
            spriteDirectory = "objects/noReturnKevin",
            fillColor = "242262",
            chillout = false
        }
    })
end

local frameTextures = {
    none = "/block00",
    horizontal = "/block01",
    vertical = "/block02",
    both = "/block03"
}

local nPatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

local kevinColor = { 98 / 255, 34 / 255, 43 / 255 }
local altKevinColor = { 36 / 255, 34 / 255, 98 / 255 }

local function isempty(s)
    return s == nil or s == ""
end

local function getBlockStyle(entity)
    local entitySpriteDir = entity.altTexture
        and (isempty(entity.spriteDirectory) and "objects/noReturnKevin" or entity.spriteDirectory)
        or "objects/crushblock"
    local frameTexture = entitySpriteDir .. frameTextures[entity.axes or "both"]
    local smallFaceTexture = entitySpriteDir .. "/idle_face"
    local giantFaceTexture = entitySpriteDir .. "/giant_block00"
    local fillColor = entity.altTexture
        and (isempty(entity.fillColor) and altKevinColor or utils.getColor(entity.fillColor))
        or kevinColor

    return {
        spriteDir = entitySpriteDir,
        frameTexture = frameTexture,
        smallFaceTexture = smallFaceTexture,
        giantFaceTexture = giantFaceTexture,
        fillColor = fillColor
    }
end

function nonReturnKevin.sprite(room, entity)
    local blockStyle = getBlockStyle(entity)

    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local chillout = entity.chillout
    local giant = height >= 48 and width >= 48 and chillout

    local ninePatch = drawableNPatch.fromTexture(blockStyle.frameTexture, nPatchOptions, x, y, width, height)
    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, blockStyle.fillColor)

    local faceTexture = giant and blockStyle.giantFaceTexture or blockStyle.smallFaceTexture
    local faceSprite = drawableSprite.fromTexture(faceTexture, entity)
    faceSprite:addPosition(math.floor(width / 2), math.floor(height / 2))

    local sprites = ninePatch:getDrawableSprite()
    table.insert(sprites, 1, rectangle:getDrawableSprite())
    table.insert(sprites, 2, faceSprite)
    return sprites
end

return nonReturnKevin

module CH_NonReturnSokoban


using ..Ahorn, Maple


@mapdef Entity "CherryHelper/NonReturnSokoban" NonReturnSokoban(x::Integer, y::Integer, width::Integer=32, height::Integer=32, axes::String="both", chillout::Bool=false, altTexture::Bool=false)


const placements = Ahorn.PlacementDict(
    "Non-Return Sokoban (iamdadbod, Cherry Helper)" => Ahorn.EntityPlacement(
       NonReturnSokoban,
        "rectangle",
        Dict{String, Any}(
            "altTexture" => false
        )
    ),
    "Non-Return Sokoban (Vaporwave, Cherry Helper)" => Ahorn.EntityPlacement(
       NonReturnSokoban,
        "rectangle",
        Dict{String, Any}(
            "altTexture" => true
        )
    ),
)



const SokobanColor = (195, 138, 6) ./ 255
const altSokobanColor = (36, 34, 98) ./ 255
Ahorn.editingOptions(entity::NonReturnSokoban) = Dict{String, Any}(
    "axes" => Maple.kevin_axes
)

Ahorn.minimumSize(entity::NonReturnSokoban) = 24, 24
Ahorn.resizable(entity::NonReturnSokoban) = true, true

Ahorn.selection(entity::NonReturnSokoban) = Ahorn.getEntityRectangle(entity)

# Todo - Use randomness to decide on Sokoban border
function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::NonReturnSokoban, room::Maple.Room)
    restrainedTex = (get(entity.data, "altTexture", false) ? "objects/noReturnSokobanAlt" : "objects/noReturnSokoban")

    frameImage = Dict{String, String}(
        "none" => string(restrainedTex, "/block00"),
        "horizontal" => string(restrainedTex, "/block01"),
        "vertical" => string(restrainedTex, "/block02"),
        "both" => string(restrainedTex, "/block03")
    )
    axes = lowercase(get(entity.data, "axes", "both"))
    chillout = get(entity.data, "chillout", false)
    
    smallFace = string(restrainedTex,"/idle_face")
    giantFace = string(restrainedTex,"/giant_block00")

    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    giant = height >= 48 && width >= 48 && chillout
    face = giant ? giantFace : smallFace
    frame = frameImage[lowercase(axes)]
    faceSprite = Ahorn.getSprite(face, "Gameplay")

    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)

    Ahorn.drawRectangle(ctx, 2, 2, width - 4, height - 4, (get(entity.data, "altTexture", false) ?  altSokobanColor : SokobanColor))
    Ahorn.drawImage(ctx, faceSprite, div(width - faceSprite.width, 2), div(height - faceSprite.height, 2))

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, 0, 8, 0, 8, 8)
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, height - 8, 8, 24, 8, 8)
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, frame, 0, (i - 1) * 8, 0, 8, 8, 8)
        Ahorn.drawImage(ctx, frame, width - 8, (i - 1) * 8, 24, 8, 8, 8)
    end

    Ahorn.drawImage(ctx, frame, 0, 0, 0, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, 0, 24, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, 0, height - 8, 0, 24, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, height - 8, 24, 24, 8, 8)
end

end
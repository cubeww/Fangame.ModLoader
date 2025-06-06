if (global.po_objPlayer != -1)
{
    return argument0.image_xscale * argument0.xScale;
}
else if (global.po_player_flip != -1)
{
    return argument0.image_xscale * argument0.x_scale;
}
else if (global.po_flip_player != -1)
{
    return argument0.image_xscale * argument0.facing;
}
else
{
    return argument0.image_xscale;
}
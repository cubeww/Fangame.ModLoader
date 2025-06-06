if (global.po_scrFlipGrav != -1 || global.po_player_flip != -1)
{
    return argument0.image_yscale * global.grav;
}
else if (global.po_flip_player != -1)
{
    return argument0.image_yscale * argument0.vflip;
}
else
{
    return argument0.image_yscale;
}
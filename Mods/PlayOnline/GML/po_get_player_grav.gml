if (global.po_player2 != -1 || global.po_scrFlipGrav != -1 || global.po_player_flip != -1)
{
    return global.grav;
} 
else if (global.po_flip_player != -1) 
{
    return Player.vflip;
} 
else if (global.po_playerReverse != -1) 
{
    return global.reverse;
}
else
{
    return 0;
}
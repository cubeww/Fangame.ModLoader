if (global.po_player2 != -1)
{
    global.grav = argument0;
}
else if (global.po_scrFlipGrav != -1)
{
    if (global.grav != argument0)
    {
        script_execute(global.po_scrFlipGrav);
    }
}
else if (global.po_reversePlayer != -1)
{
    if (global.reverse != argument0)
    {
        script_execute(global.po_reversePlayer);
    }
}
else if (global.po_player_flip != -1)
{
    script_execute(global.po_player_flip, argument0);
}
else if (global.po_flip_player != -1)
{
    script_execute(global.po_flip_player, argument0);
}
if (global.po_player != -1) 
{
    p = instance_find(global.po_player, 0);
    if (p == -1 && global.po_player2 != -1) 
    {
        return instance_find(global.po_player2, 0);
    }
    return p;
} 
else if (global.po_objPlayer != -1) 
{
    return instance_find(global.po_objPlayer, 0);
} 
else if (global.po_Player != -1) 
{
    return instance_find(global.po_Player, 0);
}

return -1;
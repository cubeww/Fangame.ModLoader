if (global.po_player != -1) 
{
    p = instance_find(global.po_player);
    if (p == -1 && global.po_player2 != -1) 
    {
        return instance_find(global.po_player2);
    }
    return p;
} 
else if (global.po_objPlayer != -1) 
{
    return = instance_find(global.po_objPlayer);
} 
else if (global.po_Player != -1) 
{
    return = instance_find(global.po_Player);
}

return -1;
/// ONLINE
if (!global.PO_WORLD.__ONLINE_race)
{
    var po_save_position;
    po_save_position = false;
    
    if (global.PO_YOYOYO)
    {
        if (argument[0])
        {
            po_save_position = true;
        }
    }
    else if (global.PO_RENEX)
    {
        if (room != rmInit && room != rmTitle && room != rmMenu && room != rmOptions)
        {
            po_save_position = true;
        }
    }
    else
    {
        if (room != rSelectStage)
        {
            po_save_position = true;
        }
    }
    
    if (po_save_position)
    {
        po_buffer_clear(global.PO_WORLD.__ONLINE_buffer);
        __ONLINE_p = global.PO_PLAYER;
        if (global.PO_PLAYER2 != -1)
        {
            if (!instance_exists(__ONLINE_p))
            {
                __ONLINE_p = global.PO_PLAYER2;
            }
        }
        
        if (instance_exists(__ONLINE_p))
        {
            po_buffer_write_uint8(global.PO_WORLD.__ONLINE_buffer, 5);
            if (global.PO_YOYOYO)
            {
                po_buffer_write_uint8(global.PO_WORLD.__ONLINE_buffer, global.grav);
            }
            else
            {
                if (__ONLINE_p == global.PO_PLAYER)
                {
                    po_buffer_write_uint8(global.PO_WORLD.__ONLINE_buffer, 0);
                }
                else
                {
                    po_buffer_write_uint8(global.PO_WORLD.__ONLINE_buffer, 1);
                }
            }
            po_buffer_write_int32(global.PO_WORLD.__ONLINE_buffer, __ONLINE_p.x);
            po_buffer_write_float64(global.PO_WORLD.__ONLINE_buffer, __ONLINE_p.y);
            po_buffer_write_int16(global.PO_WORLD.__ONLINE_buffer, room);
            po_socket_write_message(global.PO_WORLD.__ONLINE_socket, global.PO_WORLD.__ONLINE_buffer);
        }
    }
}

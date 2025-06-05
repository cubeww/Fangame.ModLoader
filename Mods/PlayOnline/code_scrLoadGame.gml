/// ONLINE
var po_someone_saved;
po_someone_saved = false;
if (global.PO_TEMP_FILE)
{
    if (file_exists("tempOnline2"))
    {
        po_someone_saved = true;
        po_buffer_clear(global.PO_WORLD.__ONLINE_buffer);
        po_buffer_read_from_file(global.PO_WORLD.__ONLINE_buffer, "tempOnline2");
        global.PO_WORLD.__ONLINE_sGravity = po_buffer_read_uint8(global.PO_WORLD.__ONLINE_buffer);
        global.PO_WORLD.__ONLINE_sX = po_buffer_read_int32(global.PO_WORLD.__ONLINE_buffer);
        global.PO_WORLD.__ONLINE_sY = po_buffer_read_float64(global.PO_WORLD.__ONLINE_buffer);
        global.PO_WORLD.__ONLINE_sRoom = po_buffer_read_int16(global.PO_WORLD.__ONLINE_buffer);
        file_delete("tempOnline2");
    }   
}
else
{
    po_someone_saved = global.PO_WORLD.__ONLINE_sSaved;
}
if (po_someone_saved)
{
    if (room_exists(global.PO_WORLD.__ONLINE_sRoom))
    {
        __ONLINE_p = global.PO_PLAYER;
        if (global.PO_PLAYER2 != -1)
        {
            if (global.PO_WORLD.__ONLINE_sGravity == 1)
            {
                instance_create(0, 0, global.PO_PLAYER2);
                with (global.PO_PLAYER)
                {
                    instance_destroy();
                }
                __ONLINE_p = global.PO_PLAYER2;
            }
        }
        if (global.PO_YOYOYO)
        {
            if (global.grav != global.PO_WORLD.__ONLINE_sGravity)
            {
                script_execute(scrFlipGrav);
            }
        }
        else
        {
            global.grav = __ONLINE_sGravity;
        }
        __ONLINE_p = global.PO_PLAYER;
        __ONLINE_p.x = global.PO_WORLD.__ONLINE_sX;
        __ONLINE_p.y = global.PO_WORLD.__ONLINE_sY;
        room_goto(global.PO_WORLD.__ONLINE_sRoom);
    }
    global.PO_WORLD.__ONLINE_sSaved = false;
}

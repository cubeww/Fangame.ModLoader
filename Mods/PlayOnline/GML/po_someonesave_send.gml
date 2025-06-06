var cur_player;

if (!global.po_race)
{
    po_buffer_clear(global.po_buffer);
    cur_player = po_find_player();
    if (cur_player != noone)
    {
        po_buffer_write_uint8(global.po_buffer, 5);
        po_buffer_write_uint8(global.po_buffer, po_get_player_grav());
        po_buffer_write_int32(global.po_buffer, cur_player.x);
        po_buffer_write_float64(global.po_buffer, cur_player.y);
        po_buffer_write_int16(global.po_buffer, room);
        po_socket_write_message(global.po_socket, global.po_buffer);
    }
}

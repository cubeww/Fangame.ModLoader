/// ONLINE
if (global.PO_TEMP_FILE)
{
    with (global.PO_WORLD)
    {
        po_buffer_clear(__ONLINE_buffer);
        po_buffer_write_uint16(__ONLINE_buffer, __ONLINE_socket);
        po_buffer_write_uint16(__ONLINE_buffer, __ONLINE_udpsocket);
        po_buffer_write_string(__ONLINE_buffer, __ONLINE_selfID);
        po_buffer_write_string(__ONLINE_buffer, __ONLINE_name);
        po_buffer_write_string(__ONLINE_buffer, __ONLINE_selfGameID);
        po_buffer_write_uint8(__ONLINE_buffer, __ONLINE_race);
        __ONLINE_n = instance_number(__ONLINE_onlinePlayer);
        po_buffer_write_uint16(__ONLINE_buffer, __ONLINE_n);
        for (__ONLINE_i = 0; __ONLINE_i < __ONLINE_n; __ONLINE_i += 1)
        {
            __ONLINE_oPlayer = instance_find(__ONLINE_onlinePlayer, __ONLINE_i);
            po_buffer_write_string(__ONLINE_buffer, __ONLINE_oPlayer.__ONLINE_ID);
            po_buffer_write_int32(__ONLINE_buffer, __ONLINE_oPlayer.x);
            po_buffer_write_int32(__ONLINE_buffer, __ONLINE_oPlayer.y);
            po_buffer_write_int32(__ONLINE_buffer, __ONLINE_oPlayer.sprite_index);
            po_buffer_write_float32(__ONLINE_buffer, __ONLINE_oPlayer.image_speed);
            po_buffer_write_float32(__ONLINE_buffer, __ONLINE_oPlayer.image_xscale);
            po_buffer_write_float32(__ONLINE_buffer, __ONLINE_oPlayer.image_yscale);
            po_buffer_write_float32(__ONLINE_buffer, __ONLINE_oPlayer.image_angle);
            po_buffer_write_uint16(__ONLINE_buffer, __ONLINE_oPlayer.__ONLINE_oRoom);
            po_buffer_write_string(__ONLINE_buffer, __ONLINE_oPlayer.__ONLINE_name);
        }
        po_buffer_write_to_file(__ONLINE_buffer, "tempOnline");
    }
}
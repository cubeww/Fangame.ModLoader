var i, player_num, cur_player;

if (global.po_tempExe != -1)
{
    if (file_exists("tempOnline"))
    {
        po_buffer_read_from_file(global.po_buffer, "tempOnline");
        global.po_socket = po_buffer_read_uint16(global.po_buffer);
        global.po_udpsocket = po_buffer_read_uint16(global.po_buffer);
        global.po_self_ip = po_buffer_read_string(global.po_buffer);
        global.po_name = po_buffer_read_string(global.po_buffer);
        global.po_game_id = po_buffer_read_string(global.po_buffer);
        global.po_race = po_buffer_read_uint8(global.po_buffer);
        player_num = po_buffer_read_uint16(global.po_buffer);
        for (i = 0; i < player_num; i += 1)
        {
            cur_player = instance_create(0, 0, po_onlineplayer);
            cur_player.ip = po_buffer_read_string(global.po_buffer);
            cur_player.x = po_buffer_read_int32(global.po_buffer);
            cur_player.y = po_buffer_read_int32(global.po_buffer);
            cur_player.sprite_index = po_buffer_read_int32(global.po_buffer);
            cur_player.image_speed = po_buffer_read_float32(global.po_buffer);
            cur_player.image_xscale = po_buffer_read_float32(global.po_buffer);
            cur_player.image_yscale = po_buffer_read_float32(global.po_buffer);
            cur_player.image_angle = po_buffer_read_float32(global.po_buffer);
            cur_player.cur_room = po_buffer_read_uint16(global.po_buffer);
            cur_player.name = po_buffer_read_string(global.po_buffer);
        }
        return true;
    }
}

return false;
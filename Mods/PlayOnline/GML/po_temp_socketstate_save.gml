var player_num, i, cur_player;

if (global.po_tempExe != -1)
{
    po_buffer_clear(global.po_buffer);
    po_buffer_write_uint16(global.po_buffer, global.po_socket);
    po_buffer_write_uint16(global.po_buffer, global.po_udpsocket);
    po_buffer_write_string(global.po_buffer, global.po_self_ip);
    po_buffer_write_string(global.po_buffer, global.po_name);
    po_buffer_write_string(global.po_buffer, global.po_game_id);
    po_buffer_write_uint8(global.po_buffer, global.po_race);
    player_num = instance_number(po_onlineplayer);
    po_buffer_write_uint16(global.po_buffer, player_num);
    for (i = 0; i < player_num; i += 1)
    {
        cur_player = instance_find(po_onlineplayer, i);
        po_buffer_write_string(global.po_buffer, cur_player.ip);
        po_buffer_write_int32(global.po_buffer, cur_player.x);
        po_buffer_write_int32(global.po_buffer, cur_player.y);
        po_buffer_write_int32(global.po_buffer, cur_player.sprite_index);
        po_buffer_write_float32(global.po_buffer, cur_player.image_speed);
        po_buffer_write_float32(global.po_buffer, cur_player.image_xscale);
        po_buffer_write_float32(global.po_buffer, cur_player.image_yscale);
        po_buffer_write_float32(global.po_buffer, cur_player.image_angle);
        po_buffer_write_uint16(global.po_buffer, cur_player.cur_room);
        po_buffer_write_string(global.po_buffer, cur_player.name);
    }
    po_buffer_write_to_file(global.po_buffer, "tempOnline");
}
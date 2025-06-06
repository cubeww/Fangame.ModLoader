// =================================================================================
//                                         TCP
// =================================================================================

po_socket_update_read(global.po_socket);
while (po_socket_read_message(global.po_socket, global.po_buffer))
{
    switch (po_buffer_read_uint8(global.po_buffer))
    {
        case 0: // CREATED
            var created_player_ip, created_player, i, cur_player;
            created_player_ip = po_buffer_read_string(global.po_buffer);
            created_player = noone;
            for (i = 0; i < instance_number(po_onlineplayer); i += 1)
            {
                cur_player = instance_find(po_onlineplayer, i);
                if (cur_player.ip == created_player_ip)
                {
                    created_player = cur_player;
                    break;
                }
            }
            if (created_player == noone)
            {
                created_player = instance_create(0, 0, po_onlineplayer);
                created_player.ip = created_player_ip;
                created_player.name = po_buffer_read_string(global.po_buffer);
            }
            break;
        case 1: // DESTROYED
            var destroyed_player_ip, i, cur_player;
            destroyed_player_ip = po_buffer_read_string(global.po_buffer);
            for (i = 0; i < instance_number(po_onlineplayer); i += 1)
            {
                cur_player = instance_find(po_onlineplayer, i);
                if (cur_player.ip == destroyed_player_ip)
                {
                    with (cur_player)
                    {
                        instance_destroy();
                    }
                    break;
                }
            }
            break;
        case 2: // INCOMPATIBLE VERSION
            var last_version, err_message;
            last_version = po_buffer_read_string(global.po_buffer);
            err_message = "Your tool uses the version " + global.po_version + " but the oldest compatible version is " + last_version + ". Please update your tool.";
            show_message(err_message);
            game_end();
            exit;
            break;
        case 4: // CHAT MESSAGE 
            var chat_player_ip, chat_player, cur_player, i, new_chatbox;
            chat_player_ip = po_buffer_read_string(global.po_buffer);
            chat_player = noone;
            for (i = 0; i < instance_number(po_onlineplayer); i += 1)
            {
                cur_player = instance_find(po_onlineplayer, i);
                if (cur_player.ip == chat_player_ip)
                {
                    chat_player = cur_player;
                    break;
                }
            }
            if (chat_player != noone)
            {
                new_chatbox = instance_create(0, 0, po_chatbox);
                new_chatbox.message = po_buffer_read_string(global.po_buffer);
                new_chatbox.follower = chat_player;
                if (chat_player.visible)
                {
                    po_sound_play(global.po_snd_chatbox);
                }
            }
            break;
        case 5: // SOMEONE SAVED
            var new_player_saved;
            if (!global.po_race)
            {
                global.po_someone_saved = true;
                global.po_someone_grav = po_buffer_read_uint8(global.po_buffer);
                global.po_someone_name = po_buffer_read_string(global.po_buffer);
                global.po_someone_x = po_buffer_read_int32(global.po_buffer);
                global.po_someone_y = po_buffer_read_float64(global.po_buffer);
                global.po_someone_room = po_buffer_read_int16(global.po_buffer);
                new_player_saved = instance_create(0, 0, po_playersaved);
                new_player_saved.name = global.po_someone_name;
                po_temp_someonesaved_save();
                po_sound_play(global.po_snd_saved);
            }
            break;
        case 6: // SELF IP
            global.po_self_ip = po_buffer_read_string(global.po_buffer);
            break;
    }
}

var must_quit, err_message;
must_quit = false;
switch (po_socket_get_state(global.po_socket))
{
    case 2:
        if (!global.po_connected)
        {
            global.po_connected = true;
        }
        break;
    case 4:
        show_message("Connection closed.");
        must_quit = true;
        break;
    case 5:
        po_socket_reset(global.po_socket);
        err_message = "Could not connect to the server.";
        if (global.po_connected)
        {
            err_message = "Connection lost";
        }
        show_message(err_message);
        must_quit = true;
        break;
}
if (must_quit)
{
    if (po_use_temp_file())
    {
        if (file_exists("temp"))
        {
            file_delete("temp");
        }
    }
    game_end();
    exit;
}

var cur_player;
cur_player = po_find_player();
if (cur_player != noone)
{
    if (!global.po_last_player_exists)
    {
        // SEND PLAYER CREATE
        po_buffer_clear(global.po_buffer);
        po_buffer_write_uint8(global.po_buffer, 0);
        po_socket_write_message(global.po_socket, global.po_buffer);
    }
    global.po_stopped_frames += 1;
    if (global.po_last_player_x != cur_player.x || global.po_last_player_y != cur_player.y || keyboard_check_released(vk_anykey) || keyboard_check_pressed(vk_anykey))
    {
        global.po_stopped_frames = 0;
    }
    if (global.po_stopped_frames < 5 || global.po_timer < 3)
    {
        if (global.po_timer >= 3)
        {
            global.po_timer = 0;
        }
        
        // SEND PLAYER MOVED
        if (global.po_self_ip != "")
        {
            po_buffer_clear(global.po_buffer);
            po_buffer_write_uint8(global.po_buffer, 1);
            po_buffer_write_string(global.po_buffer, global.po_self_ip);
            po_buffer_write_string(global.po_buffer, global.po_game_id);
            po_buffer_write_uint16(global.po_buffer, room);
            po_buffer_write_uint64(global.po_buffer, current_time);
            po_buffer_write_int32(global.po_buffer, cur_player.x);
            po_buffer_write_int32(global.po_buffer, cur_player.y);
            po_buffer_write_int32(global.po_buffer, cur_player.sprite_index);
            po_buffer_write_float32(global.po_buffer, cur_player.image_speed);
            po_buffer_write_float32(global.po_buffer, po_get_player_xscale(cur_player));
            po_buffer_write_float32(global.po_buffer, po_get_player_yscale(cur_player));
            po_buffer_write_float32(global.po_buffer, cur_player.image_angle);
            po_buffer_write_string(global.po_buffer, global.po_name);
            po_udpsocket_send(global.po_udpsocket, global.po_buffer);
        }
    }
    global.po_timer += 1;
    if (keyboard_check_pressed(vk_space))
    {
        var chat_message, chat_message_len, new_chatbox;
        chat_message = get_string("Say something:", "");
        chat_message = string_replace_all(chat_message, "#", "\\#");
        chat_message_len = string_length(chat_message);
        if (chat_message_len > 0)
        {
            if (chat_message_len > 300)
            {
                chat_message = string_copy(chat_message, 0, chat_message_len);
            }

            // SEND CHAT MESSAGE
            po_buffer_clear(global.po_buffer);
            po_buffer_write_uint8(global.po_buffer, 4);
            po_buffer_write_string(global.po_buffer, chat_message);
            po_socket_write_message(global.po_socket, global.po_buffer);

            new_chatbox = instance_create(0, 0, po_chatbox);
            new_chatbox.message = chat_message;
            new_chatbox.follower = cur_player;
            po_sound_play(global.po_snd_chatbox);
        }
    }

    global.po_last_player_x = cur_player.x;
    global.po_last_player_y = cur_player.y;
}
else
{
    if (global.po_last_player_exists)
    {
        // SEND PLAYER DESTROYED
        po_buffer_clear(global.po_buffer);
        po_buffer_write_uint8(global.po_buffer, 1);
        po_socket_write_message(global.po_socket, global.po_buffer);
    }
}
global.po_last_player_exists = (cur_player != noone);

global.po_heartbeat += 1 / room_speed;
if (global.po_heartbeat > 3)
{
    global.po_heartbeat = 0;
    
    // SEND PLAYER HEARTBEAT
    po_buffer_clear(global.po_buffer);
    po_buffer_write_uint8(global.po_buffer, 2);
    
    po_socket_write_message(global.po_socket, global.po_buffer);
}
po_socket_update_write(global.po_socket);

// =================================================================================
//                                         UDP
// =================================================================================

while (po_udpsocket_receive(global.po_udpsocket, global.po_buffer))
{
    switch (po_buffer_read_uint8(global.po_buffer))
    {
        case 1: // RECEIVED MOVED
            var moved_player_ip, moved_player, moved_game_id, cur_player;
            moved_player_ip = po_buffer_read_string(global.po_buffer);
            moved_game_id = po_buffer_read_string(global.po_buffer);
            moved_player = noone;
            for (i = 0; i < instance_number(po_onlineplayer); i += 1)
            {
                cur_player = instance_find(po_onlineplayer, i);
                if (cur_player.ip == moved_player_ip)
                {
                    moved_player = cur_player;
                    break;
                }
            }
            if (moved_player == noone)
            {
                moved_player = instance_create(0, 0, po_onlineplayer);
                moved_player.ip = moved_player_ip;
            }
            moved_player.cur_room = po_buffer_read_uint16(global.po_buffer);
            global.po_sync_time = po_buffer_read_uint64(global.po_buffer);
            if (moved_player.sync_time < global.sync_time)
            {
                moved_player.sync_time = global.po_sync_time;
                moved_player.x = po_buffer_read_int32(global.po_buffer);
                moved_player.y = po_buffer_read_int32(global.po_buffer);
                moved_player.sprite_index = po_buffer_read_int32(global.po_buffer);
                moved_player.image_speed = po_buffer_read_float32(global.po_buffer);
                moved_player.image_xscale = po_buffer_read_float32(global.po_buffer);
                moved_player.image_yscale = po_buffer_read_float32(global.po_buffer);
                moved_player.image_angle = po_buffer_read_float32(global.po_buffer);
                moved_player.name = po_buffer_read_string(global.po_buffer);
            }
            break;
        default:
            show_message("Received unexpected data from the server.");
    }
}
if (po_udpsocket_get_state(global.po_udpsocket) != 1)
{
    show_message("Connection to the UDP socket lost.");
    game_end();
    exit;
}

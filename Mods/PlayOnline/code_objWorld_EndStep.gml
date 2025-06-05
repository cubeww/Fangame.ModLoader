/// ONLINE
// TCP SOCKETS 
po_socket_update_read(__ONLINE_socket);
while (po_socket_read_message(__ONLINE_socket, __ONLINE_buffer))
{
    switch (po_buffer_read_uint8(__ONLINE_buffer))
    {
        case 0: // CREATED 
            __ONLINE_ID = po_buffer_read_string(__ONLINE_buffer);
            __ONLINE_found = false;
            for (__ONLINE_i = 0; __ONLINE_i < instance_number(__ONLINE_onlinePlayer) && !__ONLINE_found; __ONLINE_i += 1)
            {
                if (instance_find(__ONLINE_onlinePlayer, __ONLINE_i).__ONLINE_ID == __ONLINE_ID)
                {
                    __ONLINE_found = true;
                }
            }
            if (!__ONLINE_found)
            {
                __ONLINE_oPlayer = instance_create(0, 0, __ONLINE_onlinePlayer);
                __ONLINE_oPlayer.__ONLINE_ID = __ONLINE_ID;
                __ONLINE_oPlayer.__ONLINE_name = po_buffer_read_string(__ONLINE_buffer);
            }
            break;
        case 1: // DESTROYED 
            __ONLINE_ID = po_buffer_read_string(__ONLINE_buffer);
            __ONLINE_found = false;
            for (__ONLINE_i = 0; __ONLINE_i < instance_number(__ONLINE_onlinePlayer) && !__ONLINE_found; __ONLINE_i += 1)
            {
                __ONLINE_oPlayer = instance_find(__ONLINE_onlinePlayer, __ONLINE_i);
                if (__ONLINE_oPlayer.__ONLINE_ID == __ONLINE_ID)
                {
                    with (__ONLINE_oPlayer)
                    {
                        instance_destroy();
                    }
                    __ONLINE_found = true;
                }
            }
            break;
        case 2: // INCOMPATIBLE VERSION 
            __ONLINE_lastVersion = po_buffer_read_string(__ONLINE_buffer);
            __ONLINE_errorMessage = "Your tool uses the version " + __ONLINE_version + " but the oldest compatible version is " + __ONLINE_lastVersion + ". Please update your tool.";
            show_message(__ONLINE_errorMessage);
            game_end();
            exit;
            break;
        case 4: // CHAT MESSAGE 
            __ONLINE_ID = po_buffer_read_string(__ONLINE_buffer);
            __ONLINE_found = false;
            __ONLINE_oPlayer = 0;
            for (__ONLINE_i = 0; __ONLINE_i < instance_number(__ONLINE_onlinePlayer) && !__ONLINE_found; __ONLINE_i += 1)
            {
                __ONLINE_oPlayer = instance_find(__ONLINE_onlinePlayer, __ONLINE_i);
                if (__ONLINE_oPlayer.__ONLINE_ID == __ONLINE_ID)
                {
                    __ONLINE_found = true;
                }
            }
            if (__ONLINE_found)
            {
                __ONLINE_message = po_buffer_read_string(__ONLINE_buffer);
                __ONLINE_oChatbox = instance_create(0, 0, __ONLINE_chatbox);
                __ONLINE_oChatbox.__ONLINE_message = __ONLINE_message;
                __ONLINE_oChatbox.__ONLINE_follower = __ONLINE_oPlayer;
                if (__ONLINE_oPlayer.visible)
                {
                    script_execute(po_sound_play, global.po_snd_chatbox);
                }
            }
            break;
        case 5: // SOMEONE SAVED 
            if (!__ONLINE_race)
            {
                __ONLINE_sSaved = true;
                __ONLINE_sGravity = po_buffer_read_uint8(__ONLINE_buffer);
                __ONLINE_sName = po_buffer_read_string(__ONLINE_buffer);
                __ONLINE_sX = po_buffer_read_int32(__ONLINE_buffer);
                __ONLINE_sY = po_buffer_read_float64(__ONLINE_buffer);
                __ONLINE_sRoom = po_buffer_read_int16(__ONLINE_buffer);
                __ONLINE_a = instance_create(0, 0, __ONLINE_playerSaved);
                __ONLINE_a.__ONLINE_name = __ONLINE_sName;
                if (global.PO_TEMP_FILE)
                {
                    po_buffer_clear(__ONLINE_buffer);
                    po_buffer_write_uint8(__ONLINE_buffer, __ONLINE_sGravity);
                    po_buffer_write_int32(__ONLINE_buffer, __ONLINE_sX);
                    po_buffer_write_float64(__ONLINE_buffer, __ONLINE_sY);
                    po_buffer_write_int16(__ONLINE_buffer, __ONLINE_sRoom);
                    po_buffer_write_to_file(__ONLINE_buffer, "tempOnline2");
                }
                script_execute(po_sound_play, global.po_snd_saved);
            }
            break;
        case 6: // SELF ID 
            __ONLINE_selfID = po_buffer_read_string(__ONLINE_buffer);
            break;
    }
}
__ONLINE_mustQuit = false;
switch (po_socket_get_state(__ONLINE_socket))
{
    case 2:
        if (!__ONLINE_connected)
        {
            __ONLINE_connected = true;
        }
        break;
    case 4:
        show_message("Connection closed.");
        __ONLINE_mustQuit = true;
        break;
    case 5:
        po_socket_reset(__ONLINE_socket);
        __ONLINE_errorMessage = "Could not connect to the server.";
        if (__ONLINE_connected)
        {
            __ONLINE_errorMessage = "Connection lost";
        }
        show_message(__ONLINE_errorMessage);
        __ONLINE_mustQuit = true;
        break;
}
if (__ONLINE_mustQuit)
{
    if (global.PO_TEMP_FILE)
    {
        if (file_exists("temp"))
        {
            file_delete("temp");
        }
    }
    game_end();
    exit;
}
__ONLINE_p = global.PO_PLAYER;
if (global.PO_PLAYER2 != -1)
{
    if (!instance_exists(__ONLINE_p))
    {
        __ONLINE_p = global.PO_PLAYER2;
    }
}
__ONLINE_exists = instance_exists(__ONLINE_p);
__ONLINE_X = __ONLINE_pX;
__ONLINE_Y = __ONLINE_pY;
if (__ONLINE_exists)
{
    if (__ONLINE_exists != __ONLINE_pExists)
    {
        // SEND PLAYER CREATE
        po_buffer_clear(__ONLINE_buffer);
        po_buffer_write_uint8(__ONLINE_buffer, 0);
        po_socket_write_message(__ONLINE_socket, __ONLINE_buffer);
    }
    __ONLINE_X = __ONLINE_p.x;
    __ONLINE_Y = __ONLINE_p.y;
    __ONLINE_stoppedFrames += 1;
    if (__ONLINE_pX != __ONLINE_X || __ONLINE_pY != __ONLINE_Y || keyboard_check_released(vk_anykey) || keyboard_check_pressed(vk_anykey))
    {
        __ONLINE_stoppedFrames = 0;
    }
    if (__ONLINE_stoppedFrames < 5 || __ONLINE_t < 3)
    {
        if (__ONLINE_t >= 3)
        {
            __ONLINE_t = 0;
        }
        
        // SEND PLAYER MOVED
        if (__ONLINE_selfID != "")
        {
            po_buffer_clear(__ONLINE_buffer);
            po_buffer_write_uint8(__ONLINE_buffer, 1);
            po_buffer_write_string(__ONLINE_buffer, __ONLINE_selfID);
            po_buffer_write_string(__ONLINE_buffer, __ONLINE_selfGameID);
            po_buffer_write_uint16(__ONLINE_buffer, room);
            po_buffer_write_uint64(__ONLINE_buffer, current_time);
            po_buffer_write_int32(__ONLINE_buffer, __ONLINE_X);
            po_buffer_write_int32(__ONLINE_buffer, __ONLINE_Y);
            po_buffer_write_int32(__ONLINE_buffer, __ONLINE_p.sprite_index);
            po_buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_speed);
            if (global.PO_GLOBAL_PLAYER_XSCALE)
            {
                po_buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_xscale * global.player_xscale);
            }
            else
            {
                if (global.PO_YOYOYO)
                {
                    po_buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_xscale * __ONLINE_p.xScale);
                }
                else
                {
                    po_buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_xscale);
                }
            }
            if (global.PO_YOYOYO)
            {
                po_buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_yscale * global.grav);
            }
            else
            {
                po_buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_yscale);
            }
            po_buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_angle);
            po_buffer_write_string(__ONLINE_buffer, __ONLINE_name);
            po_udpsocket_send(__ONLINE_udpsocket, __ONLINE_buffer);
        }
    }
    __ONLINE_t += 1;
    if (keyboard_check_pressed(vk_space))
    {
        __ONLINE_message = get_string("Say something:", "");
        __ONLINE_message = string_replace_all(__ONLINE_message, "#", "\\#");
        __ONLINE_message_length = string_length(__ONLINE_message);
        if (__ONLINE_message_length > 0)
        {
            __ONLINE_message_max_length = 300;
            if (__ONLINE_message_length > __ONLINE_message_max_length)
            {
                __ONLINE_message = string_copy(__ONLINE_message, 0, __ONLINE_message_max_length);
            }
            po_buffer_clear(__ONLINE_buffer);
            po_buffer_write_uint8(__ONLINE_buffer, 4);
            po_buffer_write_string(__ONLINE_buffer, __ONLINE_message);
            po_socket_write_message(__ONLINE_socket, __ONLINE_buffer);
            __ONLINE_oChatbox = instance_create(0, 0, __ONLINE_chatbox);
            __ONLINE_oChatbox.__ONLINE_message = __ONLINE_message;
            __ONLINE_oChatbox.__ONLINE_follower = __ONLINE_p;
            script_execute(po_sound_play, global.po_snd_chatbox);
        }
    }
}
else
{
    if (__ONLINE_exists != __ONLINE_pExists)
    {
        // SEND PLAYER DESTROYED
        po_buffer_clear(__ONLINE_buffer);
        po_buffer_write_uint8(__ONLINE_buffer, 1);
        po_socket_write_message(__ONLINE_socket, __ONLINE_buffer);
    }
}
__ONLINE_pExists = __ONLINE_exists;
__ONLINE_pX = __ONLINE_X;
__ONLINE_pY = __ONLINE_Y;
__ONLINE_heartbeat += 1 / room_speed;
if (__ONLINE_heartbeat > 3)
{
    __ONLINE_heartbeat = 0;
    
    // SEND PLAYER HEARTBEAT
    po_buffer_clear(__ONLINE_buffer);
    po_buffer_write_uint8(__ONLINE_buffer, 2);
    
    po_socket_write_message(__ONLINE_socket, __ONLINE_buffer);
}
po_socket_update_write(__ONLINE_socket);

// UDP SOCKETS 
while (po_udpsocket_receive(__ONLINE_udpsocket, __ONLINE_buffer))
{
    switch (po_buffer_read_uint8(__ONLINE_buffer))
    {
        case 1: // RECEIVED MOVED 
            __ONLINE_ID = po_buffer_read_string(__ONLINE_buffer);
            __ONLINE_gameID = po_buffer_read_string(__ONLINE_buffer);
            __ONLINE_found = false;
            __ONLINE_oPlayer = 0;
            for (__ONLINE_i = 0; __ONLINE_i < instance_number(__ONLINE_onlinePlayer) && !__ONLINE_found; __ONLINE_i += 1)
            {
                __ONLINE_oPlayer = instance_find(__ONLINE_onlinePlayer, __ONLINE_i);
                if (__ONLINE_oPlayer.__ONLINE_ID == __ONLINE_ID)
                {
                    __ONLINE_found = true;
                }
            }
            if (!__ONLINE_found)
            {
                __ONLINE_oPlayer = instance_create(0, 0, __ONLINE_onlinePlayer);
                __ONLINE_oPlayer.__ONLINE_ID = __ONLINE_ID;
            }
            __ONLINE_oPlayer.__ONLINE_oRoom = po_buffer_read_uint16(__ONLINE_buffer);
            __ONLINE_syncTime = po_buffer_read_uint64(__ONLINE_buffer);
            if (__ONLINE_oPlayer.__ONLINE_syncTime < __ONLINE_syncTime)
            {
                __ONLINE_oPlayer.__ONLINE_syncTime = __ONLINE_syncTime;
                __ONLINE_oPlayer.x = po_buffer_read_int32(__ONLINE_buffer);
                __ONLINE_oPlayer.y = po_buffer_read_int32(__ONLINE_buffer);
                __ONLINE_oPlayer.sprite_index = po_buffer_read_int32(__ONLINE_buffer);
                __ONLINE_oPlayer.image_speed = po_buffer_read_float32(__ONLINE_buffer);
                __ONLINE_oPlayer.image_xscale = po_buffer_read_float32(__ONLINE_buffer);
                __ONLINE_oPlayer.image_yscale = po_buffer_read_float32(__ONLINE_buffer);
                __ONLINE_oPlayer.image_angle = po_buffer_read_float32(__ONLINE_buffer);
                __ONLINE_oPlayer.__ONLINE_name = po_buffer_read_string(__ONLINE_buffer);
            }
            break;
        default:
            show_message("Received unexpected data from the server.");
    }
}
if (po_udpsocket_get_state(__ONLINE_udpsocket) != 1)
{
    show_message("Connection to the UDP socket lost.");
    game_end();
    exit;
}

using System.Security.Cryptography;
using Fangame.ModLoader;
using Fangame.ModLoader.GM8;
using UndertaleModLib;

namespace PlayOnline;

public class PlayOnlineMod : Mod
{
    Config Config = null!;
    string GameId = "";
    string GameName = "";

    public override void Load()
    {
        Config = LoadConfig(new Config());
        if (GM8Data != null)
        {
            ModGM8(GM8Data);
        }
        if (UndertaleData != null)
        {
            ModGMS(UndertaleData);
        }
    }

    public void ModGM8(GM8Data data)
    {
        GameId = GetGameDataHash();
        GameName = Path.GetFileNameWithoutExtension(ExecutablePath);

        // Extensions
        using (GM8Stream s = GM8Stream.FromFile(Path.Combine(ModDirectory, "http.ext8")))
        {
            data.Extensions.Add(new GM8Extension(s));
        }

        using (GM8Stream s = GM8Stream.FromFile(Path.Combine(ModDirectory, "dialog.ext8")))
        {
            data.Extensions.Add(new GM8Extension(s));
        }

        // Fonts
        using (GM8Stream s = GM8Stream.FromFile(Path.Combine(ModDirectory, "font.font8")))
        {
            data.Fonts.Add(new GM8Font(s, data.Version));
        }

        // Sounds
        using (GM8Stream s = GM8Stream.FromFile(Path.Combine(ModDirectory, "chatbox.snd8")))
        {
            data.Sounds.Add(new GM8Sound(s));
        }

        using (GM8Stream s = GM8Stream.FromFile(Path.Combine(ModDirectory, "saved.snd8")))
        {
            data.Sounds.Add(new GM8Sound(s));
        }

        // Objects
        foreach (var obj in data.Objects)
        {
            // World
            if (obj?.Name is "world" or "objWorld" or "World" or "oWorld")
            {
                obj.GetEventOrAdd(GM8ObjectEventType.Create).AddCode(Code_World_Create());
                obj.GetEventOrAdd(GM8ObjectEventType.Step, (int)GM8ObjectEventSubtypeStep.StepEnd).AddCode(Code_World_EndStep());
                obj.GetEventOrAdd(GM8ObjectEventType.Other, (int)GM8ObjectEventSubtypeOther.GameEnd).AddCode(Code_World_GameEnd());
            }
        }

        // Online Player
        GM8Object onlinePlayer = new GM8Object
        {
            Name = "__ONLINE_onlinePlayer",
            Persistent = true,
            Depth = -10,
        };
        onlinePlayer.GetEventOrAdd(GM8ObjectEventType.Create).AddCode(Code_OnlinePlayer_Create());
        onlinePlayer.GetEventOrAdd(GM8ObjectEventType.Step, (int)GM8ObjectEventSubtypeStep.StepEnd).AddCode(Code_OnlinePlayer_EndStep());
        onlinePlayer.GetEventOrAdd(GM8ObjectEventType.Draw).AddCode(Code_OnlinePlayer_Draw());
        data.Objects.Add(onlinePlayer);

        // Chatbox
        GM8Object chatbox = new GM8Object
        {
            Name = "__ONLINE_chatbox",
            Visible = true,
            Depth = -11,
            Persistent = true,
        };
        chatbox.GetEventOrAdd(GM8ObjectEventType.Create).AddCode(Code_Chatbox_Create());
        chatbox.GetEventOrAdd(GM8ObjectEventType.Step, (int)GM8ObjectEventSubtypeStep.StepEnd).AddCode(Code_Chatbox_EndStep());
        chatbox.GetEventOrAdd(GM8ObjectEventType.Draw).AddCode(Code_Chatbox_Draw());
        data.Objects.Add(chatbox);

        // Player Saved
        GM8Object playerSaved = new GM8Object
        {
            Name = "__ONLINE_playerSaved",
            Visible = true,
            Depth = -10,
        };
        chatbox.GetEventOrAdd(GM8ObjectEventType.Step, (int)GM8ObjectEventSubtypeStep.StepEnd).AddCode(Code_PlayerSaved_EndStep());
        chatbox.GetEventOrAdd(GM8ObjectEventType.Draw).AddCode(Code_PlayerSaved_Draw());
        data.Objects.Add(playerSaved);

        // Scripts
        foreach (var script in data.Scripts)
        {
            // Save Game
            if (script?.Name is "saveGame")
            {
                script.Source += '\n' + Code_SaveGame();
            }

            // Load Game
            if (script?.Name is "loadGame")
            {
                script.Source += '\n' + Code_LoadGame();
            }
        }
    }

    public void ModGMS(UndertaleData data)
    {
    }

    private string GetGameDataHash()
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(GameDataPath);
        var hash = md5.ComputeHash(stream);
        return Convert.ToHexStringLower(hash);
    }

    // GML Code

    private string Code_World_Create()
    {
        return $$"""
            /// ONLINE
            // {{GameId}}: The ID of the game
            // {{Config.ServerIp}}: The server
            // {{Config.TcpPort}}: The TCP port
            // {{Config.UdpPort}}: The UDP port
            // {{GameName}}: The game name
            // {{Config.Version}}: The version 
            __ONLINE_connected = false;
            __ONLINE_buffer = buffer_create();
            __ONLINE_selfID = "";
            __ONLINE_name = "";
            __ONLINE_selfGameID = "{{GameId}}";
            __ONLINE_server = "{{Config.ServerIp}}";
            __ONLINE_version = "{{Config.Version}}";
            __ONLINE_race = false;
            __ONLINE_vis = 0;
            __ONLINE_save_enabled = 1;
            if (file_exists("tempOnline"))
            {
                buffer_read_from_file(__ONLINE_buffer, "tempOnline");
                __ONLINE_socket = buffer_read_uint16(__ONLINE_buffer);
                __ONLINE_udpsocket = buffer_read_uint16(__ONLINE_buffer);
                __ONLINE_selfID = buffer_read_string(__ONLINE_buffer);
                __ONLINE_name = buffer_read_string(__ONLINE_buffer);
                __ONLINE_selfGameID = buffer_read_string(__ONLINE_buffer);
                __ONLINE_race = buffer_read_uint8(__ONLINE_buffer);
                __ONLINE_n = buffer_read_uint16(__ONLINE_buffer);
                __ONLINE_vis = buffer_read_uint16(__ONLINE_buffer);
                __ONLINE_save_enabled = buffer_read_uint16(__ONLINE_buffer);
                for (__ONLINE_i = 0; __ONLINE_i < __ONLINE_n; __ONLINE_i += 1)
                {
                    __ONLINE_oPlayer = instance_create(0, 0, __ONLINE_onlinePlayer);
                    __ONLINE_oPlayer.__ONLINE_ID = buffer_read_string(__ONLINE_buffer);
                    __ONLINE_oPlayer.x = buffer_read_int32(__ONLINE_buffer);
                    __ONLINE_oPlayer.y = buffer_read_int32(__ONLINE_buffer);
                    __ONLINE_oPlayer.sprite_index = buffer_read_int32(__ONLINE_buffer);
                    __ONLINE_oPlayer.image_speed = buffer_read_float32(__ONLINE_buffer);
                    __ONLINE_oPlayer.image_xscale = buffer_read_float32(__ONLINE_buffer);
                    __ONLINE_oPlayer.image_yscale = buffer_read_float32(__ONLINE_buffer);
                    __ONLINE_oPlayer.image_angle = buffer_read_float32(__ONLINE_buffer);
                    __ONLINE_oPlayer.__ONLINE_oRoom = buffer_read_uint16(__ONLINE_buffer);
                    __ONLINE_oPlayer.__ONLINE_name = buffer_read_string(__ONLINE_buffer);
                }
            }
            else
            {
                __ONLINE_socket = socket_create();
                socket_connect(__ONLINE_socket, __ONLINE_server, {{Config.TcpPort}});
                __ONLINE_name = wd_input_box("Name", "Enter your name:", "");
                if (__ONLINE_name == "")
                {
                    __ONLINE_name = "Anonymous";
                }
                __ONLINE_name = string_replace_all(__ONLINE_name, "#", "\#");
                if (string_length(__ONLINE_name) > 20)
                {
                    __ONLINE_name = string_copy(__ONLINE_name, 0, 20);
                }
                __ONLINE_password = wd_input_box("Password", "Leave it empty for no password:", "");
                if (string_length(__ONLINE_password) > 20)
                {
                    __ONLINE_password = string_copy(__ONLINE_password, 0, 20);
                }
                __ONLINE_selfGameID += __ONLINE_password;
                wd_message_set_text("Do you want to enable RACE mod? (shared saves will be disabled)");
                __ONLINE_race = wd_message_show(wd_mk_information, wd_mb_yes, wd_mb_no, 0) == wd_mb_yes;
                buffer_clear(__ONLINE_buffer);
                buffer_write_uint8(__ONLINE_buffer, 3);
                buffer_write_string(__ONLINE_buffer, __ONLINE_name);
                buffer_write_string(__ONLINE_buffer, __ONLINE_selfGameID);
                buffer_write_string(__ONLINE_buffer, "i wanna be the engine yuuutu edition");
                buffer_write_string(__ONLINE_buffer, __ONLINE_version);
                buffer_write_uint8(__ONLINE_buffer, __ONLINE_password != "");
                socket_write_message(__ONLINE_socket, __ONLINE_buffer);
                __ONLINE_udpsocket = udpsocket_create();
                udpsocket_start(__ONLINE_udpsocket, false, 0);
                udpsocket_set_destination(__ONLINE_udpsocket, __ONLINE_server, {{Config.UdpPort}});
                buffer_clear(__ONLINE_buffer);
                buffer_write_uint8(__ONLINE_buffer, 0);
                udpsocket_send(__ONLINE_udpsocket, __ONLINE_buffer);
            }
            __ONLINE_pExists = false;
            __ONLINE_pX = 0;
            __ONLINE_pY = 0;
            __ONLINE_t = 0;
            __ONLINE_heartbeat = 0;
            __ONLINE_stoppedFrames = 0;
            __ONLINE_sGravity = 0;
            __ONLINE_sX = 0;
            __ONLINE_sY = 0;
            __ONLINE_sRoom = 0;
            __ONLINE_sSaved = false;
            """;
    }

    private string Code_World_EndStep()
    {
        return """
            /// ONLINE
            // player: The name of the player object
            // player2: The name of the player2 object if it exists
            // TCP SOCKETS 
            socket_update_read(__ONLINE_socket);
            while (socket_read_message(__ONLINE_socket, __ONLINE_buffer))
            {
                switch (buffer_read_uint8(__ONLINE_buffer))
                {
                    case 0: // CREATED 
                        __ONLINE_ID = buffer_read_string(__ONLINE_buffer);
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
                            __ONLINE_oPlayer.__ONLINE_name = buffer_read_string(__ONLINE_buffer);
                        }
                        break;
                    case 1: // DESTROYED 
                        __ONLINE_ID = buffer_read_string(__ONLINE_buffer);
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
                        __ONLINE_lastVersion = buffer_read_string(__ONLINE_buffer);
                        __ONLINE_errorMessage = "Your tool uses the version " + __ONLINE_version + " but the oldest compatible version is " + __ONLINE_lastVersion + ". Please update your tool.";
                        wd_message_simple(__ONLINE_errorMessage);
                        game_end();
                        exit;
                        break;
                    case 4: // CHAT MESSAGE 
                        __ONLINE_ID = buffer_read_string(__ONLINE_buffer);
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
                            __ONLINE_message = buffer_read_string(__ONLINE_buffer);
                            __ONLINE_oChatbox = instance_create(0, 0, __ONLINE_chatbox);
                            __ONLINE_oChatbox.__ONLINE_message = __ONLINE_message;
                            __ONLINE_oChatbox.__ONLINE_follower = __ONLINE_oPlayer;
                            if (__ONLINE_oPlayer.visible)
                            {
                                sound_play(__ONLINE_sndChatbox);
                            }
                        }
                        break;
                    case 5: // SOMEONE SAVED 
                        if (!__ONLINE_race)
                        {
                            __ONLINE_sSaved = true;
                            __ONLINE_sGravity = buffer_read_uint8(__ONLINE_buffer);
                            __ONLINE_sName = buffer_read_string(__ONLINE_buffer);
                            __ONLINE_sX = buffer_read_int32(__ONLINE_buffer);
                            __ONLINE_sY = buffer_read_float64(__ONLINE_buffer);
                            __ONLINE_sRoom = buffer_read_int16(__ONLINE_buffer);
                            __ONLINE_a = instance_create(0, 0, __ONLINE_playerSaved);
                            __ONLINE_a.__ONLINE_name = __ONLINE_sName;
                            __ONLINE_a.__ONLINE_state = -1;
                            buffer_clear(__ONLINE_buffer);
                            buffer_write_uint8(__ONLINE_buffer, __ONLINE_sGravity);
                            buffer_write_int32(__ONLINE_buffer, __ONLINE_sX);
                            buffer_write_float64(__ONLINE_buffer, __ONLINE_sY);
                            buffer_write_int16(__ONLINE_buffer, __ONLINE_sRoom);
                            buffer_write_to_file(__ONLINE_buffer, "tempOnline2");
                            sound_play(__ONLINE_sndSaved);
                        }
                        break;
                    case 6: // SELF ID 
                        __ONLINE_selfID = buffer_read_string(__ONLINE_buffer);
                        break;
                }
            }
            __ONLINE_mustQuit = false;
            switch (socket_get_state(__ONLINE_socket))
            {
                case 2:
                    if (!__ONLINE_connected)
                    {
                        __ONLINE_connected = true;
                    }
                    break;
                case 4:
                    wd_message_simple("Connection closed.");
                    __ONLINE_mustQuit = true;
                    break;
                case 5:
                    socket_reset(__ONLINE_socket);
                    __ONLINE_errorMessage = "Could not connect to the server.";
                    if (__ONLINE_connected)
                    {
                        __ONLINE_errorMessage = "Connection lost";
                    }
                    wd_message_simple(__ONLINE_errorMessage);
                    __ONLINE_mustQuit = true;
                    break;
            }
            if (__ONLINE_mustQuit)
            {
                if (file_exists("temp"))
                {
                    file_delete("temp");
                }
                game_end();
                exit;
            }
            __ONLINE_p = player;
            if (!instance_exists(__ONLINE_p))
            {
                __ONLINE_p = player2;
            }
            __ONLINE_exists = instance_exists(__ONLINE_p);
            __ONLINE_X = __ONLINE_pX;
            __ONLINE_Y = __ONLINE_pY;
            if (__ONLINE_exists)
            {
                if (__ONLINE_exists != __ONLINE_pExists)
                {
                    // SEND PLAYER CREATE
                    buffer_clear(__ONLINE_buffer);
                    buffer_write_uint8(__ONLINE_buffer, 0);
                    socket_write_message(__ONLINE_socket, __ONLINE_buffer);
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
                        buffer_clear(__ONLINE_buffer);
                        buffer_write_uint8(__ONLINE_buffer, 1);
                        buffer_write_string(__ONLINE_buffer, __ONLINE_selfID);
                        buffer_write_string(__ONLINE_buffer, __ONLINE_selfGameID);
                        buffer_write_uint16(__ONLINE_buffer, room);
                        buffer_write_uint64(__ONLINE_buffer, current_time);
                        buffer_write_int32(__ONLINE_buffer, __ONLINE_X);
                        buffer_write_int32(__ONLINE_buffer, __ONLINE_Y);
                        buffer_write_int32(__ONLINE_buffer, __ONLINE_p.sprite_index);
                        buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_speed);
                        buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_xscale);
                        buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_yscale);
                        buffer_write_float32(__ONLINE_buffer, __ONLINE_p.image_angle);
                        buffer_write_string(__ONLINE_buffer, __ONLINE_name);
                        udpsocket_send(__ONLINE_udpsocket, __ONLINE_buffer);
                    }
                }
                __ONLINE_t += 1;
                if (keyboard_check_pressed(vk_space))
                {
                    __ONLINE_message = wd_input_box("Chat", "Say something:", "");
                    __ONLINE_message = string_replace_all(__ONLINE_message, "#", "\\#");
                    __ONLINE_message_length = string_length(__ONLINE_message);
                    if (__ONLINE_message_length > 0)
                    {
                        __ONLINE_message_max_length = 300;
                        if (__ONLINE_message_length > __ONLINE_message_max_length)
                        {
                            __ONLINE_message = string_copy(__ONLINE_message, 0, __ONLINE_message_max_length);
                        }
                        buffer_clear(__ONLINE_buffer);
                        buffer_write_uint8(__ONLINE_buffer, 4);
                        buffer_write_string(__ONLINE_buffer, __ONLINE_message);
                        socket_write_message(__ONLINE_socket, __ONLINE_buffer);
                        __ONLINE_oChatbox = instance_create(0, 0, __ONLINE_chatbox);
                        __ONLINE_oChatbox.__ONLINE_message = __ONLINE_message;
                        __ONLINE_oChatbox.__ONLINE_follower = __ONLINE_p;
                        sound_play(__ONLINE_sndChatbox);
                    }
                }
            }
            else
            {
                if (__ONLINE_exists != __ONLINE_pExists)
                {
                    // SEND PLAYER DESTROYED
                    buffer_clear(__ONLINE_buffer);
                    buffer_write_uint8(__ONLINE_buffer, 1);
                    socket_write_message(__ONLINE_socket, __ONLINE_buffer);
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
                buffer_clear(__ONLINE_buffer);
                buffer_write_uint8(__ONLINE_buffer, 2);

                socket_write_message(__ONLINE_socket, __ONLINE_buffer);
            }
            socket_update_write(__ONLINE_socket);

            // UDP SOCKETS 
            while (udpsocket_receive(__ONLINE_udpsocket, __ONLINE_buffer))
            {
                switch (buffer_read_uint8(__ONLINE_buffer))
                {
                    case 1: // RECEIVED MOVED 
                        __ONLINE_ID = buffer_read_string(__ONLINE_buffer);
                        __ONLINE_gameID = buffer_read_string(__ONLINE_buffer);
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
                        __ONLINE_oPlayer.__ONLINE_oRoom = buffer_read_uint16(__ONLINE_buffer);
                        __ONLINE_syncTime = buffer_read_uint64(__ONLINE_buffer);
                        if (__ONLINE_oPlayer.__ONLINE_syncTime < __ONLINE_syncTime)
                        {
                            __ONLINE_oPlayer.__ONLINE_syncTime = __ONLINE_syncTime;
                            __ONLINE_oPlayer.x = buffer_read_int32(__ONLINE_buffer);
                            __ONLINE_oPlayer.y = buffer_read_int32(__ONLINE_buffer);
                            __ONLINE_oPlayer.sprite_index = buffer_read_int32(__ONLINE_buffer);
                            __ONLINE_oPlayer.image_speed = buffer_read_float32(__ONLINE_buffer);
                            __ONLINE_oPlayer.image_xscale = buffer_read_float32(__ONLINE_buffer);
                            __ONLINE_oPlayer.image_yscale = buffer_read_float32(__ONLINE_buffer);
                            __ONLINE_oPlayer.image_angle = buffer_read_float32(__ONLINE_buffer);
                            __ONLINE_oPlayer.__ONLINE_name = buffer_read_string(__ONLINE_buffer);
                        }
                        break;
                    default:
                        wd_message_simple("Received unexpected data from the server.");
                }
            }
            if (udpsocket_get_state(__ONLINE_udpsocket) != 1)
            {
                wd_message_simple("Connection to the UDP socket lost.");
                game_end();
                exit;
            }
            if (keyboard_check_pressed(ord('V')))
            {
                if (__ONLINE_vis == 0)
                    __ONLINE_vis = 1;
                else if (__ONLINE_vis == 1)
                    __ONLINE_vis = 2;
                else if (__ONLINE_vis == 2)
                    __ONLINE_vis = 0;
                __ONLINE_a = instance_create(0, 0, __ONLINE_playerSaved);
                __ONLINE_a.__ONLINE_state = __ONLINE_vis;
            }
            if (keyboard_check_pressed(ord('T')))
            {
                __ONLINE_save_enabled = 1 - __ONLINE_save_enabled;
                __ONLINE_a = instance_create(0, 0, __ONLINE_playerSaved);
                __ONLINE_a.__ONLINE_state = __ONLINE_save_enabled + 3;
            }
            """;
    }

    private string Code_World_GameEnd()
    {
        return """
            /// ONLINE 
            if (!file_exists("temp") && !file_exists(working_directory + "\save\temp") && !file_exists("temp.dat"))
            {
                if (file_exists("tempOnline"))
                {
                    file_delete("tempOnline");
                }
                if (file_exists("tempOnline2"))
                {
                    file_delete("tempOnline2");
                }
            }
            buffer_destroy(__ONLINE_buffer);
            if (!file_exists("tempOnline"))
            {
                socket_destroy(__ONLINE_socket);
                udpsocket_destroy(__ONLINE_udpsocket);
            }
            """;
    }

    private string Code_OnlinePlayer_Create()
    {
        return """
            /// ONLINE 
            __ONLINE_alpha = 1;
            __ONLINE_oRoom = -1;
            __ONLINE_name = "";
            __ONLINE_syncTime = 0;
            """;
    }

    private string Code_OnlinePlayer_EndStep()
    {
        return """
            /// ONLINE
            // player: The name of the player object
            // player2: The name of the player2 object if it exists 
            visible = __ONLINE_oRoom == room;
            image_alpha = __ONLINE_alpha;
            __ONLINE_p = player;
            if (!instance_exists(__ONLINE_p))
            {
                __ONLINE_p = player2;
            }
            if (instance_exists(__ONLINE_p))
            {
                __ONLINE_dist = distance_to_object(__ONLINE_p);
                image_alpha = min(__ONLINE_alpha, __ONLINE_dist / 100);
            }
            """;
    }

    private string Code_OnlinePlayer_Draw()
    {
        return """
            /// ONLINE
            // world: The name of the world object 
            if (world.__ONLINE_vis <= 1)
            {
                if (sprite_exists(sprite_index))
                {
                    draw_sprite_ext(sprite_index, image_index, x, y, image_xscale, image_yscale, image_angle, c_white, image_alpha);
                    if (world.__ONLINE_vis == 0)
                    {
                        __ONLINE__alpha = draw_get_alpha();
                        __ONLINE__color = draw_get_color();
                        draw_set_alpha(image_alpha);
                        draw_set_font(__ONLINE_ftOnlinePlayerName);
                        draw_set_valign(fa_center);
                        draw_set_halign(fa_center);
                        draw_set_color(c_black);
                        __ONLINE_border = 2;
                        __ONLINE_padding = 30;
                        __ONLINE_xx = x;
                        __ONLINE_yy = y - __ONLINE_padding;
                        draw_set_alpha(1);
                        draw_text(__ONLINE_xx + __ONLINE_border, __ONLINE_yy, __ONLINE_name);
                        draw_text(__ONLINE_xx, __ONLINE_yy + __ONLINE_border, __ONLINE_name);
                        draw_text(__ONLINE_xx - __ONLINE_border, __ONLINE_yy, __ONLINE_name);
                        draw_text(__ONLINE_xx, __ONLINE_yy - __ONLINE_border, __ONLINE_name);
                        draw_set_color(c_white);
                        draw_text(__ONLINE_xx, __ONLINE_yy, __ONLINE_name);
                        draw_set_alpha(__ONLINE__alpha);
                        draw_set_color(__ONLINE__color);
                        if (font_exists(0))
                        {
                            draw_set_font(0);
                        }
                        draw_set_valign(fa_top);
                        draw_set_halign(fa_left);
                    }
                }
            }
            """;
    }

    private string Code_Chatbox_Create()
    {
        return """
            /// ONLINE 
            __ONLINE_paddingText = 10;
            __ONLINE_width = 250;
            __ONLINE_padding = 15;
            __ONLINE_alpha = 1;
            __ONLINE_fade = false;
            __ONLINE_fadeAlpha = 1;
            __ONLINE_t = 70;
            __ONLINE_sep = -1;
            __ONLINE_maxTextWidth = __ONLINE_width - 2 * __ONLINE_paddingText;
            __ONLINE_hasDestroyed = false;
            """;
    }

    private string Code_Chatbox_EndStep()
    {
        return """
            /// ONLINE
            // player: The name of the player object
            // player2: The name of the player2 object if it exists 
            __ONLINE_f = __ONLINE_follower;
            if (__ONLINE_f == player && !instance_exists(__ONLINE_f))
            {
                __ONLINE_f = player2;
            }
            if (instance_exists(__ONLINE_f))
            {
                x = __ONLINE_f.x;
                y = __ONLINE_f.y;
            }
            else
            {
                instance_destroy();
                exit;
            }
            if (__ONLINE_fade)
            {
                __ONLINE_fadeAlpha -= 0.02;
                if (__ONLINE_fadeAlpha <= 0)
                {
                    instance_destroy();
                    exit;
                }
            }
            __ONLINE_alpha = 1;
            if (__ONLINE_follower != player)
            {
                visible = __ONLINE_follower.visible;
                __ONLINE_p = player;
                __ONLINE_p = player2;
                if (instance_exists(__ONLINE_p))
                {
                    __ONLINE_dist = distance_to_object(__ONLINE_p);
                    __ONLINE_alpha = __ONLINE_dist / 100;
                }
            }
            __ONLINE_t -= 1;
            if (__ONLINE_t < 0)
            {
                __ONLINE_fade = true;
            }

            // Destroy all other chatboxes of the same player 
            if (!__ONLINE_hasDestroyed)
            {
                __ONLINE_found = false;
                __ONLINE_oChatbox = 0;
                for (__ONLINE_i = 0; __ONLINE_i < instance_number(__ONLINE_chatbox) && !__ONLINE_found; __ONLINE_i += 1)
                {
                    __ONLINE_oChatbox = instance_find(__ONLINE_chatbox, __ONLINE_i);
                    if (__ONLINE_oChatbox.__ONLINE_follower == __ONLINE_follower && __ONLINE_oChatbox.id != id)
                    {
                        __ONLINE_found = true;
                    }
                }
                if (__ONLINE_found)
                {
                    with (__ONLINE_oChatbox)
                    {
                        instance_destroy();
                    }
                }
                __ONLINE_hasDestroyed = true;
            }
            """;
    }

    private string Code_Chatbox_Draw()
    {
        return """
                        /// ONLINE 
            draw_set_font(__ONLINE_ftOnlinePlayerName);
            __ONLINE_textHeight = string_height_ext(__ONLINE_message, __ONLINE_sep, __ONLINE_maxTextWidth);

            __ONLINE_height = __ONLINE_textHeight + 2 * __ONLINE_paddingText;
            __ONLINE_yOffset = -__ONLINE_height / 2 + 60;
            __ONLINE_left = 0;
            __ONLINE_right = room_width;
            __ONLINE_top = 0;
            __ONLINE_bottom = room_height;
            if (view_enabled && view_visible[0])
            {
                __ONLINE_left = view_xview[0];
                __ONLINE_right = __ONLINE_left + view_wview[0];
                __ONLINE_top = view_yview[0];
                __ONLINE_bottom = __ONLINE_top + view_hview[0];
            }
            __ONLINE_xx = min(max(x, __ONLINE_left + __ONLINE_width / 2 + __ONLINE_padding), __ONLINE_right - __ONLINE_width / 2 - __ONLINE_padding);
            __ONLINE_yy = min(max(y - __ONLINE_yOffset, __ONLINE_top + __ONLINE_height / 2 + __ONLINE_padding), __ONLINE_bottom - __ONLINE_height / 2 - __ONLINE_padding);
            __ONLINE__alpha = draw_get_alpha();
            __ONLINE__color = draw_get_color();
            draw_set_alpha(min(__ONLINE_alpha, __ONLINE_fadeAlpha));
            draw_set_color(c_white);
            draw_rectangle(__ONLINE_xx - __ONLINE_width / 2, __ONLINE_yy - __ONLINE_height / 2, __ONLINE_xx + __ONLINE_width / 2, __ONLINE_yy + __ONLINE_height / 2, false);
            draw_set_color(c_black);
            draw_rectangle(__ONLINE_xx - __ONLINE_width / 2, __ONLINE_yy - __ONLINE_height / 2, __ONLINE_xx + __ONLINE_width / 2, __ONLINE_yy + __ONLINE_height / 2, true);
            draw_set_valign(fa_center);
            draw_set_halign(fa_center);
            draw_text_ext(__ONLINE_xx, __ONLINE_yy, __ONLINE_message, __ONLINE_sep, __ONLINE_maxTextWidth);
            draw_set_alpha(__ONLINE__alpha);
            draw_set_color(__ONLINE__color);
            if (font_exists(0))
            {
                draw_set_font(0);
            }
            draw_set_valign(fa_top);
            draw_set_halign(fa_left);
            """;
    }

    private string Code_PlayerSaved_EndStep()
    {
        return """
            /// ONLINE 
            image_alpha -= 0.01;
            if (image_alpha <= 0)
            {
                instance_destroy();
            }
            """;
    }

    private string Code_PlayerSaved_Draw()
    {
        return """
            /// ONLINE 
            __ONLINE_xx = 20;
            __ONLINE_yy = 20;
            if (view_enabled && view_visible[0])
            {
                __ONLINE_xx += view_xview[0];
                __ONLINE_yy += view_yview[0];
            }
            __ONLINE_text = "hjw";
            if (__ONLINE_state == 4)
                __ONLINE_text = "Online save enabled!";
            else if (__ONLINE_state == 3)
                __ONLINE_text = "Online save disabled!";
            else if (__ONLINE_state >= 0)
                __ONLINE_text = "player visual mode: " + string(__ONLINE_state);
            else
                __ONLINE_text = __ONLINE_name + " saved!";
            __ONLINE__alpha = draw_get_alpha();
            __ONLINE__color = draw_get_color();
            draw_set_valign(fa_top);
            draw_set_halign(fa_left);
            draw_set_alpha(image_alpha);
            draw_set_font(__ONLINE_ftOnlinePlayerName);
            draw_set_color(c_black);
            draw_text(__ONLINE_xx + 1, __ONLINE_yy, __ONLINE_text);
            draw_text(__ONLINE_xx, __ONLINE_yy + 1, __ONLINE_text);
            draw_text(__ONLINE_xx - 1, __ONLINE_yy, __ONLINE_text);
            draw_text(__ONLINE_xx, __ONLINE_yy - 1, __ONLINE_text);
            draw_set_color(c_white);
            draw_text(__ONLINE_xx, __ONLINE_yy, __ONLINE_text);
            draw_set_alpha(__ONLINE__alpha);
            draw_set_color(__ONLINE__color);
            if (font_exists(0))
            {
                draw_set_font(0);
            }
            draw_set_valign(fa_top);
            draw_set_halign(fa_left);
            """;
    }

    private string Code_SaveGame()
    {
        return """
            /// ONLINE
            // world: The name of the world object
            // player: The name of the player object
            // player2: The name of the player2 object if it exists 
            if (!world.__ONLINE_race)
            {
                if (room != rSelectStage)
                {
                    buffer_clear(world.__ONLINE_buffer);
                    __ONLINE_p = player;
                    if (!instance_exists(__ONLINE_p))
                    {
                        __ONLINE_p = player2;
                    }
                    if (instance_exists(__ONLINE_p))
                    {
                        buffer_write_uint8(world.__ONLINE_buffer, 5);
                        if (__ONLINE_p == player)
                        {
                            buffer_write_uint8(world.__ONLINE_buffer, 0);
                        }
                        else
                        {
                            buffer_write_uint8(world.__ONLINE_buffer, 1);
                        }
                        buffer_write_int32(world.__ONLINE_buffer, __ONLINE_p.x);
                        buffer_write_float64(world.__ONLINE_buffer, __ONLINE_p.y);
                        buffer_write_int16(world.__ONLINE_buffer, room);
                        socket_write_message(world.__ONLINE_socket, world.__ONLINE_buffer);
                    }
                }
            }
            """;
    }

    private string Code_LoadGame()
    {
        return """
            /// ONLINE
            // world: The name of the world object 
            with (world)
            {
                buffer_clear(__ONLINE_buffer);
                buffer_write_uint16(__ONLINE_buffer, __ONLINE_socket);
                buffer_write_uint16(__ONLINE_buffer, __ONLINE_udpsocket);
                buffer_write_string(__ONLINE_buffer, __ONLINE_selfID);
                buffer_write_string(__ONLINE_buffer, __ONLINE_name);
                buffer_write_string(__ONLINE_buffer, __ONLINE_selfGameID);
                buffer_write_uint8(__ONLINE_buffer, __ONLINE_race);
                __ONLINE_n = instance_number(__ONLINE_onlinePlayer);
                buffer_write_uint16(__ONLINE_buffer, __ONLINE_n);
                buffer_write_uint16(__ONLINE_buffer, __ONLINE_vis);
                buffer_write_uint16(__ONLINE_buffer, __ONLINE_save_enabled);
                for (__ONLINE_i = 0; __ONLINE_i < __ONLINE_n; __ONLINE_i += 1)
                {
                    __ONLINE_oPlayer = instance_find(__ONLINE_onlinePlayer, __ONLINE_i);
                    buffer_write_string(__ONLINE_buffer, __ONLINE_oPlayer.__ONLINE_ID);
                    buffer_write_int32(__ONLINE_buffer, __ONLINE_oPlayer.x);
                    buffer_write_int32(__ONLINE_buffer, __ONLINE_oPlayer.y);
                    buffer_write_int32(__ONLINE_buffer, __ONLINE_oPlayer.sprite_index);
                    buffer_write_float32(__ONLINE_buffer, __ONLINE_oPlayer.image_speed);
                    buffer_write_float32(__ONLINE_buffer, __ONLINE_oPlayer.image_xscale);
                    buffer_write_float32(__ONLINE_buffer, __ONLINE_oPlayer.image_yscale);
                    buffer_write_float32(__ONLINE_buffer, __ONLINE_oPlayer.image_angle);
                    buffer_write_uint16(__ONLINE_buffer, __ONLINE_oPlayer.__ONLINE_oRoom);
                    buffer_write_string(__ONLINE_buffer, __ONLINE_oPlayer.__ONLINE_name);
                }
                buffer_write_to_file(__ONLINE_buffer, "tempOnline");
            }

            /// ONLINE
            // world: The name of the world object
            // player: The name of the player object
            // player2: The name of the player2 object if it exists 
            if (world.__ONLINE_save_enabled)
            {
                if (file_exists("tempOnline2"))
                {
                    buffer_clear(world.__ONLINE_buffer);
                    buffer_read_from_file(world.__ONLINE_buffer, "tempOnline2");
                    world.__ONLINE_sGravity = buffer_read_uint8(world.__ONLINE_buffer);
                    world.__ONLINE_sX = buffer_read_int32(world.__ONLINE_buffer);
                    world.__ONLINE_sY = buffer_read_float64(world.__ONLINE_buffer);
                    world.__ONLINE_sRoom = buffer_read_int16(world.__ONLINE_buffer);
                    file_delete("tempOnline2");
                    if (room_exists(world.__ONLINE_sRoom))
                    {
                        __ONLINE_p = player;
                        if (world.__ONLINE_sGravity == 1)
                        {
                            instance_create(0, 0, player2);
                            with (player)
                            {
                                instance_destroy();
                            }
                            __ONLINE_p = player2;
                        }
                        global.grav = world.__ONLINE_sGravity;
                        __ONLINE_p.x = world.__ONLINE_sX;
                        __ONLINE_p.y = world.__ONLINE_sY;
                        room_goto(world.__ONLINE_sRoom);
                    }
                    world.__ONLINE_sSaved = false;
                }
            }
            
            """;
    }
}

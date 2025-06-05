/// ONLINE

// Macros
global.PO_GAME_ID = "{GAME_ID}";
global.PO_GAME_NAME = "{GAME_NAME}";
global.PO_SERVER = "{SERVER}";
global.PO_VERSION = "{VERSION}";
global.PO_NAME = "{NAME}";
global.PO_PASSWORD = "{PASSWORD}";
global.PO_RACE = {RACE};
global.PO_TCP_PORT = {TCP_PORT};
global.PO_UDP_PORT = {UDP_PORT};
global.PO_TEMP_FILE = {TEMP_FILE};
global.PO_WORLD = {WORLD};
global.PO_PLAYER = {PLAYER};
global.PO_PLAYER2 = {PLAYER2};
global.PO_GLOBAL_PLAYER_XSCALE = {GLOBAL_PLAYER_XSCALE};
global.PO_YOYOYO = {YOYOYO};
global.PO_RENEX = {RENEX};

// Initialization
po_http_dll_init();
__ONLINE_connected = false;
__ONLINE_buffer = po_buffer_create();
__ONLINE_selfID = "";
__ONLINE_name = "";
__ONLINE_selfGameID = global.PO_GAME_ID;
__ONLINE_server = global.PO_SERVER;
__ONLINE_version = global.PO_VERSION;
__ONLINE_race = false;
if (file_exists("tempOnline"))
{
    po_buffer_read_from_file(__ONLINE_buffer, "tempOnline");
    __ONLINE_socket = po_buffer_read_uint16(__ONLINE_buffer);
    __ONLINE_udpsocket = po_buffer_read_uint16(__ONLINE_buffer);
    __ONLINE_selfID = po_buffer_read_string(__ONLINE_buffer);
    __ONLINE_name = po_buffer_read_string(__ONLINE_buffer);
    __ONLINE_selfGameID = po_buffer_read_string(__ONLINE_buffer);
    __ONLINE_race = po_buffer_read_uint8(__ONLINE_buffer);
    __ONLINE_n = po_buffer_read_uint16(__ONLINE_buffer);
    for (__ONLINE_i = 0; __ONLINE_i < __ONLINE_n; __ONLINE_i += 1)
    {
        __ONLINE_oPlayer = instance_create(0, 0, __ONLINE_onlinePlayer);
        __ONLINE_oPlayer.__ONLINE_ID = po_buffer_read_string(__ONLINE_buffer);
        __ONLINE_oPlayer.x = po_buffer_read_int32(__ONLINE_buffer);
        __ONLINE_oPlayer.y = po_buffer_read_int32(__ONLINE_buffer);
        __ONLINE_oPlayer.sprite_index = po_buffer_read_int32(__ONLINE_buffer);
        __ONLINE_oPlayer.image_speed = po_buffer_read_float32(__ONLINE_buffer);
        __ONLINE_oPlayer.image_xscale = po_buffer_read_float32(__ONLINE_buffer);
        __ONLINE_oPlayer.image_yscale = po_buffer_read_float32(__ONLINE_buffer);
        __ONLINE_oPlayer.image_angle = po_buffer_read_float32(__ONLINE_buffer);
        __ONLINE_oPlayer.__ONLINE_oRoom = po_buffer_read_uint16(__ONLINE_buffer);
        __ONLINE_oPlayer.__ONLINE_name = po_buffer_read_string(__ONLINE_buffer);
    }
}
else
{
    __ONLINE_socket = po_socket_create();
    po_socket_connect(__ONLINE_socket, __ONLINE_server, global.PO_TCP_PORT);
    __ONLINE_name = global.PO_NAME;
    if (__ONLINE_name == "")
    {
        __ONLINE_name = "Anonymous";
    }
    __ONLINE_name = string_replace_all(__ONLINE_name, "#", "\#");
    if (string_length(__ONLINE_name) > 20)
    {
        __ONLINE_name = string_copy(__ONLINE_name, 0, 20);
    }
    __ONLINE_password = global.PO_PASSWORD;
    if (string_length(__ONLINE_password) > 20)
    {
        __ONLINE_password = string_copy(__ONLINE_password, 0, 20);
    }
    __ONLINE_selfGameID += __ONLINE_password;
    __ONLINE_race = global.PO_RACE;
    po_buffer_clear(__ONLINE_buffer);
    po_buffer_write_uint8(__ONLINE_buffer, 3);
    po_buffer_write_string(__ONLINE_buffer, __ONLINE_name);
    po_buffer_write_string(__ONLINE_buffer, __ONLINE_selfGameID);
    po_buffer_write_string(__ONLINE_buffer, global.PO_GAME_NAME);
    po_buffer_write_string(__ONLINE_buffer, __ONLINE_version);
    po_buffer_write_uint8(__ONLINE_buffer, __ONLINE_password != "");
    po_socket_write_message(__ONLINE_socket, __ONLINE_buffer);
    __ONLINE_udpsocket = po_udpsocket_create();
    po_udpsocket_start(__ONLINE_udpsocket, false, 0);
    po_udpsocket_set_destination(__ONLINE_udpsocket, __ONLINE_server, global.PO_UDP_PORT);
    po_buffer_clear(__ONLINE_buffer);
    po_buffer_write_uint8(__ONLINE_buffer, 0);
    po_udpsocket_send(__ONLINE_udpsocket, __ONLINE_buffer);
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

// Assets
global.po_snd_chatbox = po_sound_add(program_directory + "/" + "po_snd_chatbox");
global.po_snd_saved = po_sound_add(program_directory + "/" + "po_snd_saved");
global.po_font_playername = po_font_add("arial");
// Initialization
po_http_dll_init();
po_load_assets();
po_config();

// Engine-specific
global.po_player = po_asset_get_index("player");               // Yuuutu / Nikaple / Better
global.po_player2 = po_asset_get_index("player2");             // Yuuutu
global.po_objPlayer = po_asset_get_index("objPlayer");         // YoYoYo
global.po_Player = po_asset_get_index("Player");               // Renex / Verve
global.po_tempExe = po_asset_get_index("tempExe");             // Yuuutu
global.po_scrFlipGrav = po_asset_get_index("scrFlipGrav");     // YoYoYo
global.po_flip_player = po_asset_get_index("flip_player");     // Renex
global.po_player_flip = po_asset_get_index("player_flip");     // Verve
global.po_global_init = po_asset_get_index("global_init");     // Better
global.po_playerReverse = po_asset_get_index("playerReverse"); // Nikaple

// Main state
global.po_connected = false;
global.po_buffer = po_buffer_create();
global.po_self_ip = "";
global.po_last_player_exists = false;
global.po_last_player_x = 0;
global.po_last_player_y = 0;
global.po_timer = 0;
global.po_heartbeat = 0;
global.po_stopped_frames = 0;

// Someone saved state
global.po_someone_saved = false;
global.po_someone_grav = 0;
global.po_someone_x = 0;
global.po_someone_y = 0;
global.po_someone_room = 0;

if (!po_temp_socketstate_load())
{
    // Create tcp socket
    global.po_socket = po_socket_create();
    po_socket_connect(global.po_socket, global.po_server, global.po_tcp_port);

    // Send name
    po_buffer_clear(global.po_buffer);
    po_buffer_write_uint8(global.po_buffer, 3);
    po_buffer_write_string(global.po_buffer, global.po_name);
    po_buffer_write_string(global.po_buffer, global.po_game_id);
    po_buffer_write_string(global.po_buffer, global.po_game_name);
    po_buffer_write_string(global.po_buffer, global.po_version);
    po_buffer_write_uint8(global.po_buffer, global.po_password != "");
    po_socket_write_message(global.po_socket, global.po_buffer);

    // Create udp socket
    global.po_udpsocket = po_udpsocket_create();
    po_udpsocket_start(global.po_udpsocket, false, 0);
    po_udpsocket_set_destination(global.po_udpsocket, global.po_server, global.po_udp_port);

    // Send initialize connection
    po_buffer_clear(global.po_buffer);
    po_buffer_write_uint8(global.po_buffer, 0);
    po_udpsocket_send(global.po_udpsocket, global.po_buffer);
}
var cur_player;

if (po_temp_someonesave_load() || global.po_someone_saved)
{
    if (room_exists(global.po_someone_room))
    {
        po_set_player_grav(global.po_someone_grav);
        cur_player = po_find_player();
        cur_player.x = global.po_someone_x;
        cur_player.y = global.po_someone_y;
        room_goto(global.po_someone_room);
    }
    global.po_someone_saved = false;
}

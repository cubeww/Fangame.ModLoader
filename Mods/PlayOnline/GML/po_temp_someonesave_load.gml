if (global.po_tempExe != -1)
{
    if (file_exists("tempOnline2"))
    {
        po_buffer_clear(global.po_buffer);
        po_buffer_read_from_file(global.po_buffer, "tempOnline2");
        global.po_someone_grav = po_buffer_read_uint8(global.po_buffer);
        global.po_someone_x = po_buffer_read_int32(global.po_buffer);
        global.po_someone_y = po_buffer_read_float64(global.po_buffer);
        global.po_someone_room = po_buffer_read_int16(global.po_buffer);
        file_delete("tempOnline2");
        return true;
    }
}

return false;
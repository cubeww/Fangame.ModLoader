if (global.po_tempExe != -1)
{
    po_buffer_clear(global.po_buffer);
    po_buffer_write_uint8(global.po_buffer, global.po_someone_grav);
    po_buffer_write_int32(global.po_buffer, global.po_someone_x);
    po_buffer_write_float64(global.po_buffer, global.po_someone_y);
    po_buffer_write_int16(global.po_buffer, global.po_someone_room);
    po_buffer_write_to_file(global.po_buffer, "tempOnline2");
}
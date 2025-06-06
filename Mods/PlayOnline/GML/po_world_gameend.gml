if (global.po_tempExe != -1)
{
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

    if (!file_exists("tempOnline"))
    {
        po_socket_destroy(global.po_socket);
        po_udpsocket_destroy(global.po_udpsocket);
    }
}

po_buffer_destroy(global.po_buffer);

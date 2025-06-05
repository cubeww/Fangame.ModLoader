/// ONLINE 
if (global.PO_TEMP_FILE)
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
}

po_buffer_destroy(__ONLINE_buffer);

if (global.PO_TEMP_FILE)
{
    if (!file_exists("tempOnline"))
    {
        po_socket_destroy(__ONLINE_socket);
        po_udpsocket_destroy(__ONLINE_udpsocket);
    }
}
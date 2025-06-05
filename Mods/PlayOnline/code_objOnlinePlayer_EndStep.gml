/// ONLINE
visible = __ONLINE_oRoom == room;
image_alpha = __ONLINE_alpha;
__ONLINE_p = global.PO_PLAYER;
if (global.PO_PLAYER2 != -1)
{
    if (!instance_exists(__ONLINE_p))
    {
        __ONLINE_p = global.PO_PLAYER2;
    }
}

if (instance_exists(__ONLINE_p))
{
    __ONLINE_dist = distance_to_object(__ONLINE_p);
    image_alpha = min(__ONLINE_alpha, __ONLINE_dist / 100);
}

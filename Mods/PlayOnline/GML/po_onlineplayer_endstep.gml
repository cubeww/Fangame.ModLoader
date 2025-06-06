visible = (cur_room == room);
image_alpha = alpha;
p = po_find_player();
if (p != noone)
{
    dist = distance_to_object(p);
    image_alpha = min(alpha, dist / 100);
}

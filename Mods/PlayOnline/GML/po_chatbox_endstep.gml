f = follower;
if (instance_exists(f))
{
    x = f.x;
    y = f.y;
}
else
{
    instance_destroy();
    exit;
}
if (fade)
{
    fade_alpha -= 0.02;
    if (fade_alpha <= 0)
    {
        instance_destroy();
        exit;
    }
}
alpha = 1;
if (follower != po_get_player())
{
    visible = follower.visible;
    p = po_get_player();
    if (instance_exists(p))
    {
        dist = distance_to_object(p);
        alpha = dist / 100;
    }
}
t -= 1;
if (t < 0)
{
    fade = true;
}

// Destroy all other chatboxes of the same player 
if (!has_destroyed)
{
    found = false;
    oChatbox = 0;
    for (i = 0; i < instance_number(chatbox) && !found; i += 1)
    {
        oChatbox = instance_find(chatbox, i);
        if (oChatbox.follower == follower && oChatbox.id != id)
        {
            found = true;
        }
    }
    if (found)
    {
        with (oChatbox)
        {
            instance_destroy();
        }
    }
    has_destroyed = true;
}

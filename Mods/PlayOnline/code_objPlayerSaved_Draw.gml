/// ONLINE 
__ONLINE_xx = 20;
__ONLINE_yy = 20;
if (view_enabled && view_visible[0])
{
    __ONLINE_xx += view_xview[0];
    __ONLINE_yy += view_yview[0];
}
__ONLINE_text = __ONLINE_name + " saved!";
__ONLINE__alpha = draw_get_alpha();
__ONLINE__color = draw_get_color();
draw_set_valign(fa_top);
draw_set_halign(fa_left);
draw_set_alpha(image_alpha);
draw_set_font(global.po_font_playername);
draw_set_color(c_black);
draw_text(__ONLINE_xx + 1, __ONLINE_yy, __ONLINE_text);
draw_text(__ONLINE_xx, __ONLINE_yy + 1, __ONLINE_text);
draw_text(__ONLINE_xx - 1, __ONLINE_yy, __ONLINE_text);
draw_text(__ONLINE_xx, __ONLINE_yy - 1, __ONLINE_text);
draw_set_color(c_white);
draw_text(__ONLINE_xx, __ONLINE_yy, __ONLINE_text);
draw_set_alpha(__ONLINE__alpha);
draw_set_color(__ONLINE__color);
if (font_exists(0))
{
    draw_set_font(0);
}
draw_set_valign(fa_top);
draw_set_halign(fa_left);

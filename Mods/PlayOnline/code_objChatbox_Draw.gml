/// ONLINE 
draw_set_font(global.po_font_playername);
__ONLINE_textHeight = string_height_ext(__ONLINE_message, __ONLINE_sep, __ONLINE_maxTextWidth);

__ONLINE_height = __ONLINE_textHeight + 2 * __ONLINE_paddingText;
__ONLINE_yOffset = -__ONLINE_height / 2 + 60;
__ONLINE_left = 0;
__ONLINE_right = room_width;
__ONLINE_top = 0;
__ONLINE_bottom = room_height;
if (view_enabled && view_visible[0])
{
    __ONLINE_left = view_xview[0];
    __ONLINE_right = __ONLINE_left + view_wview[0];
    __ONLINE_top = view_yview[0];
    __ONLINE_bottom = __ONLINE_top + view_hview[0];
}
__ONLINE_xx = min(max(x, __ONLINE_left + __ONLINE_width / 2 + __ONLINE_padding), __ONLINE_right - __ONLINE_width / 2 - __ONLINE_padding);
__ONLINE_yy = min(max(y - __ONLINE_yOffset, __ONLINE_top + __ONLINE_height / 2 + __ONLINE_padding), __ONLINE_bottom - __ONLINE_height / 2 - __ONLINE_padding);
__ONLINE__alpha = draw_get_alpha();
__ONLINE__color = draw_get_color();
draw_set_alpha(min(__ONLINE_alpha, __ONLINE_fadeAlpha));
draw_set_color(c_white);
draw_rectangle(__ONLINE_xx - __ONLINE_width / 2, __ONLINE_yy - __ONLINE_height / 2, __ONLINE_xx + __ONLINE_width / 2, __ONLINE_yy + __ONLINE_height / 2, false);
draw_set_color(c_black);
draw_rectangle(__ONLINE_xx - __ONLINE_width / 2, __ONLINE_yy - __ONLINE_height / 2, __ONLINE_xx + __ONLINE_width / 2, __ONLINE_yy + __ONLINE_height / 2, true);
draw_set_valign(fa_center);
draw_set_halign(fa_center);
draw_text_ext(__ONLINE_xx, __ONLINE_yy, __ONLINE_message, __ONLINE_sep, __ONLINE_maxTextWidth);
draw_set_alpha(__ONLINE__alpha);
draw_set_color(__ONLINE__color);
if (font_exists(0))
{
    draw_set_font(0);
}
draw_set_valign(fa_top);
draw_set_halign(fa_left);

draw_set_font(global.po_font_playername);
text_height = string_height_ext(message, sep, max_text_width);

height = text_height + 2 * padding_text;
y_offset = -height / 2 + 60;
drawleft = 0;
drawright = room_width;
top = 0;
bottom = room_height;
if (view_enabled && view_visible[0])
{
    drawleft = view_xview[0];
    drawright = drawleft + view_wview[0];
    top = view_yview[0];
    bottom = top + view_hview[0];
}
xx = min(max(x, drawleft + width / 2 + padding), drawright - width / 2 - padding);
yy = min(max(y - y_offset, top + height / 2 + padding), bottom - height / 2 - padding);
_alpha = draw_get_alpha();
_color = draw_get_color();
draw_set_alpha(min(alpha, fade_alpha));
draw_set_color(c_white);
draw_rectangle(xx - width / 2, yy - height / 2, xx + width / 2, yy + height / 2, false);
draw_set_color(c_black);
draw_rectangle(xx - width / 2, yy - height / 2, xx + width / 2, yy + height / 2, true);
draw_set_valign(fa_center);
draw_set_halign(fa_center);
draw_text_ext(xx, yy, message, sep, max_text_width);
draw_set_alpha(_alpha);
draw_set_color(_color);
if (font_exists(0))
{
    draw_set_font(0);
}
draw_set_valign(fa_top);
draw_set_halign(fa_left);

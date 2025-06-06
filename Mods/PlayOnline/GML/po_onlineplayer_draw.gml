if (sprite_exists(sprite_index))
{
    draw_sprite_ext(sprite_index, image_index, x, y, image_xscale, image_yscale, image_angle, c_white, image_alpha);
    _alpha = draw_get_alpha();
    _color = draw_get_color();
    draw_set_alpha(image_alpha);
    draw_set_font(global.po_font_playername);
    draw_set_valign(fa_center);
    draw_set_halign(fa_center);
    draw_set_color(c_black);
    border = 2;
    padding = 30;
    xx = x;
    yy = y - padding;
    draw_set_alpha(1);
    draw_text(xx + border, yy, name);
    draw_text(xx, yy + border, name);
    draw_text(xx - border, yy, name);
    draw_text(xx, yy - border, name);
    draw_set_color(c_white);
    draw_text(xx, yy, name);
    draw_set_alpha(_alpha);
    draw_set_color(_color);
    if (font_exists(0))
    {
        draw_set_font(0);
    }
    draw_set_valign(fa_top);
    draw_set_halign(fa_left);
}

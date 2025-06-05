using System.Security.Cryptography;
using System.Text;
using Fangame.ModLoader;
using Fangame.ModLoader.Common;
using UndertaleModLib.Models;

namespace PlayOnline;

public class PlayOnlineMod : Mod
{
    PlayOnlineConfig Config = null!;

    public override void Load()
    {
        Config = GetConfig<PlayOnlineConfig>();

        if (CommonData != null)
        {
            // Find important assets
            CommonObject? player = null;
            CommonObject? player2 = null;
            CommonObject? world = null;
            foreach (var obj in CommonData.Objects)
            {
                switch (obj.Name)
                {
                    case "world" or "World" or "objWorld" or "oWorld":
                        world = obj;
                        break;
                    case "player" or "Player" or "objPlayer" or "oPlayer":
                        player = obj;
                        break;
                    case "player2" or "Player2" or "objPlayer2" or "oPlayer2":
                        player2 = obj;
                        break;
                    case "__ONLINE_onlinePlayer" or "__ONLINE_chatbox" or "__ONLINE_playerSaved":
                        Console.WriteLine("error: game is already online version");
                        return;
                }
            }

            if (world == null)
            {
                Console.WriteLine("error: object \"world\" not exists");
                return;

            }

            if (player == null)
            {
                Console.WriteLine("error: object \"player\" not exists");
                return;
            }

            CommonScript? loadGame = null;
            CommonScript? saveGame = null;
            CommonScript? saveExe = null;
            CommonScript? tempExe = null;
            foreach (var script in CommonData.Scripts)
            {
                switch (script.Name)
                {
                    case "loadGame" or "scrLoadGame" or "savedata_save":
                        loadGame = script;
                        break;
                    case "saveGame" or "scrSaveGame" or "savedata_load":
                        saveGame = script;
                        break;
                    case "saveExe":
                        saveExe = script;
                        break;
                    case "tempExe":
                        tempExe = script;
                        break;
                    case "po_http_init":
                        Console.WriteLine("error: game is already online version");
                        return;
                }
            }

            if (loadGame == null)
            {
                Console.WriteLine("error: script \"loadGame\" not exists");
                return;

            }

            if (saveGame == null)
            {
                Console.WriteLine("error: script \"saveGame\" not exists");
                return;
            }

            // HTTP
            string scriptName = "";
            StringBuilder sb = new StringBuilder();
            CommonScript newScript;
            foreach (var line in File.ReadAllLines(Path.Combine(ModDirectory, "po_http_dll_2_3.gml")))
            {
                if (line.StartsWith("#define"))
                {
                    if (scriptName != "")
                    {
                        newScript = CommonData.Scripts.CreateNew();
                        newScript.Name = scriptName;
                        newScript.Source = sb.ToString();
                        sb.Clear();
                    }
                    scriptName = line[8..];
                }
                else
                {
                    sb.AppendLine(line);
                }
            }
            newScript = CommonData.Scripts.CreateNew();
            newScript.Name = scriptName;
            newScript.Source = sb.ToString();
            sb.Clear();
            CopyFileToRunningDirectory("po_http_dll_2_3.dll");

            // Replace macros
            var worldCreate = new StringBuilder(GetCode("code_objWorld_Create.gml"));
            worldCreate.Replace("{GAME_ID}", GetGameDataHash());
            worldCreate.Replace("{GAME_NAME}", Path.GetFileNameWithoutExtension(ExecutablePath));
            worldCreate.Replace("{SERVER}", Config.ServerIp);
            worldCreate.Replace("{VERSION}", "1.1.9");
            worldCreate.Replace("{NAME}", Config.PlayerName);
            worldCreate.Replace("{PASSWORD}", Config.Password);
            worldCreate.Replace("{RACE}", Config.RaceMode ? "true" : "false");
            worldCreate.Replace("{TCP_PORT}", Config.TcpPort.ToString());
            worldCreate.Replace("{UDP_PORT}", Config.UdpPort.ToString());
            worldCreate.Replace("{TEMP_FILE}", tempExe != null ? "true" : "false");
            worldCreate.Replace("{WORLD}", world.Name);
            worldCreate.Replace("{PLAYER}", player.Name);
            worldCreate.Replace("{PLAYER2}", player2?.Name ?? "-1");
            worldCreate.Replace("{GLOBAL_PLAYER_XSCALE}", "false"); // Why?
            worldCreate.Replace("{YOYOYO}", (world.Name == "objWorld" && saveGame.Name == "scrSaveGame") ? "true" : "false");
            worldCreate.Replace("{RENEX}", (world.Name == "World") ? "true" : "false");

            // World
            world.EventAddCode(EventType.Create, 0, worldCreate.ToString());
            world.EventAddCode(EventType.Step, (int)EventSubtypeStep.EndStep, GetCode("code_objWorld_EndStep.gml"));
            world.EventAddCode(EventType.Other, (int)EventSubtypeOther.GameEnd, GetCode("code_objWorld_GameEnd.gml"));

            // Save Game
            saveGame.Source += '\n' + GetCode("code_scrSaveGame.gml");

            // Load Game
            if (tempExe != null)
            {
                // Yuuutu, Lemon
                // loadGame() => write('temp'), game_restart()
                // GameStart() => tempExe()
                // tempExe() => read('temp'), saveExe()
                // saveExe() => load room, player...
                loadGame.Source += '\n' + GetCode("code_scrSaveTemp.gml");
                tempExe.Source += '\n' + GetCode("code_scrLoadGame.gml");
            }
            else if (saveExe != null)
            {
                // Nikaple, Kamilia 3
                saveExe.Source += '\n' + GetCode("code_scrLoadGame.gml");
            }
            else
            {
                // YoYoYo, Renex
                loadGame.Source += '\n' + GetCode("code_scrLoadGame.gml");
            }

            // Online Player
            CommonObject onlinePlayer = CommonData.Objects.CreateNew();
            onlinePlayer.Name = "__ONLINE_onlinePlayer";
            onlinePlayer.Persistent = true;
            onlinePlayer.Depth = -10;
            onlinePlayer.EventAddCode(EventType.Create, 0, GetCode("code_objOnlinePlayer_Create.gml"));
            onlinePlayer.EventAddCode(EventType.Step, (int)EventSubtypeStep.EndStep, GetCode("code_objOnlinePlayer_EndStep.gml"));
            onlinePlayer.EventAddCode(EventType.Draw, (int)EventSubtypeDraw.Draw, GetCode("code_objOnlinePlayer_Draw.gml"));

            // Chatbox
            CommonObject chatbox = CommonData.Objects.CreateNew();
            chatbox.Name = "__ONLINE_chatbox";
            chatbox.Visible = true;
            chatbox.Depth = -11;
            chatbox.Persistent = true;
            chatbox.EventAddCode(EventType.Create, 0, GetCode("code_objChatbox_Create.gml"));
            chatbox.EventAddCode(EventType.Step, (int)EventSubtypeStep.EndStep, GetCode("code_objChatbox_EndStep.gml"));
            chatbox.EventAddCode(EventType.Draw, (int)EventSubtypeDraw.Draw, GetCode("code_objChatbox_Draw.gml"));

            // Player Saved
            CommonObject playerSaved = CommonData.Objects.CreateNew();
            playerSaved.Name = "__ONLINE_playerSaved";
            playerSaved.Visible = true;
            playerSaved.Depth = -10;
            playerSaved.EventAddCode(EventType.Step, (int)EventSubtypeStep.EndStep, GetCode("code_objPlayerSaved_EndStep.gml"));
            playerSaved.EventAddCode(EventType.Draw, (int)EventSubtypeDraw.Draw, GetCode("code_objPlayerSaved_Draw.gml"));

            // GM8 specific
            if (GM8Data != null)
            {
                CommonScript soundAdd = CommonData.Scripts.CreateNew();
                soundAdd.Name = "po_sound_add";
                soundAdd.Source = GetCode("po_sound_add_GM8.gml");

                CommonScript soundPlay = CommonData.Scripts.CreateNew();
                soundPlay.Name = "po_sound_play";
                soundPlay.Source = GetCode("po_sound_play_GM8.gml");

                CommonScript fontAdd = CommonData.Scripts.CreateNew();
                fontAdd.Name = "po_font_add";
                fontAdd.Source = GetCode("po_font_add_GM8.gml");

                CopyFileToRunningDirectory("po_snd_chatbox.wav");
                CopyFileToRunningDirectory("po_snd_saved.wav");
            }

            // GMS specific
            if (UndertaleData != null)
            {
                CommonScript soundAdd = CommonData.Scripts.CreateNew();
                soundAdd.Name = "po_sound_add";
                soundAdd.Source = GetCode("po_sound_add_GMS.gml");

                CommonScript soundPlay = CommonData.Scripts.CreateNew();
                soundPlay.Name = "po_sound_play";
                soundPlay.Source = GetCode("po_sound_play_GMS.gml");

                CommonScript fontAdd = CommonData.Scripts.CreateNew();
                fontAdd.Name = "po_font_add";
                fontAdd.Source = GetCode("po_font_add_GMS.gml");

                CopyFileToRunningDirectory("po_snd_chatbox.ogg");
                CopyFileToRunningDirectory("po_snd_saved.ogg");
                CopyFileToRunningDirectory("arial.ttf");
            }
        }
    }

    private string GetCode(string fileName)
    {
        return File.ReadAllText(Path.Combine(ModDirectory, fileName));
    }

    private string GetGameDataHash()
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(GameDataPath);
        var hash = md5.ComputeHash(stream);
        return Convert.ToHexStringLower(hash);
    }
}

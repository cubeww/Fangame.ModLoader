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
            // Find world
            CommonObject? world = null;
            foreach (var obj in CommonData.Objects)
            {
                switch (obj.Name)
                {
                    case "world" or "World" or "objWorld":
                        world = obj;
                        break;
                    case "__ONLINE_onlinePlayer" or "po_onlineplayer":
                        Console.WriteLine("error: game is already online version");
                        return;
                }
            }

            if (world == null)
            {
                Console.WriteLine("error: object \"world\" not exists");
                return;
            }

            // Find important scripts
            CommonScript? loadGame = null;
            CommonScript? saveGame = null;
            CommonScript? saveExe = null;
            CommonScript? tempExe = null;
            CommonScript? scrLoadGame = null;
            CommonScript? scrSaveGame = null;
            CommonScript? savedata_save = null;
            CommonScript? savedata_load = null;
            foreach (var script in CommonData.Scripts)
            {
                switch (script.Name)
                {
                    case "loadGame": loadGame = script; break;
                    case "saveGame": saveGame = script; break;
                    case "saveExe": saveExe = script; break;
                    case "tempExe": tempExe = script; break;
                    case "scrLoadGame": scrLoadGame = script; break;
                    case "scrSaveGame": scrSaveGame = script; break;
                    case "savedata_save": savedata_save = script; break;
                    case "savedata_load": savedata_load = script; break;
                    case "po_http_init": Console.WriteLine("error: game is already online version"); return;
                }
            }

            // HTTP
            ImportScriptList("po_http_dll_2_3.gml");
            CopyFileToRunningDirectory("po_http_dll_2_3.dll");

            // Config
            var config = new StringBuilder(File.ReadAllText(Path.Combine(ModDirectory, "GML/Config/po_config.gml")));
            config.Replace("{SERVER}", Config.ServerIp);
            config.Replace("{TCP_PORT}", Config.TcpPort.ToString());
            config.Replace("{UDP_PORT}", Config.UdpPort.ToString());
            config.Replace("{RACE}", Config.RaceMode ? "true" : "false");
            config.Replace("{PASSWORD}", Config.Password);
            config.Replace("{GAME_ID}", GetGameDataHash());
            config.Replace("{GAME_NAME}", Path.GetFileNameWithoutExtension(ExecutablePath));
            config.Replace("{NAME}", Config.PlayerName);
            config.Replace("{VERSION}", "1.1.9");
            var configScript = CommonData.Scripts.CreateNew();
            configScript.Name = "po_config";
            configScript.Source = config.ToString();

            // World
            world.EventAddCode(EventType.Create, 0, '\n' + "po_world_create();");
            world.EventAddCode(EventType.Step, (int)EventSubtypeStep.EndStep, '\n' + "po_world_endstep();");
            world.EventAddCode(EventType.Other, (int)EventSubtypeOther.GameEnd, '\n' + "po_world_gameend();");

            // Save Game
            if (saveGame != null)
            {
                saveGame.Source += '\n' + "po_someonesave_send();";
            }
            else if (scrSaveGame != null)
            {
                scrSaveGame.Source += '\n' + "if (argument0) { po_someonesave_send(); }";
            }
            else if (savedata_save != null)
            {
                savedata_save.Source += '\n' + "po_someonesave_send();";
            }

            // Load Game
            if (tempExe != null && loadGame != null)
            {
                // Yuuutu
                // loadGame() => write('temp'), game_restart()
                // GameStart() => tempExe()
                // tempExe() => read('temp'), saveExe()
                // saveExe() => load room, player...
                loadGame.Source += '\n' + "po_temp_socketstate_save();";
                tempExe.Source += '\n' + "po_someonesave_load();";
            }
            else if (saveExe != null)
            {
                // Nikaple
                saveExe.Source += '\n' + "po_someonesave_load();";
            }
            else if (scrLoadGame != null)
            {
                // YoYoYo
                scrLoadGame.Source += '\n' + "po_someonesave_load();";
            }
            else if (savedata_load != null)
            {
                // Renex
                savedata_load.Source += '\n' + "po_someonesave_load();";
            }

            // Online Player
            CommonObject onlinePlayer = CommonData.Objects.CreateNew();
            onlinePlayer.Name = "po_onlineplayer";
            onlinePlayer.Persistent = true;
            onlinePlayer.Depth = -10;
            onlinePlayer.EventAddCode(EventType.Create, 0, "po_onlineplayer_create();");
            onlinePlayer.EventAddCode(EventType.Step, (int)EventSubtypeStep.EndStep, "po_onlineplayer_endstep();");
            onlinePlayer.EventAddCode(EventType.Draw, (int)EventSubtypeDraw.Draw, "po_onlineplayer_draw();");

            // Chatbox
            CommonObject chatbox = CommonData.Objects.CreateNew();
            chatbox.Name = "po_chatbox";
            chatbox.Visible = true;
            chatbox.Depth = -11;
            chatbox.Persistent = true;
            chatbox.EventAddCode(EventType.Create, 0, "po_chatbox_create();");
            chatbox.EventAddCode(EventType.Step, (int)EventSubtypeStep.EndStep, "po_chatbox_endstep();");
            chatbox.EventAddCode(EventType.Draw, (int)EventSubtypeDraw.Draw, "po_chatbox_draw();");

            // Player Saved
            CommonObject playerSaved = CommonData.Objects.CreateNew();
            playerSaved.Name = "po_playersaved";
            playerSaved.Visible = true;
            playerSaved.Depth = -10;
            playerSaved.EventAddCode(EventType.Step, (int)EventSubtypeStep.EndStep, "po_playersaved_endstep();");
            playerSaved.EventAddCode(EventType.Draw, (int)EventSubtypeDraw.Draw, "po_playersaved_draw();");

            // Common scripts
            ImportScripts("GML");

            // GM8 specific
            if (GM8Data != null)
            {
                ImportScripts("GML/GM8");
                CopyFileToRunningDirectory("po_snd_chatbox.wav");
                CopyFileToRunningDirectory("po_snd_saved.wav");
            }

            // GMS specific
            if (UndertaleData != null)
            {
                ImportScripts("GML/GMS");
                CopyFileToRunningDirectory("po_snd_chatbox.ogg");
                CopyFileToRunningDirectory("po_snd_saved.ogg");
                CopyFileToRunningDirectory("arial.ttf");
            }
        }
    }

    private void ImportScriptList(string fileName)
    {
        if (CommonData != null)
        {
            string scriptName = "";
            StringBuilder sb = new StringBuilder();
            CommonScript newScript;
            foreach (var line in File.ReadAllLines(Path.Combine(ModDirectory, fileName)))
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
        }
    }

    private void ImportScripts(string directory)
    {
        if (CommonData != null)
        {
            foreach (var filePath in Directory.EnumerateFiles(Path.Combine(ModDirectory, directory), "*.gml"))
            {
                CommonScript script = CommonData.Scripts.CreateNew();
                script.Name = Path.GetFileNameWithoutExtension(filePath);
                script.Source = File.ReadAllText(filePath);
            }
        }
    }

    private string GetGameDataHash()
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(GameDataPath);
        var hash = md5.ComputeHash(stream);
        return Convert.ToHexStringLower(hash);
    }
}

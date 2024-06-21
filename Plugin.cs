using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;
using System.Text.Json;

namespace InvSee
{
    [ApiVersion(2, 1)]
    public class InvSee : TerrariaPlugin
    {

        public override string Author => "Onusai";
        public override string Description => "View other players inventory";
        public override string Name => "InvSee";
        public override Version Version => new Version(1, 0, 0, 0);

        public class ConfigData
        {
            public bool CommandEnabled { get; set; } = true;
            public bool OnlyShowIfOnTeam { get; set; } = true;
        }

        ConfigData configData;

        public InvSee(Main game) : base(game) { }

        public override void Initialize()
        {
            configData = PluginConfig.Load("InvSee");
            ServerApi.Hooks.GameInitialize.Register(this, OnGameLoad);
        }

        void OnGameLoad(EventArgs e)
        {
            RegisterCommand("invsee", "", SeeInv, "/invsee <player>");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnGameLoad);
            }
            base.Dispose(disposing);
        }

        void RegisterCommand(string name, string perm, CommandDelegate handler, string helptext)
        {
            TShockAPI.Commands.ChatCommands.Add(new Command(perm, handler, name)
            { HelpText = helptext });
        }

        void SeeInv(CommandArgs args)
        {
            if (!configData.CommandEnabled || args.Parameters.Count == 0) return;

            var cmdPlayer = args.Player;

            TSPlayer[] players;
            if (args.Parameters[0] == "all")
            {
                players = TShock.Players;
            }
            else
            {
                players = TSPlayer.FindByNameOrID(args.Parameters[0]).ToArray();
            }

            if (players.Length == 0) return;

            

            foreach (TSPlayer player in players)
            {
                if (player == null) continue;

                if (!cmdPlayer.Group.HasPermission("tshock.admin") && configData.OnlyShowIfOnTeam && cmdPlayer.Team != player.Team)
                {
                    if (args.Parameters[0] != "all")
                        cmdPlayer.SendErrorMessage("Unable to use invsee on this player because you are on different teams");
                    continue;
                }

                var msg = "";

                cmdPlayer.SendInfoMessage(player.Name + "'s Inventory -----------");

                int i = 0;
                foreach (Item item in player.TPlayer.inventory)
                {
                    if (item.type == 0) continue;

                    msg += ChatItem(item.type, item.stack) + " ";

                    i++;
                    if (i == 10)
                    {
                        cmdPlayer.SendInfoMessage(msg);
                        msg = "";
                        i = 0;
                    }
                }
                if (msg != "") cmdPlayer.SendInfoMessage(msg);

                msg = "";
                i = 0;
                foreach (Item item in player.TPlayer.armor)
                {
                    if (item.type != 0)
                        msg += ChatItem(item.type, item.stack) + " ";

                    i++;
                    if (i == 9)
                    {
                        cmdPlayer.SendInfoMessage("Equiped: " + msg);
                        msg = "";
                    }
                }
                cmdPlayer.SendInfoMessage("Vanity:" + msg);


                msg = "";
                foreach (Item item in player.TPlayer.miscEquips)
                {
                    if (item.type == 0) continue;
                    msg += ChatItem(item.type, item.stack) + " ";
                }
                cmdPlayer.SendInfoMessage("Misc: " + msg);
            }
        }

        String ChatItem(int id, int stack)
        {
            return "[i/s" + stack.ToString() + ":" + id.ToString() + "]";
        }

        public static class PluginConfig
        {
            public static string filePath;
            public static ConfigData Load(string Name)
            {
                filePath = String.Format("{0}/{1}.json", TShock.SavePath, Name);

                if (!File.Exists(filePath))
                {
                    var data = new ConfigData();
                    Save(data);
                    return data;
                }

                var jsonString = File.ReadAllText(filePath);
                var myObject = JsonSerializer.Deserialize<ConfigData>(jsonString);

                return myObject;
            }

            public static void Save(ConfigData myObject)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(myObject, options);

                File.WriteAllText(filePath, jsonString);
            }
        }

    }
}
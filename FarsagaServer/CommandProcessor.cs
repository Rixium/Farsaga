using Farsaga.Config;
using Farsaga.GameClasses.PlayerClasses;
using Farsaga.Network.Packets;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarsagaServer {

    class CommandProcessor {

        public CommandProcessor() {

        }

        public static bool ProcessCommand(string command, int playerrole, Player p, Form1 form, NetServer server) {
            switch(playerrole) {
                case ServerRoles.ADMIN:
                    if(command == "!restart") {
                        form.Restart();
                        return true;
                    } else if(command.Contains("!mod"))
                    {
                        command = command.Remove(0, 5);
                        DatabaseHandler.PromoteUser(command, ServerRoles.MODERATOR);
                        Console.WriteLine(command);
                        foreach (Player player in form.GetPlayers())
                        {
                            if (player.getName() == command)
                            {
                                player.setRole(ServerRoles.MODERATOR);
                                return true;
                            }
                        }
                    } else if(command.Contains("!demote"))
                    {
                        command = command.Remove(0, 8);
                        Console.WriteLine(command);
                        DatabaseHandler.DemoteUser(command, ServerRoles.PLAYER);
                        foreach(Player player in form.GetPlayers())
                        {
                            if(player.getName() == command)
                            {
                                player.setRole(ServerRoles.PLAYER);
                                return true;
                            }
                        }
                    }
                    break;
                case ServerRoles.MODERATOR:
                    break;
                default:
                    break;
            }
            return false;
        }
    }

}

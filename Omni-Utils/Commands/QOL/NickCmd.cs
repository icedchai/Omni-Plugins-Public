using CommandSystem;
using Exiled.API.Features;
using System;

namespace Omni_Utils.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    //NickCmd.cs by icedchqi
    //Documented November 16th 2024
    public class NickCmd : ICommand
    {
        public string Command { get; } = "nickname";

        public string[] Aliases { get; } = new[] { "nick", "name", "rename" };

        public string Description { get; } = "Set your nickname";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            //This is a client-side command, created as an improvement upon a previous nickname command for players.
            //The issues with the previous one became apparent when you had more than 24 characters in your nickname,
            //or if you had more than three words in the argument.
            Player player = Player.Get(sender);
            if (arguments.Count <= 0)
            {
                response = "USAGE: nickname (NICK)";
                return false;
            }
            if (player == null)
            {
                response = "You must exist to run this command!";
                return false;
            }
            //The for loop is a bit fucked, but I don't care. This loop will cycle through every word in the command, rather than
            //only the first three.
            string name = arguments.At(0);
            for (int i = 1; i < arguments.Count; i++)
            {
                name += $" {arguments.At(i)}";
            }
            //This function checks for slurs in a nickname. Won't catch everyone, but it's better to be able to filter through hard
            //slurs automatically than to have an admin see it and have to deal with it.
            /*if (OmniUtilsAPI.CheckSlurs(name, out string offender))
            {
                player.Ban(999999999, $"Inappropriate usage of naming system - Offensive Item : {offender}. Handler: Automatic");
                Log.Info($"Banned player {player.Nickname} ({player.UserId}) for inappropriate name: {name}.");
            }*/

            //Player.CustomName is the nickname, not Player.Nickname, which is their username.
            player.CustomName = name;
            Log.Info($"{player.Nickname} ({player.UserId}) set nickname to {name}");
            response = $"Set your nickname";
            return true;
        }
    }

}

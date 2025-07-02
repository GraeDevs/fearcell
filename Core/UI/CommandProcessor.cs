using fearcell.Content.Items.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace fearcell.Core.UI
{
    public class CommandProcessor
    {
        private Dictionary<string, ICommand> commands;

        /*
         * Implement saving of this variable
         */
        public bool hasAdminAccess;

        public CommandProcessor()
        {
            hasAdminAccess = false;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
            {
                { "help", new HelpCommand(this) },
                { "clear", new ClearCommand() },
                { "time", new TimeCommand() },
                { "player", new PlayerCommand() },
                { "world", new WorldCommand() },
                { "echo", new EchoCommand() },
                { "version", new VersionCommand() },
                { "adminaccess", new AdminAccessCommand(this) },

                { "spawn", new SpawnItemCommand(this) }
            };
        }

        public List<string> ProcessCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string> { "Invalid command. Type 'help' for available commands." };

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string commandName = parts[0].ToLower();
            string[] args = parts.Skip(1).ToArray();

            if (commands.ContainsKey(commandName))
            {
                if (IsAdminCommand(commandName) && !hasAdminAccess)
                {
                    return new List<string>
                    {
                        $"'{commandName}' requires admin access.",
                        "Use 'adminAccess' command to unlock admin features."
                    };
                }

                return commands[commandName].Execute(args);
            }
            else
            {
                return new List<string>
                {
                    $"'{commandName}' is not recognized as a valid command.",
                    "Type 'help' for available commands."
                };
            }
        }

        private bool IsAdminCommand(string commandName)
        {
            string[] adminCommands = {"spawn" };
            return adminCommands.Contains(commandName.ToLower());
        }


        public void AddCommand(string name, ICommand command)
        {
            commands[name.ToLower()] = command;
        }

        public List<string> GetCommandNames()
        {
            return commands.Keys.ToList();
        }
    }

    // Command interface
    public interface ICommand
    {
        List<string> Execute(string[] args);
        string GetDescription();
        string GetUsage();
    }

    // Help command
    public class HelpCommand : ICommand
    {
        private CommandProcessor processor;

        public HelpCommand(CommandProcessor commandProcessor)
        {
            processor = commandProcessor;
        }

        public List<string> Execute(string[] args)
        {
            var result = new List<string>
            {
                "Available Commands:",
                "==================",
                "",
                "help        - Show this help message",
                "clear       - Clear the terminal screen",
                "time        - Display current game time",
                "player      - Show player information",
                "world       - Show world information",
                "echo <text> - Echo back the provided text",
                "version     - Show terminal version",
                "adminAccess - Unlock administrative commands (requires admin key)",
                ""
            };

            if (processor.hasAdminAccess)
            {
                result.Add(" :");
                result.Add("Admin Commands:");
                result.Add("==============");
                result.Add("");
                result.Add("spawn <itemID> [amount] - Spawn items");
                result.Add("");
            }

            result.Add("Usage: <command> [arguments]");
            return result;
        }

        public string GetDescription() => "Show available commands";
        public string GetUsage() => "help";
    }

    // Clear command
    public class ClearCommand : ICommand
    {
        public List<string> Execute(string[] args)
        {
            return new List<string> { "CLEAR_SCREEN" };
        }

        public string GetDescription() => "Clear the terminal screen";
        public string GetUsage() => "clear";
    }

    // Time command
    public class TimeCommand : ICommand
    {
        public List<string> Execute(string[] args)
        {
            var result = new List<string>();

            if (Main.dayTime)
            {
                TimeSpan time = TimeSpan.FromSeconds(Main.time / 60);
                result.Add($"Current Time: {7 + time.Hours:D2}:{time.Minutes:D2} AM (Day)");
            }
            else
            {
                TimeSpan time = TimeSpan.FromSeconds(Main.time / 60);
                result.Add($"Current Time: {7 + time.Hours:D2}:{time.Minutes:D2} PM (Night)");
            }

            result.Add($"Day: {Main.dayTime}");
            result.Add($"Raw Time: {Main.time:F0}");

            return result;
        }

        public string GetDescription() => "Display current game time";
        public string GetUsage() => "time";
    }

    // Player command
    public class PlayerCommand : ICommand
    {
        public List<string> Execute(string[] args)
        {
            Player player = Main.LocalPlayer;
            var result = new List<string>
            {
                "Player Information:",
                "==================",
                $"Name: {player.name}",
                $"Health: {player.statLife}/{player.statLifeMax}",
                $"Mana: {player.statMana}/{player.statManaMax}",
                $"Position: X={player.position.X:F0}, Y={player.position.Y:F0}",
                $"Difficulty: {(player.difficulty == 0 ? "Classic" : player.difficulty == 1 ? "Expert" : player.difficulty == 2 ? "Master" : "Journey")}"
            };
            return result;
        }

        public string GetDescription() => "Show player information";
        public string GetUsage() => "player";
    }

    // World command
    public class WorldCommand : ICommand
    {
        public List<string> Execute(string[] args)
        {
            var result = new List<string>
            {
                "World Information:",
                "==================",
                $"World Name: {Main.worldName}",
                $"World Size: {Main.maxTilesX} x {Main.maxTilesY}",
                $"Spawn Point: X={Main.spawnTileX}, Y={Main.spawnTileY}",
                $"Surface Level: {Main.worldSurface}",
                $"Rock Layer: {Main.rockLayer}",
                $"Hell Layer: {Main.maxTilesY - 200}",
                $"Hardmode: {Main.hardMode}",
                $"Expert Mode: {Main.expertMode}",
                $"Master Mode: {Main.masterMode}"
            };
            return result;
        }

        public string GetDescription() => "Show world information";
        public string GetUsage() => "world";
    }

    // Echo command
    public class EchoCommand : ICommand
    {
        public List<string> Execute(string[] args)
        {
            return new List<string> { string.Join(" ", args) };
        }

        public string GetDescription() => "Echo back the provided text";
        public string GetUsage() => "echo";
    }

    // Version command
    public class VersionCommand : ICommand
    {
        public List<string> Execute(string[] args)
        {
            return new List<string>
            {
                "Delta Corporation [TM]",
                "Version 1.0",
                "",
                ""
            };
        }

        public string GetDescription() => "Show terminal version";
        public string GetUsage() => "version";
    }

    public class AdminAccessCommand : ICommand
    {
        private CommandProcessor processor;

        public AdminAccessCommand(CommandProcessor commandProcessor)
        {
            processor = commandProcessor;
        }

        public List<string> Execute(string[] args)
        {
            if (processor.hasAdminAccess)
            {
                return new List<string> { "Admin access already granted." };
            }

            Player player = Main.LocalPlayer;

            int itemSlot = -1;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item != null && item.type == ModContent.ItemType<BlackItem>() && !item.IsAir)
                {
                    itemSlot = i;
                    break;
                }
            }

            if (itemSlot == -1)
            {
                return new List<string>
                {
                    "Access denied: Admin key not found in inventory.",
                    "Required item: Admin Access Key"
                };
            }

            string itemName = player.inventory[itemSlot].Name;
            player.inventory[itemSlot].TurnToAir();

            processor.hasAdminAccess = true;

            return new List<string>
            {
                "Admin access granted!",
                "",
                "New commands unlocked:",
                "- spawn <itemID> [amount]",
                "",
                "Type 'help' to see all available commands."
            };
        }

        public string GetDescription() => "Unlock admin commands";
        public string GetUsage() => "adminAccess";
    }

    public class SpawnItemCommand : ICommand
    {
        private CommandProcessor processor;

        public SpawnItemCommand(CommandProcessor commandProcessor)
        {
            processor = commandProcessor;
        }

        public List<string> Execute(string[] args)
        {
            if (args.Length == 0)
            {
                return new List<string> { "Usage: spawn <itemID> [amount]" };
            }

            if (!int.TryParse(args[0], out int itemID))
            {
                return new List<string> { "Invalid item ID." };
            }

            int amount = args.Length > 1 && int.TryParse(args[1], out int a) ? a : 1;

            Player player = Main.LocalPlayer;
            int itemIndex = Item.NewItem(player.GetSource_GiftOrReward(), player.getRect(), itemID, amount);

            if (Main.item[itemIndex] != null)
            {
                string itemName = Main.item[itemIndex].Name;
                return new List<string> { $"Spawned {amount}x {itemName}" };
            }

            return new List<string> { "Failed to spawn item." };
        }

        public string GetDescription() => "Spawn items";
        public string GetUsage() => "spawn <itemID> [amount]";
    }

}
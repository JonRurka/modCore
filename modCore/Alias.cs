using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace modCore
{
    public static class Alias
    {
        private const string FILE_NAME = "aliases";
        private const string EXTENSION = ".txt";

        private static Dictionary<string, string> alias_command = new Dictionary<string,string>();
        private static string PATH = string.Empty;

        public static Dictionary<string, string> Aliases
        {
            get { return new Dictionary<string,string>(alias_command); }
        }

        public static void Load()
        {
            PATH = ModCore.ModFolder + "\\config\\";
            string file = PATH + FILE_NAME + EXTENSION;
            if (!File.Exists(file))
            {
                Directory.CreateDirectory(PATH);
                File.Create(file).Close();
                return;
            }
            alias_command = new Dictionary<string, string>();
            StreamReader reader = new StreamReader(file);
            string fullText = reader.ReadToEnd();
            if (!fullText.Equals(string.Empty))
            {
                string[] aliases = fullText.Split(';');
                foreach (string alias in aliases)
                {
                    if (!alias.Equals(string.Empty))
                    {
                        string[] parts = alias.Split(',');
                        if (parts.Length == 2)
                            alias_command.Add(parts[0], parts[1]);
                    }
                }
            }
        }

        public static void Save()
        {
            PATH = Environment.CurrentDirectory + "\\StarForge_Data\\config\\";
            string file = PATH + FILE_NAME + EXTENSION;

            if (!Directory.Exists(PATH))
            {
                Directory.CreateDirectory(PATH);
            }

            StreamWriter writer = new StreamWriter(file, false);
            foreach (string alias in alias_command.Keys)
            {
                writer.WriteLine(alias + "," + alias_command[alias] + ";");
            }
            writer.Close();
            
        }

        public static string GetCommand(string alias)
        {
            if (alias_command.ContainsKey(alias))
            {
                return alias_command[alias];
            }
            return string.Empty;
        }

        public static void AddAlias(string alias, string command)
        {
            if (!alias_command.ContainsKey(alias) && !alias_command.ContainsValue(command))
            {
                alias_command.Add(alias, command);
            }
            else if (alias_command.ContainsKey(alias))
            {
                throw new Exception("Alias already exists.");
            }
            else if (alias_command.ContainsValue(command))
            {
                throw new Exception("There is already an alias for this command.");
            }
        }

        public static void RemoveAlias(string alias)
        {
            if (alias_command.ContainsKey(alias))
            {
                alias_command.Remove(alias);
            }
        }

        public static string[] GetAliases()
        {
            List<string> aliases = new List<string>();
            foreach (string alias in alias_command.Keys)
            {
                aliases.Add(alias);
            }
            return aliases.ToArray();
        }
    }
}

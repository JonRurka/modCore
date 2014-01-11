using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CodeHatch.AI;
using CodeHatch.Common;
using UnityEngine;

namespace modCore
{
    public class ModCore
    {
        #region variables
        public Monitor monitorComp;
        public Console console;

        ModApi modApi;
        string path = string.Empty;
        ICollection<IPlugin> plugins;
        Dictionary<string, IPlugin> name_plugin;
        Dictionary<string, CommandDescription[]> plugin_Commands;
        Dictionary<string, CommandDescription> allCommands;
        
        #endregion

        #region properties
        /// <summary>
        /// Returns a list of all plugins.
        /// </summary>
        public ICollection<IPlugin> Plugins
        {
            get { return plugins; }
        }

        /// <summary>
        /// Returns the current version of modCore.
        /// </summary>
        public string Version
        {
            get { return "ModCore version 1.2-beta"; }
        }

        /// <summary>
        /// Returns a reference to the mod API.
        /// </summary>
        public ModApi API
        {
            get { return modApi; }
        }

        [Obsolete("Please use ModApi.ConsoleOpen instead.", false)]
        public bool ConsoleOpen
        {
            get 
            {
                return modApi.ConsoleOpen;
            }
        }
        #endregion


        #region constructors
        public ModCore()
        {
            Console.modCore = this;
            ObjImporter.modCore = this;
            GameObject monitor = new GameObject("monitor");
            monitorComp = monitor.AddComponent<Monitor>();
            monitorComp.modCore = this;
            modApi = new ModApi(this);
            monitorComp.modApi = modApi;
            Log("started.");
            StartPlugins();
            AddModCoreCommands();
        }
        #endregion
        

        #region methods
        public void StartPlugins()
        {
            try
            {
                name_plugin = new Dictionary<string, IPlugin>();
                allCommands = new Dictionary<string, CommandDescription>();
                plugin_Commands = new Dictionary<string, CommandDescription[]>();

                // search for dll files
                Log("searching for dll files");
                path = Environment.CurrentDirectory + "\\StarForge_Data\\Managed\\";

                string[] dllFileNames = null;
                if (Directory.Exists(path))
                {
                    dllFileNames = Directory.GetFiles(path, "*.dll");
                }
                else
                {
                    LogError("failed to locate Managed folder.\nPath: " + path);
                    return;
                }

                // load assemblies
                Log("Loading assemblies.");
                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                foreach (string dllFile in dllFileNames)
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                    Assembly assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }

                if (assemblies.Count > 0)
                {
                    // search for types that implement IPlugin
                    Log("searching for types that implement IPlugin.");
                    Type pluginType = typeof(IPlugin);
                    ICollection<Type> pluginTypes = new List<Type>();
                    foreach (Assembly assembly in assemblies)
                    {
                        if (assembly != null)
                        {
                            Type[] types = assembly.GetTypes();
                            foreach (Type type in types)
                            {
                                if (type.IsInterface || type.IsAbstract)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (type.GetInterface(pluginType.FullName) != null)
                                    {
                                        pluginTypes.Add(type);
                                    }
                                }
                            }
                        }
                    }

                    // create instances from types.
                    Log("creating instances from types.");
                    plugins = new List<IPlugin>(pluginTypes.Count);
                    foreach (Type type in pluginTypes)
                    {
                        IPlugin plugin = (IPlugin)System.Activator.CreateInstance(type);
                        plugin.Init(this);
                        plugins.Add(plugin);
                        name_plugin.Add(plugin.Name, plugin);
                    }

                    Log("Loaded " + plugins.Count + " plugins.");
                }
                else
                {
                    LogWarning("No assemblies to load.");
                }
            }
            catch (Exception e)
            {
                LogError(e);
                LogError("Failed to load mods.");
            }
        }

        public void AddCommands(string pluginName, List<CommandDescription> newCommands)
        {
            foreach (CommandDescription cmd in newCommands)
            {
                allCommands.Add(cmd.command, cmd);
            }
            plugin_Commands.Add(pluginName.ToLower(), newCommands.ToArray());
        }

        public void submit(string message, bool networked)
        {
            if (message.StartsWith("/"))
            {
                string[] args = message.Split(' ');

                switch (args[0].ToLower())
                {
                    case "/cheat":
                    case "/say":
                    case "/me":
                    case "/suicide ":
                        break;

                    case "/exit":
                        Application.Quit();
                        break;
                        
                    case "/help":
                        #region help
                        string tabString = string.Empty;
                        string gray = string.Empty;
                        string Red = string.Empty;
                        if (Application.loadedLevel == 2)
                        {
                            tabString = "[00ff00]--[ffffff]";
                            gray = "[ffffff]";
                            Red = "[ff0000]";
                        }

                        if (args.Length == 1)
                        {
                            Print("");
                            Print("Available commands (<> = required, [] = optional): ");
                            foreach (CommandDescription cmd in allCommands.Values)
                            {
                                Print(tabString + "/" + cmd.command + " " + cmd.command_args + " - " + cmd.description_small + "." + gray);
                            }
                            Print("");
                        }
                        else if (args.Length == 3 && args[1].ToLower().StartsWith("-p"))
                        {
                            if (plugin_Commands.ContainsKey(args[2].ToLower()))
                            {
                                Print("");
                                Print("Available commands (<> = required, [] = optional): ");
                                CommandDescription[] pluginCmds = plugin_Commands[args[2].ToLower()];
                                foreach (CommandDescription cmd in pluginCmds)
                                {
                                    Print(tabString + "/" + cmd.command + " " + cmd.command_args + ": " + cmd.description_small + "." + gray);
                                }
                                Print("");
                            }
                            else
                            {
                                PrintError("Plugin not found.");
                            }
                        }
                        else if (args.Length == 2 && !args[1].ToLower().StartsWith("-p"))
                        {
                            if (allCommands.ContainsKey(args[1]))
                            {
                                Print("");
                                Print("Command description (<> = required, [] = optional): ");
                                Print(Red + "Command: " + gray + "/" + allCommands[args[1]].command + " " + allCommands[args[1]].command_args + gray);
                                if (allCommands[args[1]].Equals(string.Empty))
                                {
                                    Print(Red + "Description: " + gray + allCommands[args[1]].description_small + "." + gray);
                                }
                                else
                                {
                                    Print(Red + "Short description: " + gray + allCommands[args[1]].description_small + "." + gray);
                                    Print(Red + "Long description: " + gray + allCommands[args[1]].description_Long + "." + gray);
                                }
                                Print("");
                            }
                            else
                                PrintError("Command not found.");
                        }
                        else if (args.Length == 2 && args[1].ToLower().StartsWith("-p"))
                        {
                            PrintError("Please specify a plugin.");
                        }
                        else if (args.Length > 3)
                        {
                            PrintError("To many arguments.");
                        }
                        else
                            PrintError("Unknown error.");

                        break;
                        #endregion

                    case "/plugins":
                        #region plugins
                        Print("");
                        Print("Plugins: ");
                        if (plugins != null)
                        {
                            foreach (IPlugin plugin in plugins)
                            {
                                Print("--" + plugin.Name);
                            }
                        }
                        break;
                        #endregion

                    case "/version":
                        Print(Version);
                        break;

                    default:
                        // send to other mods for processing.
                        #region default
                        bool received = false;
                        if (allCommands.ContainsKey(args[0].Replace("/","")))
                        {
                            if (plugins != null)
                            {
                                foreach (IPlugin plugin in plugins)
                                {
                                    if (plugin != null)
                                    {
                                        plugin.Submit(message);
                                    }
                                }
                            }
                            else
                                PrintError("plugins list is null.");
                        }
                        else
                            PrintError("Command \"" + args[0] + "\" doesn't exist.");
                        break;
                        #endregion
                }
            }
            else if (plugins != null)
            {
                foreach (IPlugin plugin in plugins)
                {
                    if (plugin != null)
                        plugin.Submit(message);
                }
            }
        }

        public void Log(object message)
        {
            Debug.Log("modCore.dll: " + message.ToString());
            monitorComp.Log("modCore.dll: " + message.ToString());
        }

        public void LogWarning(object message)
        {
            Debug.Log("modCore.dll Warning: " + message.ToString());
            monitorComp.Log("modCore.dll Warning: " + message.ToString());
        }

        public void LogError(object message)
        {
            Debug.Log("modCore.dll error: " + message.ToString());
            monitorComp.Log("modCore.dll error: " + message.ToString());
        }

        public void LogError(Exception e)
        {
            Debug.Log("modCore.dll error: \nmessage: " + e.Message + ",\nSource: " + e.Source + ",\nStackTrace: " + e.StackTrace);
        }

        public void Print_AsPlayer(object message)
        {
            if (Application.loadedLevel == 2)
                Console.Submit(message.ToString(), true);
            
        }

        public void Print(object message)
        {
            if (Application.loadedLevel == 2)
            {
                Console.AddMessage(message.ToString());
                monitorComp.Log(message.ToString());
            }
            else
            {
                monitorComp.Log(message.ToString());
            }
        }

        public void PrintWarning(object message)
        {
            if (Application.loadedLevel == 2)
            {
                Console.AddWarning("Warning: " + message.ToString());
                monitorComp.Log("Warning: " + message.ToString());
            }
            else
            {
                monitorComp.Log("Warning: " + message.ToString());
            }
        }

        public void PrintError(object message)
        {
            if (Application.loadedLevel == 2)
            {
                Console.AddError("Error: " + message.ToString());
                monitorComp.Log("Error: " + message.ToString());
            }
            else
            {
                monitorComp.Log("Error: " + message.ToString());
            }
        }

        public IPlugin GetPlugin(string name)
        {
            if (name_plugin.ContainsKey(name))
                return name_plugin[name];
            else
                return null;
        }

        [Obsolete("Please use ModApi.GetMesh instead.", false)]
        public Mesh GetMesh(string name)
        {
            return ModApi.GetMesh(name);
        }

        private void AddModCoreCommands()
        {
            List<CommandDescription> modCoreCommands = new List<CommandDescription>();
            modCoreCommands.Add(new CommandDescription("exit", string.Empty, "exits the game"));
            modCoreCommands.Add(new CommandDescription("help", "[-p]|[command] [pluginName]", "displays this prompt", "Displays the help prompt. Use the name of a plugin as the third argument if '-p' was used as the second to get a list of every command for that plugin or enter in a command name for the second argument to get a detailed description of the command. notice: you do not need to place a '/' in front of the command name for the second argument"));
            modCoreCommands.Add(new CommandDescription("plugins", string.Empty, "lists installed plugins"));
            modCoreCommands.Add(new CommandDescription("version", string.Empty, "Prints the current version of modCore."));
            AddCommands("modCore", modCoreCommands);
        }
        #endregion

    }
}

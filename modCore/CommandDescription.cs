using System;
using System.Collections.Generic;
using System.Text;

namespace modCore
{
    public struct CommandDescription
    {
        public string command;
        public string command_args;
        public string description_small;
        public string description_Long;

        public CommandDescription(string _command, string _command_args, string _description_small, string _description_Long)
        {
            command = _command.ToLower();
            command_args = _command_args;
            description_small = _description_small;
            description_Long = _description_Long;
        }

        public CommandDescription(string _command, string _command_args, string _description_small)
        {
            command = _command.ToLower();
            command_args = _command_args;
            description_small = _description_small;
            description_Long = string.Empty;
        }
    }
}

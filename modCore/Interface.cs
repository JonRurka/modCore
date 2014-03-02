using System;
using System.Collections.Generic;
using System.Text;

namespace modCore
{
    public interface IPlugin
    {
        string Name { get; }

        string Version { get; }

        void Init(ModCore core);

        bool Submit(string message);
    }
}

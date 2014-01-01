using System;
using System.Collections.Generic;
using System.Text;

namespace modCore
{
    public interface IPlugin
    {
        string Name { get; }

        void Init(ModCore core);

        bool Submit(string message);
    }
}

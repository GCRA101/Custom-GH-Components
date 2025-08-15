using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGHComponents
{
    public class PluginInfo : GH_AssemblyInfo
    {
        public override string Name => "BH Components";
        public override string Version => "1.0.0";
        public override string Description => "A set of components allowing to automate work in Grasshopper for Buro Happold Projects.";
        public override string AuthorName => "Giorgio Albieri";
        public override string AuthorContact => "giorgio.albieri@burohappold.com";
    }
}

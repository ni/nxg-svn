using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.MocCommon.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;
using ViewpointSystems.Svn.Plugin.Add;

namespace ViewpointSystems.Svn.Plugin.SubMenu
{
    public class SvnCommands
    {
        [Import]
        public ICompositionHost Host { get; set; }

        /// <summary>
        /// The "top level" command
        /// </summary>
        public static readonly ICommandEx SvnSubMenuCommand = new ShellRelayCommand()
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.SelectSvnCommand.ShellSelectionRelayCommand",
            LabelTitle = "View Svn Command List",
        };
    }
}

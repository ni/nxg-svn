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
using Svn.Plugin.Add;

namespace Svn.Plugin.SubMenu
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
            UniqueId = "Svn.Plugin.SelectSvnCommand.ShellSelectionRelayCommand",
            LabelTitle = "SVN",
        };
    }
}

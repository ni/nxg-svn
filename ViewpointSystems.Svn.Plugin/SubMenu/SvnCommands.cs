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
        public static readonly ICommandEx SvnSubMenuCommand = new ShellSelectionRelayCommand(SelectSvnCommand, CanSelectSvnCommand)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.SelectSvnCommand.ShellSelectionRelayCommand",
            LabelTitle = "View Svn Command List",
        };

        /// <summary>
        /// Command handler to open the list of SVN commands in a Sub-menu
        /// </summary>
        private static void SelectSvnCommand(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            var choiceParameter = parameter.QueryService<NationalInstruments.Core.ChoiceCommandParameter>().FirstOrDefault();
            if (choiceParameter != null)
            {
                var model = (VisualModel)((selection.First()).Model);
                using (var transaction = model.TransactionManager.BeginTransaction("Execute Svn Command", TransactionPurpose.User))
                {
                    transaction.Commit();
                }
            }
        }

        public static bool CanSelectSvnCommand(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            return true;
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Core;
using NationalInstruments.Restricted.SourceModel.Envoys;
using NationalInstruments.SourceModel.Envoys;
using NationalInstruments.SourceModel;

namespace Svn.Plugin.FileWatcher
{
    [ExportEnvoyServiceFactory()]
    [BindsToKeyword(EnvoyManager.EnvoyManagerKeyword)]
    [BindOnLoaded]
    public class FilenameWatcherServiceFactory : EnvoyServiceFactory
    {
        protected override EnvoyService CreateService()
        {
            return new FilenameWatcherService();
        }

        private class FilenameWatcherService : EnvoyService
        {
            protected override void OnAttached(Envoy associatedEnvoy)
            {
                base.OnAttached(associatedEnvoy);
                associatedEnvoy.TransactionManager.TransactionStateChanged += HandleTransactionStateChanged;
            }

            private void HandleTransactionStateChanged(object sender, NationalInstruments.SourceModel.TransactionEventArgs e)
            {
                var fileNameTransactions = e.Transactions.Where(t => t.TargetElement is SourceFileReference && t.PropertyName == SourceFileReference.StoragePathPropertySymbol.Name);
                foreach (var transaction in fileNameTransactions)
                {
                    var path = ((SourceFileReference)transaction.TargetElement).StoragePath;
                    Log.WriteLine(path);
                }
                foreach (var objectChange in e.Transactions.OfType<OwnerComponentTransactionItem>())
                {
                    var element = objectChange.TargetElement as FileReference;
                    if (element != null)
                    {
                        if (objectChange.NewOwner != null)
                        {
                            // This is an add, process the element
                            var path = element.StoragePath;
                        }
                        if (objectChange.NewOwner == null)
                        {
                            // This is a remove, process the element
                            var path = element.StoragePath;
                        }
                    }
                }
            }
        }
    }
}

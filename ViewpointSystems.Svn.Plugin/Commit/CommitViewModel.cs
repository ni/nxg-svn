using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ViewpointSystems.Svn.Plugin.Commit
{
    public class CommitViewModel
    {
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        private bool okButtonClicked;
        public bool OkButtonClicked
        {
            get { return okButtonClicked; }
            set
            {
                okButtonClicked = value;
                OnPropertyChanged();
            }
        }

        private string commitMessage;
        public string CommitMessage
        {
            get { return commitMessage; }
            set
            {
                commitMessage = value;
                OnPropertyChanged();
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

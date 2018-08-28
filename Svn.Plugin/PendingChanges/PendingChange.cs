using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Shell;

namespace Svn.Plugin.PendingChanges
{
    public class PendingChange : INotifyPropertyChanged
    {

        private string path;
        /// <summary>
        /// Full path to file
        /// </summary>
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                Extension = System.IO.Path.GetExtension(value);
                OnPropertyChanged();
            }
        }

        private string extension;
        /// <summary>
        /// File Extention
        /// </summary>
        public string Extension
        {
            get { return extension; }
            set
            {
                extension = value;
                OnPropertyChanged();
            }
        }

        private string status;
        /// <summary>
        /// Status
        /// </summary>
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        private bool selected;
        /// <summary>
        /// Selected
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
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

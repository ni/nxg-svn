using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn.SvnThings
{
    public class SvnFileStatus : INotifyPropertyChanged
    {
        private string filename;
        public string Filename
        {
            get { return filename; }
            set { filename = value; RaisePropertyChanged("Filename"); }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; RaisePropertyChanged("FilePath"); }
        }

        private string version;
        public string Version
        {
            get { return version; }
            set { version = value; RaisePropertyChanged("Version"); }
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SvnFileStatus(string filename, string filepath, string status)
        {
            Filename = filename;
            FilePath = filepath;
            Version = status;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvvmCross.Wpf.Views;

namespace Svn.Wpf.Views
{


    /// <summary>
    ///  Defines the BaseView type.
    /// </summary>
    public abstract class BaseView : MvxWpfView
    {
        public string WindowTitle { get; set; }
    }
}

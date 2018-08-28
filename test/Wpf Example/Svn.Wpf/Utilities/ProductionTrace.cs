using MvvmCross.Platform.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Svn.Wpf.Utilities
{
    public class ProductionTrace : IMvxTrace
    {
        public void Trace(MvxTraceLevel level, string tag, string message)
        {
            switch (level)
            {
                case MvxTraceLevel.Diagnostic:
                    LogManager.GetLogger("Log").Info(message);
                    break;
                case MvxTraceLevel.Warning:
                    LogManager.GetLogger("Log").Warn(message);
                    break;
                case MvxTraceLevel.Error:
                    LogManager.GetLogger("Log").Error(message);
                    break;
            }

        }

        public void Trace(MvxTraceLevel level, string tag, Func<string> message)
        {
            switch (level)
            {
                case MvxTraceLevel.Diagnostic:
                    LogManager.GetLogger("Log").Info(message);
                    break;
                case MvxTraceLevel.Warning:
                    LogManager.GetLogger("Log").Warn(message);
                    break;
                case MvxTraceLevel.Error:
                    LogManager.GetLogger("Log").Error(message);
                    break;
            }
        }

        public void Trace(MvxTraceLevel level, string tag, string message, params object[] args)
        {
            switch (level)
            {
                case MvxTraceLevel.Diagnostic:
                    LogManager.GetLogger("Log").Info(message, args);
                    break;
                case MvxTraceLevel.Warning:
                    LogManager.GetLogger("Log").Warn(message, args);
                    break;
                case MvxTraceLevel.Error:
                    LogManager.GetLogger("Log").Error(message, args);
                    break;
            }
        }
    }
}

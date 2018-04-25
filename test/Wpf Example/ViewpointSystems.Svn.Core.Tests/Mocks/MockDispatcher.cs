using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform.Core;

namespace ViewpointSystems.Svn.Core.Tests.Mocks
{
    /// <summary>
    /// Defines the MockDispatcher type.
    /// </summary>
    public class MockDispatcher
        : MvxMainThreadDispatcher, IMvxViewDispatcher
    {
        /// <summary>
        /// The requests
        /// </summary>
        public readonly List<MvxViewModelRequest> Requests = new List<MvxViewModelRequest>();

        public bool CloseRequested = false;

        /// <summary>
        /// Requests the main thread action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>returns true.</returns>
        public bool RequestMainThreadAction(Action action)
        {
            action();
            return true;
        }

        /// <summary>
        /// Shows the view model.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>return true.</returns>
        public bool ShowViewModel(MvxViewModelRequest request)
        {
            Requests.Add(request);
            return true;
        }

        /// <summary>
        /// Changes the presentation.
        /// </summary>
        /// <param name="hint">The hint.</param>
        /// <returns>an exception.</returns>
        public bool ChangePresentation(MvxPresentationHint hint)
        {
            CloseRequested = hint is MvxClosePresentationHint;
            return true;
        }
    }
}

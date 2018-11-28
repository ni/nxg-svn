using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NationalInstruments.SourceModel.Persistence;

namespace Svn.Plugin
{
    /// <summary>
    /// Implements namespace versioning for elements in this assembly.
    /// </summary>
    [ParsableNamespaceSchema(ParsableNamespaceName, CurrentVersion)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class ExamplePluginsNamespaceSchema : NamespaceSchema
    {
        /// <summary>
        /// Namespace name as an XNamespace
        /// </summary>
        public static readonly XNamespace XmlNamespace = XNamespace.Get(ParsableNamespaceName);

        /// <summary>
        /// Namespace name
        /// </summary>
        public const string ParsableNamespaceName = "http://www.viewpointusa.com/svn";

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ExamplePluginsNamespaceSchema()
            : base(Assembly.GetExecutingAssembly())
        {
        }

        /// <summary>
        /// The current version
        /// </summary>
        public const string CurrentVersion = "1.0.0";

        /// <inheritdoc/>
        public override string NamespaceName
        {
            get { return ParsableNamespaceName; }
        }

        /// <inheritdoc/>
        public override string FeatureSetName
        {
            get { return "Svn"; }
        }
    }
}

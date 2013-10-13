using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt eine C++ Projektdatei.
    /// </summary>
    public class CPlusPlusProject : ProjectTypeHandler
    {
        /// <summary>
        /// Der XPath zum Namen der Assembly.
        /// </summary>
        protected override string TargetNamePath { get { return "msbuild:Project/msbuild:PropertyGroup[@Label='Globals']/msbuild:ProjectName"; } }

        /// <summary>
        /// Initialisiert eine Verwaltung.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Projektdatei.</param>
        public CPlusPlusProject( FileInfo path )
            : base( path )
        {
        }

        /// <summary>
        /// Ermittelt alle Abhängigkeiten.
        /// </summary>
        protected override IEnumerable<string> References
        {
            get
            {
                // Request from list of additional dependencies - may be LIB only
                return
                    Document
                        .SelectNodes( "//msbuild:Link/msbuild:AdditionalDependencies", Namespaces )
                        .Cast<XmlElement>()
                        .Select( node => node.InnerText )
                        .Where( list => !string.IsNullOrWhiteSpace( list ) )
                        .SelectMany( list => list.Trim().Split( ';' ) )
                        .Where( name => !string.IsNullOrWhiteSpace( name ) )
                        .Select( name => Path.GetFileNameWithoutExtension( name.Trim() ) )
                        .Concat( base.References );
            }
        }
    }
}

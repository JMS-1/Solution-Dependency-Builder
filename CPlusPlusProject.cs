using System.IO;


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
    }
}

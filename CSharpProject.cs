using System.IO;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt eine C# Projektdatei.
    /// </summary>
    public class CSharpProject : ProjectTypeHandler
    {
        /// <summary>
        /// Der XPath zum Namen der Assembly.
        /// </summary>
        protected override string TargetNamePath { get { return "msbuild:Project/msbuild:PropertyGroup[not(@Condition)]/msbuild:AssemblyName"; } }

        /// <summary>
        /// Initialisiert eine Verwaltung.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Projektdatei.</param>
        public CSharpProject( FileInfo path )
            : base( path )
        {
        }
    }
}

using System.IO;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt eine C# Projektdatei.
    /// </summary>
    public class SolutionOnlyProject : ProjectTypeHandler
    {
        /// <summary>
        /// Der XPath zum Namen der Assembly.
        /// </summary>
        protected override string TargetNamePath { get { return null; } }

        /// <summary>
        /// Initialisiert eine Verwaltung.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Projektdatei.</param>
        public SolutionOnlyProject( FileInfo path )
        {
        }
    }
}

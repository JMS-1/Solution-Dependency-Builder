using System.IO;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt ein Installationprogramm.
    /// </summary>
    public class SetupProject : ProjectTypeHandler
    {
        /// <summary>
        /// Der XPath zum Namen der Assembly.
        /// </summary>
        protected override string TargetNamePath { get { return null; } }

        /// <summary>
        /// Initialisiert eine Verwaltung.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Projektdatei.</param>
        public SetupProject( FileInfo path )
        {
        }
    }
}

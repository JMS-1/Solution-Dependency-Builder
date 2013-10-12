using System.IO;
using System.Xml;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt die Verwaltung einer Art von Projekt.
    /// </summary>
    public abstract class ProjectTypeHandler
    {
        /// <summary>
        /// Die Rohdaten.
        /// </summary>
        private readonly XmlDocument m_raw = new XmlDocument();

        /// <summary>
        /// Der volle Pfad zur Projektdatei.
        /// </summary>
        private readonly FileInfo m_path;

        /// <summary>
        /// Initialisiert eine Verwaltung.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Projektdatei.</param>
        protected ProjectTypeHandler( FileInfo path )
        {
            // Remember
            m_path = path;

            // Load
            m_raw.Load( m_path.FullName );
        }
    }
}

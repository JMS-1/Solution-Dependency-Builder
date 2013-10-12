using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt die globalen EInstellungen der Solutiondatei.
    /// </summary>
    public class GlobalSection : SolutionFileSection
    {
        /// <summary>
        /// Das Vergleichsmuster zum Erkennen der globalen Einstellungen.
        /// </summary>
        public static readonly Regex StartPattern = new Regex( "^[\\s]*Global*$", RegexOptions.Compiled );

        /// <summary>
        /// Das Vergleichsmuster zum Erkennen des Endes der Einstellungen.
        /// </summary>
        private static readonly Regex EndPattern = new Regex( "^[\\s]*EndGlobal[\\s]*$", RegexOptions.Compiled );

        /// <summary>
        /// Alle Zeilen dieses Bereichs.
        /// </summary>
        private readonly List<SolutionFileSection> m_sections = new List<SolutionFileSection>();

        /// <summary>
        /// Die aktuelle Auswertung.
        /// </summary>
        private SolutionFileSection m_factory;

        /// <summary>
        /// Erstellt einen neuen Bereich mit Einstellungen.
        /// </summary>
        /// <param name="header">Die bereits analysierte Kopfzeile.</param>
        /// <param name="solutionPath">Der volle Pfad zur Solutiondatei.</param>
        public GlobalSection( Match header, FileInfo solutionPath )
        {
        }

        /// <summary>
        /// Ergänzt eine weitere Zeile.
        /// </summary>
        /// <param name="line">Die gewünschte Zeile.</param>
        /// <returns>Gesetzt, wenn weitere Zeilen aufgenommen werden können.</returns>
        public override bool Extend( string line )
        {
            // Create factory
            if (m_factory == null)
            {
                // Fallback
                m_factory = new PassThroughLine();

                // Remember
                m_sections.Add( m_factory );
            }

            // Eat up
            if (m_factory.Extend( line ))
                return true;

            // Reset
            m_factory = null;

            // Check for full end - actually a bit of misuse of our factory pattern but who cares
            return !EndPattern.IsMatch( line );
        }

        /// <summary>
        /// Rekonstruiert den Inhalt.
        /// </summary>
        /// <returns>Der hier verwaltete Inhalt.</returns>
        public override IEnumerable<string> Reconstruct()
        {
            // Report
            return m_sections.SelectMany( section => section.Reconstruct() );
        }
    }
}

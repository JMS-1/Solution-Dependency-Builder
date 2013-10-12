using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt ein Projekt in der Solutiondatei.
    /// </summary>
    public class ProjectSection : SolutionFileSection
    {
        /// <summary>
        /// Das Vergleichsmuster zum Erkennen eines Projektabschnitts.
        /// </summary>
        public static readonly Regex Pattern = new Regex( "^[\\s]*Project\\(\"(?<id>\\{[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12}\\})\"\\)[\\s]*=[\\s]*\"[\\s]*(?<name>[^\"]*)[\\s]*\"[\\s]*,[\\s]*\"(?<path>[^\"]*)\"[\\s]*,[\\s]*\"(?<type>\\{[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12}\\})\"[\\s]*$", RegexOptions.Compiled );

        /// <summary>
        /// Alle Zeilen dieses Bereichs.
        /// </summary>
        private readonly List<string> m_lines = new List<string>();

        /// <summary>
        /// Die eindeutige Kennung des Projektes.
        /// </summary>
        public Guid UniqueIdentifier { get; private set; }

        /// <summary>
        /// Die eindeutige Kennung der Art des Projektes.
        /// </summary>
        public Guid ProjectTypeIdentifier { get; private set; }

        /// <summary>
        /// Der absolute Pfad zur Projektdatei.
        /// </summary>
        public FileInfo ProjectPath { get; private set; }

        /// <summary>
        /// Der Name des Projektes.
        /// </summary>
        public string ProjectName { get; private set; }

        /// <summary>
        /// Erstellt einen neuen Projektbereich.
        /// </summary>
        /// <param name="header">Die bereits analysierte Kopfzeile.</param>
        /// <param name="solutionPath">Der volle Pfad zur Solutiondatei.</param>
        public ProjectSection( Match header, FileInfo solutionPath )
        {
            // Load items from analysis
            ProjectPath = new FileInfo( Path.Combine( solutionPath.DirectoryName, header.Groups["path"].Value ) );
            ProjectTypeIdentifier = new Guid( header.Groups["type"].Value );
            UniqueIdentifier = new Guid( header.Groups["id"].Value );
            ProjectName = header.Groups["name"].Value;
        }

        /// <summary>
        /// Ergänzt eine weitere Zeile.
        /// </summary>
        /// <param name="line">Die gewünschte Zeile.</param>
        /// <returns>Gesetzt, wenn weitere Zeilen aufgenommen werden können.</returns>
        public override bool Extend( string line )
        {
            // Always remember
            m_lines.Add( line );

            // We are fed
            return !"EndProject".Equals( line );
        }

        /// <summary>
        /// Rekonstruiert den Inhalt.
        /// </summary>
        /// <returns>Der hier verwaltete Inhalt.</returns>
        public override IEnumerable<string> Reconstruct()
        {
            // Report
            return m_lines;
        }
    }
}

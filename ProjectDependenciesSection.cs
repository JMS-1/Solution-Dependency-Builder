using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt die Abhängigkeiten eines Projektes.
    /// </summary>
    public class ProjectDependenciesSection : SolutionFileSection
    {
        /// <summary>
        /// Das Vergleichsmuster zum Erkennen der Abhängigkeitsliste.
        /// </summary>
        public static readonly Regex StartPattern = new Regex( "^[\\s]*ProjectSection\\(ProjectDependencies\\)[\\s]*=[\\s]*postProject[\\s]*$", RegexOptions.Compiled );

        /// <summary>
        /// Das Vergleichsmuster zum Erkennen des Endes einer Abhängigkeitsliste.
        /// </summary>
        private static readonly Regex EndPattern = new Regex( "^[\\s]*EndProjectSection[\\s]*$", RegexOptions.Compiled );

        /// <summary>
        /// Eine einzelne Abhängigkeit.
        /// </summary>
        private static readonly Regex DependencyPattern = new Regex( "^[\\s]*(?<dep1>\\{[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12}\\})[\\s]=[\\s](?<dep2>\\{[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12}\\})[\\s]*$", RegexOptions.Compiled );

        /// <summary>
        /// Alle Zeilen dieses Bereichs.
        /// </summary>
        private readonly List<string> m_lines = new List<string>();

        /// <summary>
        /// Die Liste der Abhängigkeiten.
        /// </summary>
        public HashSet<Guid> Dependencies { get; private set; }

        /// <summary>
        /// Erstellt eine neue Abhängigkeitsliste.
        /// </summary>
        /// <param name="header">Die bereits analysierte Kopfzeile.</param>
        public ProjectDependenciesSection( Match header )
        {
            // Finish
            Dependencies = new HashSet<Guid>();
        }

        /// <summary>
        /// Erstellt eine neue Anhängigkeitsliste.
        /// </summary>
        public ProjectDependenciesSection()
            : this( null )
        {
            // Initialize
            m_lines.Add( "\tProjectSection(ProjectDependencies) = postProject" );
            m_lines.Add( "\tEndProjectSection" );
        }

        /// <summary>
        /// Aktualisiert die Abhängigkeiten.
        /// </summary>
        /// <param name="newDependencies">Die neue Liste der Abhängigkeiten.</param>
        public void UpdateDependencies( IEnumerable<Guid> newDependencies )
        {
            // Reset map
            Dependencies.Clear();

            // Reset text representation
            m_lines.RemoveAll( DependencyPattern.IsMatch );

            // Add new
            Dependencies.UnionWith( newDependencies );

            // Create lines
            foreach (var dependency in Dependencies)
                m_lines.Insert( 1, string.Format( "\t\t{0} = {0}", dependency.ToString( "B" ).ToUpper() ) );
        }

        /// <summary>
        /// Ergänzt eine weitere Zeile.
        /// </summary>
        /// <param name="line">Die gewünschte Zeile.</param>
        /// <returns>Gesetzt, wenn weitere Zeilen aufgenommen werden können.</returns>
        public override bool Extend( string line )
        {
            // Remember
            m_lines.Add( line );

            // Check match
            var match = DependencyPattern.Match( line );
            if (match.Success)
            {
                // Load identifier
                var id1 = new Guid( match.Groups["dep1"].Value );
                var id2 = new Guid( match.Groups["dep2"].Value );
                if (id1 != id2)
                    throw new NotSupportedException( string.Format( "Bad Dependency: {0}", line.Trim() ) );

                // Remember
                if (!Dependencies.Add( id1 ))
                    throw new NotSupportedException( string.Format( "Duplicate Dependency: {0}", line.Trim() ) );
            }

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
            if (m_lines.Count < 3)
                return Enumerable.Empty<string>();
            else
                return m_lines;
        }
    }
}

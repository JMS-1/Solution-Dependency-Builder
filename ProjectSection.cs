using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt ein Projekt in der Solutiondatei.
    /// </summary>
    public class ProjectSection : SolutionFileSection
    {
        /// <summary>
        /// Alle bekannten Arten von Projekten.
        /// </summary>
        private static readonly Dictionary<Guid, Func<FileInfo, ProjectTypeHandler>> _SupportedProjectTypes =
            new Dictionary<Guid, Func<FileInfo, ProjectTypeHandler>>
            {
                { new Guid( "{fae04ec0-301f-11d3-bf4b-00c04f79efbc}" ), path => new CSharpProject(path) },
                { new Guid( "{8bc9ceb8-8b4a-11d0-8d11-00a0c91bc942}" ), path => new CPlusPlusProject(path) },
                { new Guid( "{2150e333-8fdc-42a3-9474-1a3956d46de8}" ), path => new SolutionOnlyProject(path) },
            };

        /// <summary>
        /// Das Vergleichsmuster zum Erkennen eines Projektabschnitts.
        /// </summary>
        public static readonly Regex StartPattern = new Regex( "^[\\s]*Project\\(\"(?<type>\\{[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12}\\})\"\\)[\\s]*=[\\s]*\"[\\s]*(?<name>[^\"]*)[\\s]*\"[\\s]*,[\\s]*\"(?<path>[^\"]*)\"[\\s]*,[\\s]*\"(?<id>\\{[0-9A-Fa-f]{8}(-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12}\\})\"[\\s]*$", RegexOptions.Compiled );

        /// <summary>
        /// Das Vergleichsmuster zum Erkennen des Endes eines Projekabschnitts.
        /// </summary>
        private static readonly Regex EndPattern = new Regex( "^[\\s]*EndProject[\\s]*$", RegexOptions.Compiled );

        /// <summary>
        /// Alle Zeilen dieses Bereichs.
        /// </summary>
        private readonly List<SolutionFileSection> m_sections = new List<SolutionFileSection>();

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
        /// Die aktuelle Auswertung.
        /// </summary>
        private SolutionFileSection m_factory;

        /// <summary>
        /// Die zugehörige Projektdatei.
        /// </summary>
        public ProjectTypeHandler ProjectFile { get; private set; }

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

            // Read the handler factory
            Func<FileInfo, ProjectTypeHandler> factory;
            if (!_SupportedProjectTypes.TryGetValue( ProjectTypeIdentifier, out factory ))
                throw new NotSupportedException( string.Format( "Unsupported Project Type {0}", header.Groups["type"].Value ) );

            // Time to load project file
            ProjectFile = factory( ProjectPath );
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
                // Check mode
                var match = ProjectDependenciesSection.StartPattern.Match( line );
                if (match.Success)
                {
                    // Create new
                    m_factory = new ProjectDependenciesSection( match );
                }
                else
                {
                    // Fallback
                    m_factory = new PassThroughLine();
                }

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

        /// <summary>
        /// Löst alle Referenzen auf.
        /// </summary>
        /// <param name="projects">Alle bekannten Projekte.</param>
        /// <param name="projectLookup">Projekte zum Nachschlagen nach der eindeutigen Kennung.</param>
        public void Resolve( Dictionary<string, ProjectSection> projects, Dictionary<Guid, ProjectSection> projectLookup )
        {
            // Forward
            ProjectFile.Resolve( projects );

            // Attach to existing dependencies
            var dependencySection = m_sections.OfType<ProjectDependenciesSection>().SingleOrDefault();
            var dependencies = (dependencySection != null) ? dependencySection.Dependencies : new HashSet<Guid>();

            // Add
            foreach (var newDependency in ProjectFile.Dependencies)
                if (!dependencies.Contains( newDependency ))
                {
                    // Report
                    if (Program.Logging)
                        Console.WriteLine( "\t+{0}: {1}", ProjectFile.FilePath.FullName, projectLookup[newDependency].ProjectName );
                }

            // Remove
            foreach (var oldDependency in dependencies)
                if (!ProjectFile.Dependencies.Contains( oldDependency ))
                {
                    // Report
                    if (Program.Logging)
                    {
                        // See if we know the project
                        ProjectSection project;
                        projectLookup.TryGetValue( oldDependency, out project );

                        // Can now report
                        Console.WriteLine( "\t-{0}: {1}", ProjectFile.FilePath.FullName, (project == null) ? oldDependency : (object) project.ProjectName );
                    }
                }
        }
    }
}

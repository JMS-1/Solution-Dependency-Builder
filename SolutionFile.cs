﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Repräsentiert eine Solution als Ganzes.
    /// </summary>
    public class SolutionFile
    {
        /// <summary>
        /// Der volle Pfad zur Datei.
        /// </summary>
        private readonly FileInfo m_path;

        /// <summary>
        /// Alle Bereiche der Datei.
        /// </summary>
        private readonly List<SolutionFileSection> m_sections = new List<SolutionFileSection>();

        /// <summary>
        /// Erstellt eine Solution.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Datei.</param>
        public SolutionFile( string path )
        {
            // Remember
            m_path = new FileInfo( path );

            // Current factory
            SolutionFileSection factory = null;

            // Parse it
            foreach (var line in File.ReadAllLines( m_path.FullName ))
            {
                // Create the factory
                if (factory == null)
                {
                    // Test supported items
                    var match = ProjectSection.Pattern.Match( line );
                    if (match.Success)
                    {
                        // Project section
                        factory = new ProjectSection( match, m_path );
                    }
                    else
                    {
                        // Fallback
                        factory = new PassThroughLine();
                    }

                    // Remember
                    m_sections.Add( factory );
                }

                // Feed
                if (!factory.Extend( line ))
                    factory = null;
            }
        }

        /// <summary>
        /// Stellt die gesamte Datei wieder her.
        /// </summary>
        /// <returns>Alle Zeilen der Datei.</returns>
        public IEnumerable<string> Reconstruct()
        {
            // Merge
            return m_sections.SelectMany( section => section.Reconstruct() );
        }
    }
}

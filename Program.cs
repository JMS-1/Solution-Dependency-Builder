using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Repräsentiert den Windows Prozess als Ganzes.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Gesetzt, wenn die Protokollierung aktiviert ist.
        /// </summary>
        public static bool Logging { get; private set; }

        /// <summary>
        /// Gesetzt, wen ndie aktualisierte Datei geschrieben werden soll.
        /// </summary>
        public static bool Update { get; private set; }

        /// <summary>
        /// Startet die Anwendung.
        /// </summary>
        /// <param name="args">Die Liste der Befehlszeilenparameter.</param>
        public static void Main( string[] args )
        {
            // Create command map
            var map = new HashSet<string>( args, StringComparer.InvariantCultureIgnoreCase );

            // Check flags
            Update = map.Contains( "/write" );
            Logging = map.Contains( "/log" );

            // Process files
            foreach (var path in map.Where( arg => arg.StartsWith( "/solution=", StringComparison.InvariantCultureIgnoreCase ) ).Select( arg => arg.Substring( 10 ) ))
                Process( path );
        }

        /// <summary>
        /// Bearbeitet eine einzelne Solution.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Solution.</param>
        private static void Process( string path )
        {
            // Report
            if (Logging)
                Console.WriteLine( "Processing {0}", path );

            // Load the solution file
            var solution = new SolutionFile( path );

            // Skip
            if (Update)
            {
                // Create the new file name
                var newPath = Path.Combine( Path.GetDirectoryName( path ), Path.GetFileNameWithoutExtension( path ) + " (Build)" + Path.GetExtension( path ) );

                // Report
                if (Logging)
                    Console.WriteLine( "Creating updated Solution in {0}", newPath );

                // Store
                File.WriteAllLines( newPath, solution.Reconstruct(), Encoding.UTF8 );
            }

            // Report
            if (Logging)
                Console.WriteLine();
        }
    }
}

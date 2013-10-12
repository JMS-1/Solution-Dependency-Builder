using System.Collections.Generic;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Repräsentiert eine einzelne Textzeile.
    /// </summary>
    public class PassThroughLine : SolutionFileSection
    {
        /// <summary>
        /// Die gewünschte Zeile.
        /// </summary>
        public string Contents { get; private set; }

        /// <summary>
        /// Ergänzt eine weitere Zeile.
        /// </summary>
        /// <param name="line">Die gewünschte Zeile.</param>
        /// <returns>Gesetzt, wenn weitere Zeilen aufgenommen werden können.</returns>
        public override bool Extend( string line )
        {
            // Eat
            Contents = line;

            // We are fed
            return false;
        }

        /// <summary>
        /// Rekonstruiert den Inhalt.
        /// </summary>
        /// <returns>Der hier verwaltete Inhalt.</returns>
        public override IEnumerable<string> Reconstruct()
        {
            // Report
            yield return Contents;
        }
    }
}

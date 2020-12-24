using System.Collections.Generic;
using System.Windows;

namespace ART_TELEMETRY_APP.Tracks.Classes
{
    /// <summary>
    /// Represents a track for the <see cref="Driverless"/> section.
    /// </summary>
    class DriverlessTrack
    {
        /// <summary>
        /// Track name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Track width in <b>meter</b>.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Track length in <b>meter</b>.
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// Left side points.
        /// </summary>
        public List<Point> LeftSide { get; set; } = new List<Point>();

        /// <summary>
        /// Right side points.
        /// </summary>
        public List<Point> RightSide { get; set; } = new List<Point>();

        /// <summary>
        /// Center points.
        /// </summary>
        public List<Point> Center{ get; set; } = new List<Point>();
    }
}

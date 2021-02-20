using System.Collections.Generic;
using Telemetry_data_and_logic_layer.Colors;

namespace Telemetry_data_and_logic_layer.Groups
{
    /// <summary>
    /// Represents a channel that is a column in an <seealso cref="InputFile"/>.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Initializes:
        /// <list type="bullet">
        ///    <item>
        ///        <term><see cref="Name"/></term>
        ///        <description><paramref name="name"/></description>
        ///    </item>
        ///     <item>
        ///        <term><see cref="Data"/></term>
        ///        <description>New empty list</description>
        ///    </item>
        ///     <item>
        ///        <term><see cref="Color"/></term>
        ///        <description>Random chart color</description>
        ///    </item>
        ///</list>
        /// </summary>
        /// <param name="name">Channel name.</param>
        public Channel(int id, string name)
        {
            ID = id;
            Name = name;
            Data = new List<double>();
            Color = ColorManager.GetChartColor;
        }

        public int ID { get; set; }

        /// <summary>
        /// Name of the channel.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Data stored in the channel.
        /// </summary>
        public List<double> Data { get; private set; }

        /// <summary>
        /// Is the channel active or not.
        /// Default is false.
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Color of the channel represented in string.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Line width of the channel.
        /// </summary>
        public int LineWidth { get; set; } = 1;

        /// <summary>
        /// Add <paramref name="value"/> to the channel.
        /// </summary>
        /// <param name="value">Value to add.</param>
        public void AddChannelData(float value) => Data.Add(value);
    }
}

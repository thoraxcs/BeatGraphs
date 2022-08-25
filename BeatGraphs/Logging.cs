using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeatGraphs
{
    // Enum for possible log levels
    // TODO: If I make a place for enums, this might need to go there.
    public enum LogLevel
    {
        verbose,
        info,
        warning,
        error
    }

    /// <summary>
    /// Extensions for RichTextBox form object
    /// </summary>
    public static class RichTextBoxExtensions
    {
        /// <summary>
        /// Inserts the passed in text to the RichTextBox colored appropriately based on the desired log level
        /// </summary>
        public static void AppendText(this RichTextBox box, string text, LogLevel level)
        {
            // Only display verbose logs when requested
            if (Settings.Get("verbose") || level != LogLevel.verbose)
            {
                box.SelectionStart = box.TextLength;
                box.SelectionLength = 0;

                // Set line color
                Color color;
                switch (level)
                {
                    case LogLevel.warning:
                        color = Color.Yellow;
                        break;
                    case LogLevel.error:
                        color = Color.Red;
                        break;
                    default:
                        color = Color.White;
                        break;
                }

                box.SelectionColor = color;
                box.AppendText(text);
                box.SelectionColor = box.ForeColor;
                box.ScrollToCaret(); // Force box to bottom.
            }
        }
    }
}

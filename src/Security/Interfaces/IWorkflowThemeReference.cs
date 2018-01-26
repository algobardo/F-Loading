using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    public interface IWorkflowThemeReference
    {
        /// <summary>
        /// Set the theme for a partiular Form.
        /// Some theme variables can be null;
        /// </summary>
        /// <param name="form"></param>
        /// <param name="theme"></param>
        /// <returns>A <see cref="Security.SetThemeResult"/> that contains the
        /// status of the operation</returns>
        SetThemeResult SetTheme(Theme theme);

        /// <summary>
        /// Set an image for the theme for a particular Form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="image"></param>
        /// <returns>A <see cref="Security.SetThemeResult"/> that contains the
        /// status of the operation</returns>
        SetThemeResult SetImage(System.Drawing.Image image);

        /// <summary>
        /// Set the css for the theme for a particular Form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="css"></param>
        /// <returns>A <see cref="Security.SetThemeResult"/> that contains the
        /// status of the operation</returns>
        SetThemeResult SetCSS(string css);

        /// <summary>
        /// Set the title for the theme for a particular Form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        SetThemeResult SetTitle(string title);

        /// <summary>
        /// Return the current theme for a particular Form.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        Theme GetTheme();
    }
}

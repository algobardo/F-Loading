using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebInterface
{
    interface IThemeEditor
    {

        /// <summary>
        /// Set a null theme for a form
        /// </summary>
        /// <returns>A JSON formatted string with the status result.</returns>
        String SkipThemeStep();
    }
}

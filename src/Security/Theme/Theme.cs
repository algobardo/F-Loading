using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Security
{
    public class Theme
    {
        private string title;
        private Image image;
        private string css;

        /// <summary>
        /// Create and set a theme
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Image"></param>
        /// <param name="CSS"></param>
        public Theme(string Title, Image Image, string CSS)
        {
            this.title = Title;
            this.image = Image;
            this.css = CSS;
        }

        /// <summary>
        /// The title of the theme
        /// </summary>
        public string Title { get { return title; } set { title = value; } }

        /// <summary>
        /// The logo image of the theme
        /// </summary>
        public Image Image { get { return image; } set { image = value; } }

        /// <summary>
        /// The CSS stylesheet of the theme
        /// </summary>
        public string CSS { get { return css; } set { css = value; } }
    }
}

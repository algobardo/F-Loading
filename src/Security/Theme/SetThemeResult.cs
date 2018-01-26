using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    public class SetThemeResult
    {
        public enum Result { 
            STATUS_OK, 
            STATUS_ERROR_CSS,
            STATUS_ERROR_IMAGE,
            STATUS_ERROR_TITLE,
            STATUS_ERROR_THEME
        }
        
        private Result status;
        private Theme resultTheme;

        public SetThemeResult(Result status, Theme resultTheme)
        {
            this.status = status;
            this.resultTheme = resultTheme;
        }

        public Theme getTheme()
        {
            return resultTheme;
        }

        public Result getStatus()
        {
            return status;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServices
{
    class CLIService: HTTPSService
    {
        public CLIService() : base("https://www.cli.di.unipi.it/Auth/Auth.php","Centro di Calcolo - Polo Fibonacci")
        {
            
        }
    }
}

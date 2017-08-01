using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace WebApi.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<StartUp>("http://localhost:9001"))
            {
                Console.WriteLine("Server started on port 9001... press ENTER to shut down");
                Console.ReadLine();
            }
        }
    }
}

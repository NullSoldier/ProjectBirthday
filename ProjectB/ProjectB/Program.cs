using System;

namespace ProjectB
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ProjectB projectB = new ProjectB())
            {
                projectB.Run();
            }
        }
    }
#endif
}


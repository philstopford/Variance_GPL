using Eto.Forms;
using Eto.Veldrid;
using Eto.Veldrid.Mac;
using System;
using System.Diagnostics;
using System.IO;
using Veldrid;

namespace Variance.Mac
{
    public static class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string xmlFile = "";
            int numberOfThreads = -1; // -1 corresponds to using all threads that are detected.
            int graphicsMode = -1;

            if (args.Length > 0)
            {
                int oneThreadIndex = Array.IndexOf(args, "--1thread");
                int threadsIndex = Array.IndexOf(args, "--threads");
                int graphicsIndex = Array.IndexOf(args, "--graphicsMode");

                if (oneThreadIndex != -1)
                {
                    numberOfThreads = -1;
                }
                else
                {
                    if (threadsIndex != -1)
                    {
                        numberOfThreads = Math.Max(1, Convert.ToInt32(args[threadsIndex + 1]));
                    }
                }

                if (graphicsIndex != -1)
                {
                    switch (args[graphicsIndex + 1].ToLower())
                    {
                        case "opengl":
                            graphicsMode = (int)GraphicsBackend.OpenGL;
                            break;
                        case "vulkan":
                            graphicsMode = (int)GraphicsBackend.Vulkan;
                            break;
                        case "metal":
                        default:
                            graphicsMode = (int)GraphicsBackend.Metal;
                            break;
                    }
                }

                int i = 0;
                bool done = false;
                while (!done && (i < args.Length))
                {
                    // Extract XML file.
                    try
                    {
                        string[] tokens = args[i].Split(new char[] { '.' });
                        string extension = tokens[tokens.Length - 1];
                        if ((extension.ToUpper() == "VARIANCE") || (extension.ToUpper() == "XML"))
                        {
                            xmlFile = args[i];
                            done = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    i++;
                }

                // File validation
                if (xmlFile != "" && (!System.IO.File.Exists(xmlFile)))
                {
                    Error.ErrorReporter.showMessage_OK("Unable to find project file: " + xmlFile, "ERROR");
                    xmlFile = "";
                }
            }

            GraphicsBackend backend = VeldridSurface.PreferredBackend;

            if (graphicsMode != -1)
            {
                try
                {
                    backend = (GraphicsBackend)graphicsMode;
                }
                catch (Exception)
                {
                    // avoid changing the backend from the preferred case.
                }
            }

            var platform = new Eto.Mac.Platform();
            platform.Add<VeldridSurface.IHandler>(() => new MacVeldridSurfaceHandler());

            Int32 HTCount = Environment.ProcessorCount;

            VarianceContextGUI varianceContext = new VarianceContextGUI(false, xmlFile, numberOfThreads,
                                                        HTCount, backend);
            // run application with our main form
            VarianceApplication va = new VarianceApplication(platform, varianceContext);
            va.Run();
        }
    }
}

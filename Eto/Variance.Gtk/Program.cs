﻿using Eto.Veldrid;
using Eto.Veldrid.Gtk;
using System;
using Veldrid;

namespace Variance.Gtk;

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
                numberOfThreads = 1;
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
                    default:
                        graphicsMode = (int)GraphicsBackend.Vulkan;
                        break;
                }
            }

            int i = 0;
            bool done = false;
            while (!done && i < args.Length)
            {
                // Extract XML file.
                try
                {
                    string[] tokens = args[i].Split(new[] { '.' });
                    string extension = tokens[^1];
                    if (extension.ToUpper() == "VARIANCE" || extension.ToUpper() == "XML")
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
            if (xmlFile != "" && !System.IO.File.Exists(xmlFile))
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

        int HTCount = Environment.ProcessorCount;

        var platform = new Eto.GtkSharp.Platform();
        platform.Add<VeldridSurface.IOpenGL>(() => new GtkVeldridSurfaceHandler());

        VarianceContextGUI varianceContext = new(false, xmlFile, numberOfThreads,
            HTCount, backend);
        // run application with our main form
        VarianceApplication va = new(platform, varianceContext);
        va.Run();
    }
}
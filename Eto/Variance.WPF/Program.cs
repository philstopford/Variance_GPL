using System;
using Eto.Veldrid;
using Eto.Veldrid.Wpf;
using Veldrid;

namespace Variance.WPF;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        string xmlFile = "";
        int numberOfThreads = -1; // -1 corresponds to using all threads that are detected.
        int graphicsMode = -1;
        bool dark = false;

        if (args.Length > 0)
        {
            int oneThreadIndex = Array.IndexOf(args, "--1thread");
            int threadsIndex = Array.IndexOf(args, "--threads");
            dark = Array.IndexOf(args, "--dark") != -1;
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
                    case "d3d11":
                        graphicsMode = (int)GraphicsBackend.Direct3D11;
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

        var platform = new Eto.Wpf.Platform();
        platform.Add<VeldridSurface.IHandler>(() => new WpfVeldridSurfaceHandler());


        int HTCount = Environment.ProcessorCount;

        VarianceContextGUI varianceContext = new(false, xmlFile, numberOfThreads,
            HTCount, backend);
        // run application with our main form
        VarianceApplication va = new(platform, varianceContext);

        if (dark)
        {
            //System.Windows.Application.Current.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary { Source = new Uri("pack://application:,,,/DynamicAero2;component/Theme.xaml", UriKind.RelativeOrAbsolute) });
            //System.Windows.Application.Current.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary { Source = new Uri("pack://application:,,,/DynamicAero2;component/Brushes/Dark.xaml", UriKind.RelativeOrAbsolute) });
        }
        va.Run();
    }
}
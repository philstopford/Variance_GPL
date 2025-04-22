using System;

namespace Variance;

internal static class VHeadless
{
    private static void Main(string[] args)
    {
        bool implantMode = false;
        string xmlFile = "";
        int numberOfThreads = -1;
        bool emailSettingsOK = false;
        string emailAddress = "";
        string emailPwd = "";
        string emailServer = "";
        string emailPort = "";
        bool emailOnCompletion = false;
        bool emailPerJob = false;
        bool emailSSL = false;
        if (args.Length > 0)
        {
            int implantIndex = Array.IndexOf(args, "--implant");
            int oneThreadIndex = Array.IndexOf(args, "--1thread");
            int threadsIndex = Array.IndexOf(args, "--threads");
            int emailIndex = Array.IndexOf(args, "--email");
            int emailPerJobIndex = Array.IndexOf(args, "--emailPerJob");
            int emailOnCompletionIndex = Array.IndexOf(args, "--emailOnCompletion");

            implantMode = implantIndex != -1;
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

            emailPerJob = emailPerJobIndex != -1;
            emailOnCompletion = emailOnCompletionIndex != -1;

            if (emailIndex != -1)
            {
                string emailFile = args[emailIndex + 1];
                // Validate that this is actually a file
                if (System.IO.File.Exists(emailFile))
                {
                    bool pwdOK, serverOK, portOK;
                    bool addressOK = pwdOK = serverOK = portOK = false;
                    // Set our flags to ensure we have a valid email configuration

                    char[] splitArray = new char[] { ' ' };
                    System.IO.StreamReader emailSettings = new(emailFile);
                    while (!emailSettings.EndOfStream)
                    {
                        string line = emailSettings.ReadLine();
                        string[] tokens = line.Split(splitArray);
                        if (line.ToUpper().StartsWith("ADDRESS"))
                        {
                            // email address should be here.
                            emailAddress = tokens[1]; // spaces are illegal in email addresses.
                            addressOK = true;
                        }

                        if (line.ToUpper().StartsWith("PASSWORD"))
                        {
                            emailPwd = tokens[1];
                            for (int token = 2; token < tokens.Length; token++)
                            {
                                // spaces are legal in passwords so we need to merge tokens.
                                emailPwd += " " + tokens[token];
                            }
                            pwdOK = true;
                        }

                        if (line.ToUpper().StartsWith("SERVER"))
                        {
                            emailServer = tokens[1]; // spaces are illegal in server addresses.
                            serverOK = true;
                        }

                        if (line.ToUpper().StartsWith("PORT"))
                        {
                            emailPort = tokens[1]; // spaces are illegal in server addresses.
                            portOK = true;
                        }

                        if (line.ToUpper().StartsWith("SSL"))
                        {
                            emailSSL = true;
                        }
                    }
                    emailSettingsOK = addressOK && pwdOK && portOK && serverOK;
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
        }

        // File validation
        if (!System.IO.File.Exists(xmlFile))
        {
            Error.ErrorReporter.showMessage_OK("Unable to find project file: " + xmlFile, "ERROR");
        }
        else
        {
            // Email validation
            if (!emailSettingsOK)
            {
                emailOnCompletion = false;
                emailPerJob = false;
            }

            int HTCount = Environment.ProcessorCount;
            VarianceContext varianceContext = new(implantMode, xmlFile, numberOfThreads,
                HTCount, "Variance_hl")
            {
                emailAddress = emailAddress,
                emailPwd = emailPwd,
                ssl = emailSSL,
                host = emailServer,
                port = emailPort,
                completion = emailOnCompletion,
                perJob = emailPerJob
            };
            Headless hl = new(varianceContext);

            Console.CancelKeyPress += hl.cancelHandler;
            hl.doStuff();
        }
    }
}
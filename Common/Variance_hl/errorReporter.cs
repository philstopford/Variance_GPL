using System;

namespace Error;

public class ErrorReporter
{
    public static void showMessage_OK(string stringToDisplay, string caption)
    {
        Console.WriteLine(caption + ": " + stringToDisplay);
    }
}
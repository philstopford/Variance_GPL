using System;

namespace Error;

public static class ErrorReporter
{
    public static void showMessage_OK(string stringToDisplay, string caption)
    {
        Console.WriteLine(caption + ": " + stringToDisplay);
    }
}
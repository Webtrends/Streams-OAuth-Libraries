using System;

/// <summary>
/// A sample class to excersize the TokenRequest class
/// </summary>
public class Example
{
    // TODO: Enter your client credentials
    public static readonly string clientId = "..";
    public static readonly string clientSecret = "..";

    /// <summary>
    /// The entry point into the program
    /// </summary>
    /// <param name="args">Not used</param>
    public static void Main(string[] args)
    {
        try
        {
            TokenRequest requestor = new TokenRequest(clientId, clientSecret);
            String token = requestor.Execute();
            Console.WriteLine("Your token is: " + token);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}

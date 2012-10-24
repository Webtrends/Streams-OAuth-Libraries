package com.webtrends.client.authentication;


public class Example 
{
    // TODO: Enter your client credentials
    public static final String clientId = "..";
    public static final String clientSecret = "..";
    
    public static void main(String[] args)
    {
        try {
            TokenRequest tr = new TokenRequest(clientId,clientSecret);
            String token = tr.execute();
            System.out.println("Your token is: " + token);
        } catch (Exception e)
        {
            e.printStackTrace();
        }
    }
}

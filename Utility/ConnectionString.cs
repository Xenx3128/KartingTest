namespace TestMVC.Utility;


public static class ConnectionString  
{  
   private static string cName = "Host=localhost;Username=postgres;Password=postgres;Database=karting;Encoding=UTF8;";  
   public static string CName { get => cName;}  
}  
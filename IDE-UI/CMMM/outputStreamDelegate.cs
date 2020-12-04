using System;


public interface outputStreamDelegate
{
    void Print(String s);
}

public class outputStream
{
    public virtual void Print(string msg)
    {
        System.Console.WriteLine(msg);
    }
}


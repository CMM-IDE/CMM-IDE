using System;


public interface IOutputStream
{
    void Print(String s);
}

public class OutputStream
{
    public virtual void Print(string msg)
    {
        System.Console.WriteLine(msg);
    }
}


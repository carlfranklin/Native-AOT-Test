﻿using System.Diagnostics;

var numbersInSequence = 45;
var executions = 1000;
var stopWatch = new Stopwatch();
var response = string.Empty;

stopWatch.Start();

for (int i = 1; i <= executions; i++)
{
    Fibonacci(0, 1, 1, numbersInSequence);
}

stopWatch.Stop();
Console.WriteLine($"\n\nTotal Time elapsed for {executions} executions: {stopWatch.ElapsedMilliseconds} milliseconds.");

static void Fibonacci(int firstNumber, int secondNumber, int numbersProcessed, int numbersInSequence)
{
    //Console.Write($"{firstNumber}{(numbersProcessed < numbersInSequence ? ", " : string.Empty)}");
    if (numbersProcessed < numbersInSequence)
    {
        Fibonacci(secondNumber, firstNumber + secondNumber, numbersProcessed + 1, numbersInSequence);
    }
}
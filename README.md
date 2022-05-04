# Native AOT in .NET 7

Native AOT (Ahead of Time) is a group of technologies that help you build faster and lighter applications, by generating code at build time rather than at runtime, for .NET desktop client and server scenarios.

Native AOT generates 100% native code at build-time with no dependencies.

Native compiling for .NET has been around since the beginning of .NET.

[NGEN](https://docs.microsoft.com/en-us/dotnet/framework/tools/ngen-exe-native-image-generator) is a Native Image Generator for .NET Framework only.

[ReadyToRun](https://docs.microsoft.com/en-us/dotnet/core/deploying/ready-to-run) is a format used by [crossgen](https://devblogs.microsoft.com/dotnet/conversation-about-crossgen2/), a .NET Core tool that does a combination of compiling to native and IL (Just In Time).

[Mono AOT](https://www.mono-project.com/docs/advanced/aot/) is used by Xamarin. iOS doesn't allow JIT compiling, so something had to be done there. 

If you have been following Blazor, then you know about AOT for WASM, which uses Crossgen under the hood.

### Benefits of AOT

You can build self-contained apps that can be copied from system to system as long as they are the same.

Faster load time is great for container apps, and general purpose apps as well.

Smaller memory footprint because only the code that's required is loaded, not the entire CLR. Even though executable size is higher, if you look at memory being used it's much less.

Performance may or may not be sufficiently improved. It will be better, but how much depends on what you're doing.

### Drawbacks of AOT

No Loading assemblies using Reflection or using `Reflection.Emit` for code generation. 

May require tweaking for supporting dependencies.

Longer build times.

## Prerequisites

In order to take advantage of Native AOT the following prerequisites need to be installed.

### Windows

[.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

[Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/community/)

Enable Desktop development with C++ workload

![Desktop development with C++ workload](images/2ef3f87ed115b974660b662cc68bf46006f2698573ef72d9ae87a2cabd164bb3.png)

## Demo

The following demo is a Console Application that captures the time to generate any given numbers using the Fibonacci sequence with and without Native AOT enabled.

### Create a Console Application called Fibonacci

![Create a new project](images/de393b6128c205b133e28e079da6c74cf711f5c81c6be481e89981b8459c96d9.png)

![Configure your new project](images/0a2fac4cbfbcff37a5a092fb76475d43aef33da134a19f82bbcd5a19979861a1.png)

![Target .NET 7 (Preview)](images/57b48250251db866f4800b1cae4568280f5803e68fa53b48353f6dcee446a329.png)

Replace *Program.cs* with the following:

```csharp
using System.Diagnostics;

var numbersInSequence = 45;
var executions = 1;
var stopWatch = new Stopwatch();
var response = string.Empty;

stopWatch.Start();

for (int i = 1; i <= executions; i++)
{
    Fibonacci(0, 1, 1, numbersInSequence);
}

stopWatch.Stop();
Console.WriteLine($"\n\nTotal Time elapsed for {executions} executions: " +
    $"{stopWatch.ElapsedMilliseconds} milliseconds.");

static void Fibonacci(int firstNumber, int secondNumber, int numbersProcessed, 
    int numbersInSequence)
{
    Console.Write($"{firstNumber}{(numbersProcessed < numbersInSequence ? ", " : string.Empty)}");
    if (numbersProcessed < numbersInSequence)
    {
        Fibonacci(secondNumber, firstNumber + secondNumber, 
            numbersProcessed + 1, numbersInSequence);
    }
}
```

Fibonacci numbers are defined at [WikiPedia](https://en.wikipedia.org/wiki/Fibonacci_number):

> In mathematics, the **Fibonacci numbers**, commonly denoted *Fn*, form a [sequence](https://en.wikipedia.org/wiki/Integer_sequence), the **Fibonacci sequence**, in which each number is the sum of the two preceding ones.

The `Fibonacci` function fulfills this. You pass it the first number and the second number, the numbers processed, and the numbers in the sequence, and it calls itself after calculating the next number in the sequence. 

I've put the following line in the function to print out the number:

```c#
Console.Write($"{firstNumber}{(numbersProcessed < numbersInSequence ? ", " : string.Empty)}");
```

The `StopWatch` object measures the time it takes, which is displayed after the sequence is generated and runs.

Run the app and you should see something like this:

![image-20220504003945954](images/image-20220504003945954.png)

Outputting the series into the console adds an overhead because screen drawing.  Let's comment out the following line in the `Fibonacci` method:

```csharp
Console.Write($"{firstNumber}{(numbersProcessed < numbersInSequence ? ", " : string.Empty)}");
```

Running the app again shows that it took almost no time:

![image-20220504004159296](images/image-20220504004159296.png)

So, let's bump up the `numbersInSequence` and `executions` variables to 10,000:

```c#
var numbersInSequence = 10000;
var executions = 10000;
```

and run it again:

![image-20220504004510850](images/image-20220504004510850.png)

Now we have something we can compare.

Create a new Console Application for testing Native AOT called `FibonacciAOT` and copy the code from the first application's *Program.cs* to the new application

#### Enable Native AOT

In order to enable Native AOT we need to add a reference to the `ILCompiler` NuGet package, in order to use the Native AOT Compiler and runtime. Since is still in Preview, we first need to add a new NuGet Package source.

##### Add NuGet Package Source

.NET 7 NuGet Package Source

```http
https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet7/nuget/v3/index.json
```

![.NET 7 NuGet Package Source](images/c2ecde9f6267d5816d26c97ddce974eb4dce44137c8018725b486f1ff5f8da20.png)

Add a reference to the `ILCompiler` NuGet package.

> [!TIP]
> In the Manage NuGet Packages screen, make sure All or .NET 7 Preview under Package Source is selected, as well as the Include Prerelease checkbox is checked.

![.NET 7 Preview ](images/d6831415186e7c0221fbf8ed694e731a771d9adde095e13b2c8c2afda83ff021.png)

![ILCompiler NuGet package](images/e561dd6590c2bea58ce8b8b7a31579a96e662d840073f7e5357f439fc2e9567f.png)

Create a Publishing Profile

![Create a Publishing Profile](images/86b6582ae8761efe40ce8b1e18cffb841434a9f1e5e92c71d283447f0ef5ddc8.png)

![Target](images/ad1c7671bc2ed35438d236958be16ac00d0583eca805640e0607477553f6600e.png)

![Specific Target](images/d7ca1d2960be8e331a7e2999ecbdd3ec1d316ef18c4bfe4c35aef8f85b73bb8e.png)

![Location](images/5d19344eb807a3af6d8387023871200e43c40b1af49bf3d5ad2432337be918ab.png)

![Creation Progress](images/afdb9162f812bb75ef4a060ecfc61b2528e04dc97dc941128d266e255d5ff00b.png)

![Publishing Profile](images/df0cfa0e60db05b3dc35df0ffacf23d235790996d6d5a262aa2a9c0d90c73f60.png)

Click on Show all settings and change the Target runtime to your desired runtime, in this case I changed it to win-x64

![Show all settings](images/83d54865100a3e1f60423f83e7af9ce1691ec57cfbedc60bb16f85c4c453c371.png)

> [!TIP]
> If you expand the File publish options, ReadyToRun can be set there. We're not going to set this, but I just wanted to show you where it is.

![ReadyToRun](images/ea162118b22d8c48693c48875dc0e5fc71010e9664a6151443037bcd90ef7ac9.png)

Click Save to close Profile settings and Publish to publish the application.

![Save and Publish](images/a7ec10a56ebe3de6d851199465a1213c607ecabdcfb4db383f4816a911d9972e.png)

> [!IMPORTANT]
> When building with Native AOT You want to see the ILCompiler is being used,

![ILCompiler](images/447944a0325d7cd486f681585bfcae1a672f11c2424d7d7e935d2d41dd41012a.png)

as opposed of the Roslyn compiler.

![Roslyn](images/930779076c1ad8d9c8cb885da4172c00e4fd6a72c8e2dab1d7a11509bafa0494.png)

Once published, run the *FibonacciAOT.exe* file from the output folder.

### Results


###### Memory Utilization

![Memory Utilization](images/7ebe3f7a65f0314b1d8a9822edf9d646c861f3bca77774ab808efc6ebed22e2e.png)

###### Disk Utilization

- Without Native AOT

  ![Disk Utilization Without Native AOT](images/4a7ed5237c54283c2ad34a32a3c9073801953030b3af285e2afb4c4ecde9dc92.png)

- With Native AOT

  ![Disk Utilization With Native AOT](images/3ef9f3acfd56732dc4c768c73dfc048f2fffc164248ab5ef13a19f4796883df3.png)

##### Other Findings

After doing some digging around, I found some useful information.

> [!IMPORTANT]
> The reason why the Desktop development with C++ workload is a prerequisite, is because of the Link process,

![C++ workload](images/24a07953e2ee52e8533c1ecf316623a610f20c0dc1d9a14b916535328ec6f7ce.png)

which come with the C++ Build Tools

![C++ Build Tools](images/046187d6a983ad3b7c9134968fea0ae62cb6a6f3dfabce6ac89307955f9118f9.png)

> [!IMPORTANT]
> The location of the Native files used for building the application can be found in the obj folder.

![Native files in obj](images/5877c9e83f700fb2909f77f0496f97228ae374c1575a2a529abed1913892ddd7-165162259613451.png)

> [!IMPORTANT]
> The location of the Native files once built can be found in the bin folder.

![Native files in bin](images/5877c9e83f700fb2909f77f0496f97228ae374c1575a2a529abed1913892ddd7-165162259613451.png)

> [!NOTE]
> Building the application in Linux or macOS would generate Native files for each platform.

### Comparing Startup and Run Times

We can compare startup times using a `System.Diagnostics.Process` and timing it with a `StopWatch` object.

Add a new Console Application to the solution called `FibTimer` and replace the *Program.cs* with this:

```c#
using System.Diagnostics;

Console.WriteLine("Application Startup Timer. Enter path to .exe filename or drag it into the app");
string inputFileName = Console.ReadLine();
if (string.IsNullOrEmpty(inputFileName)) return;
// remove quotes
inputFileName = inputFileName.Replace("/q", "");

var process = new Process();
process.StartInfo.FileName = inputFileName;
var stopWatch = new Stopwatch();

stopWatch.Start();
process.Start();
stopWatch.Stop();

Console.WriteLine($"Startup for IL: {stopWatch.ElapsedMilliseconds}");
```

Set this app as the startup project, and run it. 

From the FIle Explorer, drop one of the .exe files onto it and press ENTER. 

You will see how long it took to load. The app will also run, so you get the benefit of testing the app itself at the same time.

Here are my results:

```
Fibonacci.exe:     Load Time: 19ms   Time to execute: 282ms
FibonacciAOT.exe:  Load Time: 12ms   Time to execute: 134ms
```

## Resources

|Resource Title                 |Url                                                     |
|-------------------------------|--------------------------------------------------------|
|Download .NET 7.0              |<https://dotnet.microsoft.com/en-us/download/dotnet/7.0>|
|Visual Studio 2022 Preview     |<https://visualstudio.microsoft.com/vs/community/>|
|Compiling with Native AOT      |<https://github.com/dotnet/runtime/blob/main/src/coreclr/nativeaot/docs/compiling.md>|
|Native AOT Pre-requisites      |<https://github.com/dotnet/runtime/blob/main/src/coreclr/nativeaot/docs/prerequisites.md>|
|ReadyToRun                     |<https://docs.microsoft.com/en-us/dotnet/core/deploying/ready-to-run>|
|Mono AOT                       |<https://www.mono-project.com/docs/advanced/aot/>|
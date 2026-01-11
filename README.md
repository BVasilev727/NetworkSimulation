  How to download and run:
1. Prerequisites
  .NET 8.0 SDK (Required for the net8.0 target framework seen in your build path).

  Visual Studio 2022 or VS Code.

  ScottPlot Library: This is used for generating the waveform plots.

2. Installation
  Clone the repository to your local machine.

  Restore NuGet Packages:

  Bash

    dotnet restore
  This will automatically download ScottPlot and any other necessary dependencies.

3. Running the Simulation
  Open the Solution: Open NetworkSimulation.sln in Visual Studio.

  Set Startup Project: Right-click on NetworkSimulationExecute and select Set as Startup Project.

  Run: Press F5 or click the Start button.

4. Viewing Results
  After the simulation completes, the program automatically generates three types of output in a folder named NetworkSimulation on your Desktop:

      NetworkGraph.svg: A visual map of the entire tower grid, the sender/recipient locations, and the specific path the message took.

      waveform.csv: Raw time-domain data including time and amplitude values.

      plot.png: A high-resolution image of the modulated signal waveform.

Note: If you encounter a System.UnauthorizedAccessException, ensure that waveform.csv is not currently open in Excel or another text editor.

# Release Notes, Rev. 3.0.0

# Content

Revision 3.0.0 of the ECP library and ECP tester is a major release that introduce breaking changes with the previous revision 2.6.2 of the library. The breaking change come from the removal of the internal logging system and the introduction of the [Serilog](https://serilog.net/) logging system.


### Major changes

The release contains the following major changes:

* Added the Serilog library, which results in the removal of the EcpLog class.
* Added "actions" in the device definitition files
* Added "analysis" in the device definition files

## Changes

### Serilog Library

The internal logging system has been removed and replaced with the Serilog logging system. With Serilog a modern logging system for structured event data becomes available. 

### Actions

Actions has been added to the device definition file. Actions are python scripts that can be executed from the Action menu of the ECP Tester. To enables this the IronPython interpreter has been added to the ECP Tester, which provices a full .NET implementation of python and its standard library, with the exception of 3rd party components in the python standard library.

To define actions, an ```<actions>``` is defined in the device definition file.

```
<actions>
    <action name="Negate Number"
            script="negate.py" />
</actions>
```

In the example above a single action which will be available in the Actions menu as a submenu item "Negate Number". When this menu item is clicked the python script negate.py will be executed.

The python script will have to global variables in its scope:

1. dev: the device that is currently loaded, which can be used to execute functions and send messages to.
2. dialog: a class that provide helper functions for obtaining user input. Currently, GetString(title, initial_value), GetNumber(title, initial_valyes), and GetListIndex(title, items).

Below is an example of the negate.py script from the action above

```python
import clr

clr.AddReference("Serilog")
clr.AddReference("Inventors.ECP.DefaultDevice")

from Serilog import Log
from Inventors.ECP.DefaultDevice.Functions import *


number = dialog.GetNumber("Number to negate", 10);


Log.Information("I will try to negate: {number}".format(number = number))

func = SimpleFunction()
func.Operand = number

dev.Execute(func)

Log.Information("I got the following answer: {number}".format(number = func.Answer))

index = dialog.GetListIndex("Select value", "1;2;3");

Log.Information("Index was: {index}".format(index = index))
```


For a full example of how to write a ddfx file containing actions, please see the device-serial.ddfx file in the source code of the library.

### Analysis

Analysis of incomming messages from a device has been added to the ECP tester. Analysis is performed by a python script that is executed for each message that is received. It is the responsibility of this script to add data to the current data set, and to update the plot that displays the results.

Below is there an example of how analyses can be defined in the ddfx file:

```
<analysis-specification>
        <analysis name="Signal Analysis"
                  code="128"
                  script="analysis.py" 
                  signals="1"/>
        <analysis name="Normalized Signal Analysis"
                  code="128"
                  script="analysis2.py"
                  signals="1"/>
    </analysis-specification>
```

The name is the name of the analysis that will be displayed in the analysis viewer (accessed in the Analysis menu), code is the code of the message to be analysed, and script is the python script that will analyse it, signals is the number of data points that will be extracted from each message and saved in the data set.

Below is an example of a python script (analysis2.py) for analysing a message:

```python
import clr
clr.AddReference("ScottPlot")

from ScottPlot import MarkerShape

signal = msg.X / 255.0
data.Update(0, signal)

y = data.Signal(0) 
x = data.GetX(0.0, 1.0)

if len(x) > 0:
    scatter = plt.AddScatter(x, y)
    scatter.LineWidth = 0
    scatter.MarkerSize = 10
    scatter.MarkerShape = MarkerShape.cross

    plt.Title("Normalized Signal Analysis")
    plt.XLabel("Samples []")
    plt.YLabel("Signal [counts]")
```

The message to be analysed is available in the global variable ```msg```, the data set in ```data```, and the plot in ```plt```. The ECP tester use the ScottPlot library for plotting. Please refer to the ScottPlot [cookbook](https://scottplot.net/cookbook/4.1/) for a description of how to perform plotting with the library.

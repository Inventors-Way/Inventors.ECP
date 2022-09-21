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
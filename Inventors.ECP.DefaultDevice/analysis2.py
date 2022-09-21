signal = msg.X
data.Update(0, signal)

y = data.Signal(0) / 255.0
x = data.GetX(0.0, 1.0)

if len(x) > 0:
    plt.AddScatter(x, y)
    plt.Title("Normalized Signal Analysis")
    plt.XLabel("Samples []")
    plt.YLabel("Signal [counts]")
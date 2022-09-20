def initialize():
	data.SetNumberOfSignals(1)

def analyse(msg):
	time = msg.AverageTime
	data.Update(0, time)

	y = data.Signal(0)
	x = data.GetX(0.0, 1.0)

	plt.AddScatter(x, y)
	plt.Title("Run times")
	plt.XLabel("Samples []");
	plt.YLabel("Average Time [us]")
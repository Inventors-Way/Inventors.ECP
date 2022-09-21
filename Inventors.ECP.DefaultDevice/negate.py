import clr

clr.AddReference("Serilog")
clr.AddReference("Inventors.ECP.DefaultDevice")

from Serilog import Log
from Inventors.ECP.DefaultDevice.Functions import SimpleFunction

number = 12;



Log.Information("I will try to negate: {number}".format(number = number))

func = SimpleFunction()
func.Operand = number;

dev.Execute(func)

Log.Information("The answer I got: {answer}".format(answer = func.Answer))
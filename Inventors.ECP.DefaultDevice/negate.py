import clr

clr.AddReference("Serilog")
clr.AddReference("Inventors.ECP.DefaultDevice")

from Serilog import Log
from Inventors.ECP.DefaultDevice.Functions import SimpleFunction


number = dialog.GetNumber("Number to negate", 10);


Log.Information("I will try to negate: {number}".format(number = number))

func = SimpleFunction()
func.Operand = number

dev.Execute(func)

Log.Information("I got the following answer: {number}".format(number = func.Answer))


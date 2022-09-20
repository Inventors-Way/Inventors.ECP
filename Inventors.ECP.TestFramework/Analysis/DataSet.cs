using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.TestFramework.Analysis
{
    public class DataSet
    {
        private readonly List<List<double>> _values = new();
        private int noOfSignals = 1;
        private List<double> _current;

        public void Reset()
        {
            _values.Clear();
        }

        public void SetNumberOfSignals(int noOfSignals)
        {
            this.noOfSignals = noOfSignals;

        }

        public void AddRow()
        {
            _current = new List<double>();

            for (int i = 0; i < noOfSignals; i++)
                _current.Add(0);

            _values.Add(_current);
        }

        public void Update(int signal, double value)
        {
            if (signal >= noOfSignals)
                return;

            if (_current is null)
                return;

            _current[signal] = value;   
        }

        public double[] Signal(int signal)
        {
            if (signal >= noOfSignals)
                throw new ArgumentException($"Signal [ {signal} ] does not exists, larget signal that does exists is [ {noOfSignals} ]");

            return (from row in _values
                    select row[signal]).ToArray();
        }

        public double[] GetX(double x0, double dx)
        {
            double[] retValue = new double[_values.Count];
            double x = x0;

            for (int n = 0; n < _values.Count; n++)
            {
                retValue[n] = x;
                x += dx;
            }

            return retValue;
        }
    }
}

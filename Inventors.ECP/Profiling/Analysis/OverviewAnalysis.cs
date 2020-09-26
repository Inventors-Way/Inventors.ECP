using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling.Analysis
{
    public class OverviewAnalysis
    {
        private List<string> _labels;
        private List<double> _x;
        private List<double> _average;
        private List<double> _maximum;
        private List<double> _minimum;
        private List<double> _scaleMaximum;

        public OverviewAnalysis(List<double> x, 
                                List<double> average, 
                                List<double> maximum, 
                                List<double> minimum, 
                                List<double> scaleMaximum,
                                List<string> labels)
        {
            _labels = labels;
            _x = x;
            _average = average;
            _minimum = minimum;
            _maximum = maximum;
            _scaleMaximum = scaleMaximum;

        }

        public IList<string> Labels => _labels.AsReadOnly();

        public IList<double> X => _x.AsReadOnly();

        public IList<double> Average => _average.AsReadOnly();

        public IList<double> Maximum => _maximum.AsReadOnly();

        public IList<double> Minimum => _minimum.AsReadOnly();

        public IList<double> ScaleMaximum => _scaleMaximum;
    }
}

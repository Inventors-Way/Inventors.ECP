using Inventors.ECP.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventors.ECP.Profiling.Analysis
{
    public class TaskAnalysis
    {
        private readonly string _id;
        private List<double> _average;
        private List<double> _maximum;
        private List<double> _minimum;
        private List<double> _time;
        private List<TimingViolation> _violations;
        private List<TargetEvent> _events;

        public TaskAnalysis(string id, 
                            List<double> average,
                            List<double> maximum,
                            List<double> minimum,
                            List<double> time,
                            List<TimingViolation> violations,
                            List<TargetEvent> events)
        {
            _id = id;
            _average = average;
            _maximum = maximum;
            _minimum = minimum;
            _time = time;
            _violations = violations;
            _events = events;
        }

        public string ID => _id;

        public IList<double> Average => _average?.AsReadOnly();

        public IList<double> Maximum => _maximum?.AsReadOnly();

        public IList<double> Minimum => _minimum?.AsReadOnly();

        public IList<double> Time => _time?.AsReadOnly();

        public IList<TimingViolation> Violations => _violations?.AsReadOnly();

        public IList<TargetEvent> Events => _events?.AsReadOnly();

    }
}

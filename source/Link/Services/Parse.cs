using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link
{
    public class Parse
    {
        public static TimeSpan Time(string entry)
        {
            entry = entry.ToLower();
            // Assume order is 1m30s
            TimeSpan _time = new TimeSpan();

            // Seconds
            if (entry.Contains('s'))
            {
                var _seconds = Convert.ToDouble(entry.Split('s')[0]);

                _time = _time.Add(TimeSpan.FromSeconds(_seconds));
            }

            // Minutes
            if (entry.Contains('m'))
            {
                var _minutes = Convert.ToDouble(entry.Split('m')[0]);

                _time = _time.Add(TimeSpan.FromMinutes(_minutes));
            }

            // Hours
            if (entry.Contains('h'))
            {
                var _hours = Convert.ToDouble(entry.Split('h')[0]);

                _time = _time.Add(TimeSpan.FromHours(_hours));
            }

            return _time;
        }
    }
}

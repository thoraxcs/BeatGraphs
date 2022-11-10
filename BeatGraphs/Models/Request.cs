using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// POTENTIAL FUTURE SOLUTION TO 30 SECOND PAUSE BETWEEN PAGE READS.

namespace BeatGraphs.Models
{
    class Requests
    {
        private static Queue<Request> requests;

        public Requests()
        {
            requests = new Queue<Request>();
        }

        public void Add(string url, string callback, bool errorThrow = false)
        {
            DateTime schedule = DateTime.Now;
            if (requests.Count != 0)
                schedule = requests.ElementAt(requests.Count - 1).time.AddMinutes(1);
            requests.Enqueue(new Request(schedule, url, callback, errorThrow));
        }

        public int Count()
        {
            return requests.Count;
        }

        public Request Pop()
        {
            return requests.Dequeue();
        }

        internal class Request
        {
            internal DateTime time;
            internal string url;
            internal string callback;
            internal bool errorThrow;

            internal Request(DateTime t, string u, string c, bool e)
            {
                time = t;
                url = u;
                callback = c;
                errorThrow = e;
            }
        }
    }
}

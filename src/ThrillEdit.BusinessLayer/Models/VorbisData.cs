using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrillEdit.BusinessLayer.Models
{
	public class VorbisData
    {
        private long _startPos;

        public long StartPos
        {
            get { return _startPos; }
            set { _startPos = value; }
        }
        private long _endPos;

        public long EndPos
        {
            get { return _endPos; }
            set { _endPos = value; }
        }
        private string _origin;

        public string Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        private TimeSpan _duration = new TimeSpan();

        public TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        private int _rate;

        public int Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        public long Size
        {
            get { return (_endPos - _startPos) + 1; }
        }
    }
}

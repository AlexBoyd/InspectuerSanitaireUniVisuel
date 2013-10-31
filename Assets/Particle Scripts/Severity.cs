using System;

namespace AssemblyCSharp
{
	public class Severity
	{
		 private int severity;
        private string name;

        public Severity(string str, int r)
        {
            name = str;
            severity = r;
        }

        public void setSeverity(int r)
        {
            severity = r;
        }

        public void setName(string str)
        {
            name = str;
        }

        public string getName()
        {
            return name;
        }

        public int getSeverity()
        {
            return severity;
        }
	}
}


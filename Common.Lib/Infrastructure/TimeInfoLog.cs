namespace Common.Lib.Infrastructure
{
    public class TimeInfoLog
    {
        TimeEntry? LastEntry { get; set; }
        public class TimeEntry
        {
            public TimeEntry(DateTime entryTime, string actionName, TimeEntry prev)
            {
                EntryTime = entryTime;
                ActionName = actionName;
                if(prev != null)                
                    DifMs = (EntryTime - prev.EntryTime).TotalMilliseconds;
            }

            public TimeEntry()
            {
                    
            }

            internal DateTime EntryTime { get; set; }
            public string ActionName { get; set; } = string.Empty;

            public double DifMs { get; set; }
        }

        public TimeInfoLog()
        {
            
        }

        public TimeInfoLog(DateTime dt)
        {
            Start = dt;
        }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public List<TimeEntry> TimeEntries { get; set; } = [];

        public void AddTimeEntry(string actionName)
        {
            var newTimeEntry = new TimeEntry(DateTime.Now, actionName, LastEntry);
            LastEntry = newTimeEntry;
            TimeEntries.Add(newTimeEntry);
        }
    }
}

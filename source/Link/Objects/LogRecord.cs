using System;

namespace Link
{
    public class LogRecord
    {
        [LiteDB.BsonId]
        public DateTime Date { get; set; }
        public LogType Type { get; set; }
        public string Log { get; set; }
    }
}
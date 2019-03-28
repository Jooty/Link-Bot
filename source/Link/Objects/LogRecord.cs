using System;
using LiteDB;

namespace Link
{
    public class LogRecord
    {
        [BsonId]    public DateTime Date { get; set; }
        [BsonField] public LogType  Type { get; set; }
        [BsonField] public string   Log  { get; set; }
    }
}
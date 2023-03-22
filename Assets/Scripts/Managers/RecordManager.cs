using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    public static RecordManager Singleton;
    public struct RecordData
    {
        public int attempts;
        public int moves;
        public int minutes;
        public int seconds;
        public static bool operator <(RecordData c1, RecordData c2)
        {
            return c1.moves > c2.moves || c1.minutes > c2.minutes || c1.seconds > c2.seconds;
        }
        public static bool operator >(RecordData c1, RecordData c2)
        {
            return c1.moves < c2.moves || c1.minutes < c2.minutes || c1.seconds < c2.seconds;
        }
        public override string ToString()
        {
            return $"Best\n {moves} moves\n{minutes:00}m:{seconds:00}s";
        }
    }

    void Awake()
    {
        if (Singleton == null) Singleton = this;
        else return;
    }

    private Dictionary<TextAsset, RecordData> _records = new Dictionary<TextAsset, RecordData>();
    
    public void addRecord(TextAsset level, RecordData record)
    {
        if (_records.Keys.Contains(level))
        {
            if(record > _records[level])
            {
                _records[level] = record;
            }
        }
        else
        {
            _records[level] = record;
        }
    }

    public string getRecordString(TextAsset level)
    {
        if (_records.Keys.Contains(level))
        {
            return _records[level].ToString();
        }
        return "not attempted";
    }

    public int getAttempts(TextAsset level)
    {
        if (_records.Keys.Contains(level))
        {
            return _records[level].attempts;
        }
        return 0;
    }
}

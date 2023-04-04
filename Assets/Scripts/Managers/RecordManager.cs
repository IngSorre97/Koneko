using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.SocialPlatforms.Impl;

public class RecordManager : MonoBehaviour
{
    public static RecordManager Singleton;

    private Dictionary<int, List<string>> pageSwitch = new Dictionary<int, List<string>>()
    {
        { 0, new List<string>() },
        { 1, new List<string>() }
    };

    private Dictionary<string, string> leaderboardKeys = new Dictionary<string, string>()
    {
        { "0", "12831" },
        { "1", "12834" },
        { "2", "12835" },
        { "3", "12836" },
        { "4", "12837" },
        { "5", "12838" },
        { "6", "12839" },
        { "7", "12840" },
        { "8", "12841" },
        { "9", "12842" },
        { "10", "12843" },
    };

    public struct RecordData
    {
        public string nickname;
        public int attempts;
        public int moves;
        public int minutes;
        public int seconds;
    }

    private Dictionary<string, int> records = new Dictionary<string, int>();
    private string _nickname;

    [HideInInspector] public bool isDirty = true;

    void Awake()
    {
        if (Singleton == null) Singleton = this;
        else return;
    }

    private void Start()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            Debug.Log("successfully started LootLocker session");
        });
    }

    public void AddRecord(string id, RecordData record)
    {
        isDirty = true;
        LootLockerSDKManager.SubmitScore(_nickname, record.moves, leaderboardKeys[id], (response) =>
        {
            if (response.statusCode == 200)
                Debug.Log("Successfully submitted the record");
            else
                Debug.Log("Error during request: " + response.Error);
        });
        SetRecords();
    }

    public void SetRecords(string nickname)
    {
        _nickname = nickname;
        SetRecords();
    }

    public void SetRecords()
    {
        Debug.Log("Nickname is " + _nickname);
        int remaining = leaderboardKeys.Keys.Count;
        foreach (string level in leaderboardKeys.Keys)
        {
            LootLockerSDKManager.GetMemberRank(leaderboardKeys[level], _nickname, (response) =>
            {
                if (response.statusCode == 200 && response.score != 0)
                {
                    if (records.ContainsKey(level))
                        records[level] = Mathf.Min(response.score, records[level]);
                    else 
                        records.Add( level, response.score);
                }

                remaining--;
                if (remaining == 0) FinishRecords();
            });
        }
    }

    private void FinishRecords()
    {
        Debug.Log("Finished setting records");
        isDirty = false;
    }

    public int GetRecordByLevel(string level)
    {
        if (records.ContainsKey(level))
            return records[level] - '0';
        else return -1;
    }
}

using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ReportManager : MonoBehaviour
{
    private string logFilePath;
    private StreamWriter logWriter;

   // [SerializeField] Button saveLogButton; // Button to trigger the save

    private void Start()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "GameLogs.txt");
        
        // Set up log writer
        logWriter = new StreamWriter(logFilePath, true);
        Application.logMessageReceived += CaptureLog;
        
        //saveLogButton.onClick.AddListener(SaveLogsToFile);
    }

    private void CaptureLog(string logString, string stackTrace, LogType type)
    {
        logWriter.WriteLine($"{DateTime.Now:G} [{type}] {logString}");
        if (type == LogType.Exception)
        {
            logWriter.WriteLine(stackTrace); // Add stack trace for exceptions
        }
    }

    public void SaveLogsToFile()
    {
        logWriter.Flush();
        Debug.Log("Logs saved to " + logFilePath);
    }

    private void OnApplicationQuit()
    {
        // Clean up
       // Application.logMessageReceived -= CaptureLog;
        logWriter.Close();
    }
}

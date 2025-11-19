using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    // Singleton instance
    public static CSVWriter Instance { get; private set; }

    private string filePath;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance
            return;
        }
        Instance = this;

        // Optional: persist across scenes
        DontDestroyOnLoad(gameObject);

        // Initialize CSV file
        filePath = Path.Combine(Application.persistentDataPath, "GrocerRizz.csv");

        List<string[]> rows = new List<string[]>
        {
            new string[] { "Algorithm", "NPC", "Item", "Quantity", "Listed or Impulsive", "Item Value", "Base Value", "Position Multiplier", "Promotion Multiplier", "Visibility", "Layout", "Number of Posters", "Promotional Score" }
        };

        WriteCSV(rows);
    }

    // Overwrites file and writes header + rows
    public void WriteCSV(List<string[]> rows)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var row in rows)
            {
                writer.WriteLine(string.Join(",", row));
            }
        }

        Debug.Log("CSV saved to: " + filePath);
    }

    // Appends a single row at the end of the CSV
    public void AppendRow(params object[] values)
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(string.Join(",", values));
        }
    }

    public string GetPath() => filePath;
}

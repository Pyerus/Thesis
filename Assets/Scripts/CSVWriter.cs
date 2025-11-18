using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "GrocerRizz.csv");
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

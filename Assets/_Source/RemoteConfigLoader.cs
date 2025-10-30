using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteConfigLoader : MonoBehaviour
{
    [Header("Remote Config")]
    [SerializeField] private string url;
    [SerializeField] private float timeout = 10f;

    private string localPath => Path.Combine(Application.persistentDataPath, "weapon_config.csv");
    public List<WeaponData> weapons = new();

    private IEnumerator Start()
    {
        yield return StartCoroutine(TryLoadRemote());

        if (weapons == null || weapons.Count == 0)
        {
            if (TryLoadLocal())
            {
                Debug.Log("Loaded local config copy.");
            }
            else
            {
                LoadDefault();
                Debug.LogError("The config could not be downloaded either from the network or locally. Default values are used.");
            }
        }
        foreach (var w in weapons)
        {
            Debug.Log($"Weapon {w.id}: damage={w.damage}, cooldown={w.cooldown}");
        }
    }
    private IEnumerator TryLoadRemote()
    {
        using var www = UnityWebRequest.Get(url);
        www.timeout = (int)timeout;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Failed loading config from server: " + www.error);
            yield break;
        }

        var csv = www.downloadHandler.text;
        if (csv == null)
        {
            csv = "";
        }

        var parsed = ParseCSV(csv);
        if (Validate(parsed))
        {
            weapons = parsed;
            try
            {
                File.WriteAllText(localPath, csv);
                Debug.Log("Config successfully loaded from the net and saved locally.");
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed saving locally: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Validation failed. Using fallback.");
        }
    }
    private bool TryLoadLocal()
    {
        if (!File.Exists(localPath))
        {
            return false;
        }
        try
        {
            var csv = File.ReadAllText(localPath);
            var parsed = ParseCSV(csv);
            if (Validate(parsed))
            {
                weapons = parsed;
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading local file: {e.Message}");
        }
        return false;
    }

    private void LoadDefault()
    {
        weapons = new List<WeaponData>
        {
            new() { id = "defaultID", damage = 10f, cooldown = 1f }
        };
    }
    private List<WeaponData> ParseCSV(string csv)
    {
        var result = new List<WeaponData>();

        if (string.IsNullOrEmpty(csv))
        {
            return result;
        }

        csv = csv.TrimStart('\uFEFF');
        using var reader = new StringReader(csv);
        string line;
        var lineIndex = 1;

        while ((line = reader.ReadLine()) != null)
        {
            lineIndex++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            line = line.Replace("\r", "").Trim();
            var cols = line.Split(',');
            if (cols.Length < 3)
            {
                Debug.LogWarning($"Line {lineIndex} less than 3 rows: '{line}'");
                continue;
            }

            var id = cols[0].Trim();
            var dmgStr = cols[1].Trim();
            var cdStr = cols[2].Trim();

            if (!float.TryParse(dmgStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var dmg))
            {
                Debug.LogWarning($"Error parsing damage in line {lineIndex}: '{dmgStr}'");
                continue;
            }
            if (!float.TryParse(cdStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var cd))
            {
                Debug.LogWarning($"Error parsing cooldown in line {lineIndex}: '{cdStr}'");
                continue;
            }
            result.Add(new WeaponData { id = id, damage = dmg, cooldown = cd });
        }
        return result;
    }
    private bool Validate(List<WeaponData> list)
    {
        if (list == null || list.Count == 0)
        {
            return false;
        }
        foreach (var w in list)
        {
            if (w.damage < 0 || w.cooldown <= 0)
            {
                Debug.LogWarning($"Validation failed for weapon {w.id}: damage={w.damage}, cooldown={w.cooldown}");
                return false;
            }
        }
        return true;
    }
}
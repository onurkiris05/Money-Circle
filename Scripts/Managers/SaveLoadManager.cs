using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public void StartSaveLoadSequence()
    {
        if (PlayerPrefs.GetInt("NewGame") <= 0)
        {
            PlayerPrefs.SetInt("NewGame", 1);
        }
        else
        {
            LoadWheels();
        }

        SaveWheels();
    }

    private void LoadWheels()
    {
        int loadCount = PlayerPrefs.GetInt("SavedCount");

        for (int i = 0; i < loadCount; i++)
        {
            int level = PlayerPrefs.GetInt($"{i}_wheel_level");

            Vector3 pos = new Vector3();
            pos.x = PlayerPrefs.GetFloat($"{i}_wheel_pos_x");
            pos.y = PlayerPrefs.GetFloat($"{i}_wheel_pos_y");

            WheelManager.Instance.AddWheel(level, pos);
        }
    }

    private void SaveWheels()
    {
        StartCoroutine(ProcessSaveWheels());
    }

    IEnumerator ProcessSaveWheels()
    {
        while (true)
        {
            int saveCount = WheelManager.Instance.Wheels.Count;

            if (saveCount > 0)
            {
                PlayerPrefs.SetInt("SavedCount", saveCount);

                for (int i = 0; i < saveCount; i++)
                {
                    PlayerPrefs.SetInt($"{i}_wheel_level", WheelManager.Instance.Wheels[i].Level);
                    PlayerPrefs.SetFloat($"{i}_wheel_pos_x", WheelManager.Instance.Wheels[i].transform.position.x);
                    PlayerPrefs.SetFloat($"{i}_wheel_pos_y", WheelManager.Instance.Wheels[i].transform.position.y);
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}
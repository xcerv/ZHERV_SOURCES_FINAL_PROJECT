using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField
{
    public float snd_eff_vol = 1.0f;
}

public class game_settings : MonoBehaviour
{
    public InputField inputField = new InputField(); 

    void Start()
    {
        LoadData();
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("SoundEffectVolume", inputField.snd_eff_vol);
    }

    public void LoadData()
    {
        inputField.snd_eff_vol = PlayerPrefs.GetFloat("SoundEffectVolume");
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }
}

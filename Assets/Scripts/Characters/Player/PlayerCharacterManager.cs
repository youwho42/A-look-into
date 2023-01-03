using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterManager : MonoBehaviour
{
    public List<string> baseCharacters = new List<string>();
    public List<string> aquiredCharacters = new List<string>();
    

    public void Start()
    {
        for (int i = 0; i < baseCharacters.Count; i++)
        {
            AddCharacter(baseCharacters[i]);
        }
    }
    public void AddCharacter(string characterName)
    {
        if(!aquiredCharacters.Contains(characterName))
            aquiredCharacters.Add(characterName);
    }

    
}

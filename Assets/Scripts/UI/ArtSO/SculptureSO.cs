using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Sculpture", menuName = "Klaxon/Sculpture")]
public class SculptureSO : ScriptableObject
{
    public LocalizedString localizedName;
    public LocalizedString localizedDescription;
}

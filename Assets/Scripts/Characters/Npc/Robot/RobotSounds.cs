using UnityEngine;

public class RobotSounds : MonoBehaviour
{
    WorldObjectAudioManager audioManager;

    private void Start()
    {
        audioManager = GetComponent<WorldObjectAudioManager>();
    }
    public void PlayLandedSound()
    {
        audioManager.PlaySound("Landed");
    }
    public void PlayDeactivateSound()
    {
        audioManager.PlaySound("DeactivateSwoosh");
    }
    public void PlayOpenHeadSound()
    {
        audioManager.PlaySound("OpenHeadAir");
    }
    public void PlayCloseHeadSound()
    {
        audioManager.PlaySound("CloseHeadAir");
    }
}

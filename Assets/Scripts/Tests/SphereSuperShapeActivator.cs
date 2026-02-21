using Klaxon.GravitySystem;
using Klaxon.Interactable;
using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

public class SphereSuperShapeActivator : MonoBehaviour
{
    [Range(0,8)]
    public int layerPosition;

    public GameObject blueRain;
    public GameObject blueWalls;
    public CreateParticleSuperShape superShape;
    public ParticleSystem particleEffect;
    public Vector3Int fissureLocation;
    public Tilemap fissureMap;
    public Interactable ballSpawner;

    GameObject sculpturePiece;
    public Transform mainSculpture;
    [SerializeField]
    public List<Sound> sounds = new List<Sound>();
    public AudioMixerGroup mixerGroup;
    public GameObject finalLight;
    public CompleteTaskObject undertakingObject;
    public bool isActivated;

    bool setFromSave;
    
    private IEnumerator Start()
    {
        
        foreach (var clip in sounds)
        {
            GameObject _go = new GameObject("Sound_"+ clip.name);
            _go.transform.parent = transform;
            _go.transform.position = transform.position;
            clip.SetSource(_go.AddComponent<AudioSource>());
            clip.source.outputAudioMixerGroup = mixerGroup;
        }
        yield return new WaitForSeconds(1.0f);
        if(!setFromSave)
            SetTileWalkable(false);
        
    }

    public void SetActivatorFromSave(bool active)
    {
        isActivated = active;
        if (isActivated)
        {
            finalLight.SetActive(true);
            Destroy(blueRain);
            Destroy(blueWalls);
            fissureMap.SetTile(fissureLocation, null);
            var a = GetComponent<AudioSource>();
            a.Stop();
            a.enabled = false;
            GetComponent<Collider2D>().enabled = false;

        }
        setFromSave = true;
        SetTileWalkable(isActivated);
    }

    private void SetTileWalkable(bool canWalk)
    {
        PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[fissureLocation].walkable = canWalk;
    }

    IEnumerator FixAndSetSculptureArea()
    {
        if (undertakingObject.undertaking != null)
            undertakingObject.undertaking.TryCompleteTask(undertakingObject.task);
        PlaySound(0);
        yield return new WaitForSeconds(0.5f);
        PlaySound(1);
        PlaySound(2);
        //particle effect
        particleEffect.gameObject.SetActive(true);
        particleEffect.Play();

        yield return new WaitForSeconds(1f);
        
        // Set drops to hide at top
        var allDrops = blueRain.GetComponentsInChildren<HideDrop>();
        foreach (var drop in allDrops)
        {
            drop.StartFade();
        }

        // Set glowy wall to hide
        var allWalls = blueWalls.GetComponentsInChildren<HideSculptureWall>();
        foreach (var wall in allWalls)
        {
            wall.StartHide();
        }

        yield return new WaitForSeconds(4f);
        finalLight.SetActive(true);
        PlaySound(4);

        yield return new WaitForSeconds(1.2f);

        // purple rain stops
        Destroy(blueRain);
        Destroy(blueWalls);

        // close fissure
        fissureMap.SetTile(fissureLocation, null);

        // set supersahape ball and index
        superShape.particleLayers[0].particles[layerPosition].active = true;
        superShape.particleLayers[0].particles[layerPosition + 9].active = true;
        superShape.AddToTotalM(2);

        isActivated = true;

        SetTileWalkable(true);
        // destroy ourselves
        //yield return new WaitForSeconds(3f);
        //Destroy(gameObject);





        yield return null;
    }
    IEnumerator DeactivateBall()
    {
        // disable ballSpawner
        ballSpawner.canInteract = false;
        // set ball properties
        Vector3 directionZ = new Vector3(0, GlobalSettings.SpriteDisplacementY * 1, 1);
        float speed = 0.2f;
        if(sculpturePiece.TryGetComponent(out GravityItemMovementFree item))
        {
            item.currentDirection = Vector3.zero;
            item.isWeightless = true;
        }

        // move ball to center of tile
        Vector3 startPosition = sculpturePiece.transform.position;
        float timer = 0f;
        float waitTime = 1f;
        while (timer < waitTime)
        {
            var pos = Vector3.Lerp(startPosition, transform.position, timer / waitTime);
            sculpturePiece.transform.position = pos;
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        PlaySound(3);
        // get direction to main sculpture
        var mainDir = mainSculpture.position - item.transform.position;

        // move ball up
        timer = 0f;
        waitTime = 7f;
       
        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            item.MoveZ(directionZ.normalized, speed);
            item.Move(mainDir, speed*0.5f);
            speed += 0.01f;
            yield return null;
        }
        Destroy(sculpturePiece);
        // enable ballSpawner
        ballSpawner.canInteract = true;
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated)
            return;
        if (collision.gameObject.transform.position.z != transform.position.z)
            return;
        if (collision.CompareTag("SculptureBall"))
        {
            sculpturePiece = collision.gameObject;
            StartCoroutine(DeactivateBall());
            StartCoroutine(FixAndSetSculptureArea());
        }
            
    }


    void PlaySound(int soundIndex)
    {
        sounds[soundIndex].Play();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(fissureMap.GetCellCenterWorld(fissureLocation), 0.2f);
    }

}

using System.Collections.Generic;
using UnityEngine;

public class MuseumManager : MonoBehaviour
{
    public static MuseumManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    

    public List<RestorePainting> allArtPieces = new List<RestorePainting>();
    [HideInInspector]
    public List<RestorePainting> restorePaintingQueue = new List<RestorePainting>();

    void Start()
    {
        GameEventManager.onMuseumPieceUpdateEvent.AddListener(ArtPieceUpdated);
    }

    void OnDisable()
    {
        GameEventManager.onMuseumPieceUpdateEvent.RemoveListener(ArtPieceUpdated);
    }

    void ArtPieceUpdated()
    {
        foreach (var artPiece in allArtPieces)
        {
            if (restorePaintingQueue.Contains(artPiece))
                continue;

            foreach (var item in artPiece.ingredients)
            {
                if (item.complete && !item.activated)
                    restorePaintingQueue.Add(artPiece);
            }

        }
    }

    public bool HasPaintingsInQueue()
    {
        return restorePaintingQueue.Count > 0;
    }

    public RestorePainting GetNextPainting()
    {
        if (HasPaintingsInQueue())
            return restorePaintingQueue[0];
        return null;
    }

    public void RemovePaintingFromQueue()
    {
        restorePaintingQueue.RemoveAt(0);
    }
    void AddToQueueFromSave(int allArtIndex)
    {
        restorePaintingQueue.Add(allArtPieces[allArtIndex]);
    }
}

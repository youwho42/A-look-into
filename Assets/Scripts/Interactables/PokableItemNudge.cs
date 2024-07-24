using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokableItemNudge : PokableItem
{
    public float nudgeDistance = 0.05f;
    public override void PokeItemSuccess()
    {
        base.PokeItemSuccess();
        StartCoroutine("Nudge");

    }
    public override void PokeItemFail()
    {
        base.PokeItemFail();

    }
    IEnumerator Nudge()
    {
        var time = 0.3f;
        Vector3 startingPos = transform.position;
        Vector3 destination = transform.position - PlayerInformation.instance.player.position;
        destination = destination.normalized * nudgeDistance;
        destination += transform.position;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if(TryGetComponent(out ReplaceObjectOnItemDrop replace))
        {
            replace.ShowObjects(true);
            replace.CheckForObjects();
        }
        if (!GridManager.instance.GetTileValid(transform.position))
            Destroy(gameObject);
        yield return null;

    }
}

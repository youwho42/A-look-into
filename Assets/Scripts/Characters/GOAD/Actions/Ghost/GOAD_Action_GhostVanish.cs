using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_GhostVanish : GOAD_Action
    {
        SpriteRenderer[] ghostSprites;
        public List<GOAD_ScriptableCondition> appearConditions = new List<GOAD_ScriptableCondition>();
        bool hasAppeared;
        bool hasVanished;
        public override void StartAction(GOAD_Scheduler_Ghost agent)
        {
            base.StartAction(agent);
            agent.ghoster.vanishing = true;
            ghostSprites = agent.ghoster.GhostItem.GetComponentsInChildren<SpriteRenderer>();

            agent.animator.SetFloat(agent.velocityX_hash, 0);
            agent.ghoster.currentDirection = Vector2.zero;
            agent.aStarPath.Clear();
            agent.currentPathIndex = 0;
        }

        public override void PerformAction(GOAD_Scheduler_Ghost agent)
        {
            base.PerformAction(agent);
            if(agent.AreConditionsMet(appearConditions) && !hasAppeared)
            {
                hasAppeared = true;
                StartCoroutine(RiseGhostCO(agent));
                return;
            }

            if(!hasVanished)
            {
                hasVanished = true;
                StartCoroutine(SinkGhostCO(agent, agent.ghoster.GhostItem.localPosition.z));
            }
        }

        public override void EndAction(GOAD_Scheduler_Ghost agent)
        {
            base.EndAction(agent);
            agent.ghoster.vanishing = false;
            hasVanished = false;
            hasAppeared = false;
            foreach (var sprite in ghostSprites)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            }
            
            agent.ghoster.shadowSprite.color = new Color(agent.ghoster.shadowSprite.color.r, agent.ghoster.shadowSprite.color.g, agent.ghoster.shadowSprite.color.b, 0.8f);
        }

        IEnumerator SinkGhostCO(GOAD_Scheduler_Ghost agent, float startZ)
        {
            
            float waitTime = startZ * 2;
            float timer = waitTime;
            float zPos;
            while (timer >= 0.0f)
            {
                timer -= Time.deltaTime;
                zPos = Mathf.Lerp(-.3f, startZ, timer / waitTime);
                var disp = new Vector3(0, GlobalSettings.SpriteDisplacementY * zPos, zPos);
                agent.ghoster.GhostItem.localPosition = disp;

                if (zPos <= -0.1f)
                {
                    foreach (var sprite in ghostSprites)
                    {
                        var a = MapNumber.Remap(zPos, -.1f, -.3f, 1.0f, -0.02f);
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, a);
                    }
                    var b = MapNumber.Remap(zPos, -.1f, -.3f, 1.0f, -0.02f);
                    agent.ghoster.shadowSprite.color = new Color(agent.ghoster.shadowSprite.color.r, 
                        agent.ghoster.shadowSprite.color.g, 
                        agent.ghoster.shadowSprite.color.b, 
                        b);
                }
                else
                {
                    var a = MapNumber.Remap(zPos, 0.0f, agent.ghoster.amplitude+0.8f, 0.6f, 0.2f);
                    var c = new Color(agent.ghoster.shadowSprite.color.r,
                        agent.ghoster.shadowSprite.color.g, 
                        agent.ghoster.shadowSprite.color.b, 
                        a);
                    agent.ghoster.shadowSprite.color = c;
                }

                yield return null;

            }
            agent.ghoster.shadowSprite.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
            agent.transform.position = agent.ghostHome.transform.position;
            agent.ghoster.currentTilePosition.position = agent.ghoster.currentTilePosition.GetCurrentTilePosition(agent.transform.position);
            agent.ghoster.currentLevel = agent.ghoster.currentTilePosition.position.z;
            yield return null;
        }

        IEnumerator RiseGhostCO(GOAD_Scheduler_Ghost agent)
        {

            float waitTime = 2.2f;
            float timer = 0;
            float zPos;
            while (timer <= waitTime)
            {
                timer += Time.deltaTime;
                zPos = Mathf.Lerp(-.3f, 0.8f, timer / waitTime);
                var disp = new Vector3(0, GlobalSettings.SpriteDisplacementY * zPos, zPos);
                agent.ghoster.GhostItem.localPosition = disp;

                if (zPos <= 0.0f)
                {
                    foreach (var sprite in ghostSprites)
                    {
                        var a = MapNumber.Remap(zPos, -.3f, 0.0f, 0.0f, 1.0f);
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, a);
                    }
                    var b = MapNumber.Remap(zPos, -.3f, 0.0f, 0.0f, 1.0f);
                    agent.ghoster.shadowSprite.color = new Color(agent.ghoster.shadowSprite.color.r,
                        agent.ghoster.shadowSprite.color.g,
                        agent.ghoster.shadowSprite.color.b,
                        b);
                }
                else
                {
                    var a = MapNumber.Remap(zPos, 0.0f, 0.8f, 1.0f, 0.8f);
                    var c = new Color(agent.ghoster.shadowSprite.color.r,
                        agent.ghoster.shadowSprite.color.g,
                        agent.ghoster.shadowSprite.color.b,
                        a);
                    agent.ghoster.shadowSprite.color = c;
                }

                yield return null;

            }

            success = true;
            agent.SetActionComplete(true);

            yield return null;
        }
    }
}

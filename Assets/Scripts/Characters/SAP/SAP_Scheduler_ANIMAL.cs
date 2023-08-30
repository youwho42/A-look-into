using Klaxon.GravitySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Scheduler_ANIMAL : MonoBehaviour, IAnimal
    {

        public readonly int landed_hash = Animator.StringToHash("IsLanded");
        public readonly int walking_hash = Animator.StringToHash("IsWalking");
        public readonly int gliding_hash = Animator.StringToHash("IsGliding");
        public readonly int sleeping_hash = Animator.StringToHash("IsSleeping");
        public readonly int idle_hash = Animator.StringToHash("Idle");
        public Animator animator;

        public List<SAP_Goal> goals = new List<SAP_Goal>();

        public Dictionary<string, bool> beliefs = new Dictionary<string, bool>();
        [HideInInspector]
        public int currentGoal = -1;


        [HideInInspector]
        public AnimalSounds sounds;
        [HideInInspector]
        public MusicGeneratorItem musicItem;


        [HideInInspector]
        public bool currentGoalComplete;
        public string currentGoalName;
        [HideInInspector]
        public bool offScreen;
        int currentGoalTimer = -1;

        [HideInInspector]
        public GravityItemWalk walker;
        [HideInInspector]
        public GravityItemFly flier;
        [HideInInspector]
        public bool isDeviating;

        public Vector2 minMaxFlap;
        public Vector2 minMaxGlide;
        [HideInInspector]
        public bool glide;

        [HideInInspector]
        public DrawZasYDisplacement currentLandingSpot;


        public InteractAreasManager interactAreas;

        [HideInInspector]
        public RealTimeDayNightCycle dayNightCycle;

        void OnEnable()
        {
            GameEventManager.onTimeTickEvent.AddListener(TimeTick);
        }

        void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(TimeTick);
        }

        public void Start()
        {
            walker = GetComponent<GravityItemWalk>();
            if (walker != null)
            {
                walker.currentDir = Vector2.zero;
                walker.ResetLastPosition();
            }
                
            flier = GetComponent<GravityItemFly>();

            

            sounds = GetComponent<AnimalSounds>();
            musicItem = GetComponentInChildren<MusicGeneratorItem>();

            dayNightCycle = RealTimeDayNightCycle.instance;
            
            

        }

        public int SetRandomRange(Vector2Int minMaxRange)
        {
            return Random.Range(minMaxRange.x, minMaxRange.y);
        }

        public float SetRandomRange(Vector2 minMaxRange)
        {
            return Random.Range(minMaxRange.x, minMaxRange.y);
        }


        public void TimeTick(int tick)
        {
            if (currentGoal == -1)
                return;
            if (goals[currentGoal].TimeLimit > 0)
                currentGoalTimer++;
        }

        public void Update()
        {


            if (currentGoal == -1)
            {
                SetNewGoal();
            }
            else
            {
                goals[currentGoal].Action.PerformAction(this);

                if (currentGoal == -1)
                    return;

                if (goals[currentGoal].TimeLimit > 0)
                {

                    if (currentGoalTimer >= goals[currentGoal].TimeLimit)
                    {
                        
                        SetBeliefState(goals[currentGoal].TimeLimitCondition.Condition, goals[currentGoal].TimeLimitCondition.State);
                        ResetCurrentGoal();
                        currentGoalTimer = -1;
                    }
                }
            }


            if (currentGoalComplete && currentGoal >= 0)
            {
                goals[currentGoal].Action.EndPerformAction(this);
                currentGoal = -1;

                currentGoalComplete = false;
            }

        }
        

        void SetNewGoal()
        {

            int bestOption = -1;
            int bestIndex = -1;
            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].IsRunning = false;
                if (CanCompleteGoal(goals[i]))
                {
                    if (goals[i].Priority > bestOption)
                    {
                        bestOption = goals[i].Priority;
                        bestIndex = i;
                    }

                }
            }



            if (bestIndex > -1)
            {

                if (currentGoal != bestIndex)
                {
                    if (currentGoal > -1)
                        goals[currentGoal].Action.EndPerformAction(this);

                    goals[bestIndex].Action.StartPerformAction(this);

                    
                }
                

                currentGoal = bestIndex;
                goals[currentGoal].IsRunning = true;

                if (currentGoalName != goals[currentGoal].GoalName)
                {
                    if (goals[currentGoal].TimeLimitRange != Vector2Int.zero)
                        goals[currentGoal].TimeLimit = SetRandomRange(goals[currentGoal].TimeLimitRange);

                    currentGoalTimer = -1;
                }
                    

                currentGoalName = goals[currentGoal].GoalName;
            }
            else
            {
                currentGoalName = "";
                currentGoalTimer = -1;
            }

        }

        bool CanCompleteGoal(SAP_Goal goal)
        {
            Dictionary<string, bool> temp = new Dictionary<string, bool>(beliefs);

            // Combine the two dictionaries without modifying the original dictionaries
            foreach (var kvp in SAP_WorldBeliefStates.instance.worldStates)
            {
                temp[kvp.Key] = kvp.Value;
            }
            int conditionsMet = 0;
            foreach (var c in goal.Conditions)
            {
                bool state;
                if (temp.TryGetValue(c.Condition, out state))
                {
                    if (c.State == state)
                    {
                        conditionsMet++;
                    }
                }
            }
            return conditionsMet == goal.Conditions.Count;
        }

        public void DeviateWalk()
        {
            isDeviating = true;
            if (walker.isStuck)
                walker.hasDeviatePosition = false;

            if (!walker.hasDeviatePosition)
                walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);


            walker.SetDirection();

            if (walker.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            walker.SetLastPosition();

        }

        public void DeviateFly()
        {
            isDeviating = true;
            if (flier.isStuck)
                flier.hasDeviatePosition = false;

            if (!flier.hasDeviatePosition)
                flier.FindDeviateDestination(30);


            flier.SetDirection();

            if (flier.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            flier.SetLastPosition();

        }

        void ResetCurrentGoal()
        {
            if (currentGoal > -1)
                goals[currentGoal].Action.EndPerformAction(this);
            currentGoal = -1;
            currentGoalComplete = false;
        }

        public void SetBeliefState(string condition, bool state)
        {
            if (!beliefs.ContainsKey(condition))
                beliefs.Add(condition, state);
            else
                beliefs[condition] = state;
        }

        public bool GetBeliefState(string condition)
        {
            if (!beliefs.ContainsKey(condition))
                return false;
            else
                return beliefs[condition];
        }


        public void FleePlayer()
        {
            
        }

        public void SetActiveState(bool active)
        {
            
        }
    }
}

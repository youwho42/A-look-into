using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.StatSystem
{
    [Serializable]
    public struct InitialAmounts
    {
        public float Max;
        public float Current;
    }

    [CreateAssetMenu(menuName = "Klaxon/Stat Object", fileName = "New_Stat_Object")]
    public class StatObject : ScriptableObject
    {

        public string Name;
        [SerializeField]
        InitialAmounts initialAmounts;
        [SerializeField]
        float MaxAmount;
        [SerializeField]
        float CurrentAmount;
        List<StatModifier> Modifiers = new List<StatModifier>();
        [SerializeField]
        Vector2 ClampMax;
        public bool ConstantDecline;
        [ConditionalHide("ConstantDecline", true)]
        public float DeclinePerTick;
        float CurrentDeclineAmount;
        float LastCurrentAmount;
        public void SetMax(float amount)
        {
            MaxAmount = amount;
            if(ClampMax.y!=0)
                MaxAmount = Mathf.Clamp(MaxAmount, ClampMax.x, ClampMax.y);
        }

        public void SetCurrent(float amount)
        {
            CurrentAmount = amount;
            CurrentAmount = Mathf.Clamp(CurrentAmount + amount, 0, GetModifiedMax());
            
        }
        /// <summary>
        /// Increases the Stat Destination by the given RawNumber 
        /// </summary>
        /// <param name="changer"></param>
        public void ChangeStatRaw(StatChanger changer, float factor)
        {
            if(changer.ModifierDestination == ModifierDestination.CurrentAmount)
                CurrentAmount = Mathf.Clamp(CurrentAmount + GetModifiedChangeAmount(changer.Amount * factor), 0, GetModifiedMax());
            else if (changer.ModifierDestination == ModifierDestination.MaxAmount)
            {
                if (ClampMax.y != 0)
                    MaxAmount = Mathf.Clamp(MaxAmount + (changer.Amount * factor), ClampMax.x, ClampMax.y);
                else
                    MaxAmount += changer.Amount * factor;
            }
            CheckStatFilled();
            GameEventManager.onStatUpdateEvent.Invoke();
        }
        

        /// <summary>
        /// Multiplies the Destination Amount by the given amount
        /// </summary>
        /// <param name = "changer" ></ param >
        public void ChangeStatPercent(StatChanger changer)
        {
            if (changer.ModifierDestination == ModifierDestination.CurrentAmount)
                CurrentAmount = Mathf.Clamp(CurrentAmount * changer.Amount, 0, GetModifiedMax());
            else if (changer.ModifierDestination == ModifierDestination.MaxAmount)
            {
                if (ClampMax.y != 0)
                    MaxAmount = Mathf.Clamp(MaxAmount * changer.Amount, ClampMax.x, ClampMax.y);
                else
                    MaxAmount *= changer.Amount;
            }
            CheckStatFilled();
            GameEventManager.onStatUpdateEvent.Invoke();
        }
        void CheckStatFilled()
        {

            if (CurrentAmount < MaxAmount || LastCurrentAmount == CurrentAmount)
            {
                LastCurrentAmount = CurrentAmount;
                return;
            }
            LastCurrentAmount = CurrentAmount;
            AudioManager.instance.PlaySound("StatFilled");
        }

        public float GetModifiedChangeAmount(float changeAmount)
        {
            float percent = 0;
            float final = changeAmount;
            foreach (var mod in Modifiers)
            {
                if (mod.ModifierDestination == ModifierDestination.ChangerAmount)
                {
                    percent += mod.finalModifierAmount;
                    final = changeAmount * (percent + 1);
                }
            }
            return final;
        }


        public float GetRawMax()
        {
            return MaxAmount;
        }

        public float GetRawCurrent()
        {
            return CurrentAmount;
        }
        /// <summary>
        /// Returns the MaxAmount plus its modifiers
        /// </summary>
        /// <returns></returns>
        public float GetModifiedMax()
        {
            float amount = 0;
            float percent = 0;
            float final = MaxAmount;
            foreach (var mod in Modifiers)
            {
                if(mod.ModifierDestination == ModifierDestination.MaxAmount)
                {
                    if (mod.ModifierType == ModifierType.RawNumber)
                    {
                        amount += mod.finalModifierAmount;
                        final = MaxAmount + amount;
                    }
                    else
                    {
                        percent += mod.finalModifierAmount;
                        final = MaxAmount * (percent + 1);
                    }
                }
                
            }
            if(ClampMax.y > 0)
                final = Mathf.Clamp(final, ClampMax.x, ClampMax.y);
            return final;
        }

        /// <summary>
        /// Returns the CurrentAmount
        /// </summary>
        /// <returns></returns>
        public float GetModifiedCurrent()
        {
            float amount = 0;
            float percent = 0;
            foreach (var mod in Modifiers)
            {
                if (mod.ModifierDestination == ModifierDestination.CurrentAmount)
                {
                    if (mod.ModifierType == ModifierType.RawNumber)
                        amount += mod.finalModifierAmount;
                    else
                    {
                        percent += mod.finalModifierAmount;
                        amount += CurrentAmount * percent;
                    } 
                }
            }
            var final = Mathf.Clamp(CurrentAmount + amount, 0, GetModifiedMax());
            return final;
        }

        

        public void AddModifier(StatModifier modifier)
        {
            if (!Modifiers.Contains(modifier))
                Modifiers.Add(modifier);
            modifier.IncreaseTimer(modifier.ModifierDuration);
        }

        public void DecreaseModifiersTimer()
        {
            List<StatModifier> modsToRemove = new List<StatModifier>();
            foreach (var mod in Modifiers)
            {
                if (mod == null)
                {
                    modsToRemove.Add(mod);
                    continue;
                }
                if (!mod.DecreaseModifierTimer() && mod.TimedModifier)
                    modsToRemove.Add(mod);
            }
            foreach (var mod in modsToRemove)
            {
                RemoveModifier(mod);
            }

        }

        public void ConstantDeclineTick()
        {
            if (!ConstantDecline)
                return;
            CurrentDeclineAmount += DeclinePerTick;
            CurrentAmount = Mathf.Clamp(CurrentAmount -= DeclinePerTick, 0, MaxAmount);
            if (CurrentDeclineAmount >= 1)
            {
                CurrentDeclineAmount = 0;
                GameEventManager.onStatUpdateEvent.Invoke(); 
            }
        }

        public void RemoveModifier(StatModifier modifier)
        {
            if (Modifiers.Contains(modifier))
            {
                modifier.ResetModifier();
                Modifiers.Remove(modifier);
            }
            GameEventManager.onStatUpdateEvent.Invoke();
        }

        public void RemoveAllModifiers()
        {
            foreach (var mod in Modifiers)
            {
                if(mod != null)
                    mod.ResetModifier();
            }
            Modifiers.Clear();
        }

        public void ResetStat()
        {
            MaxAmount = initialAmounts.Max;
            CurrentAmount = initialAmounts.Current;
        }

        // SAVE SYSTEM //
        public void AddModifierFromSave(StatModifier modifier, int timer)
        {
            Modifiers.Add(modifier);
            modifier.SetTimer(timer);
        }
        // SAVE SYSTEM //
        public int GetModifiersCount()
        {
            return Modifiers.Count;
        }
        // SAVE SYSTEM //
        public List<string> GetModifierNames()
        {
            var list = new List<string>();
            foreach (var mod in Modifiers)
            {
                list.Add(mod.ModifierName);
            }
            return list;
        }
        // SAVE SYSTEM //
        public List<StatModifier> GetModifierList()
        {
            var list = new List<StatModifier>(Modifiers);
            return list;
        }
        // SAVE SYSTEM //
        public List<int> GetModifierTimers()
        {
            var list = new List<int>();
            foreach (var mod in Modifiers)
            {
                list.Add(mod.GetTimer());
            }
            return list;
        }


        
    }
}

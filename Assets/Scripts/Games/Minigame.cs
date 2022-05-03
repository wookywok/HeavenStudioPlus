using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games
{
    public class Minigame : MonoBehaviour
    {
        public static float earlyTime = 0.84f, perfectTime = 0.91f, lateTime = 1.09f, endTime = 1.15f;
        public List<Minigame.Eligible> EligibleHits = new List<Minigame.Eligible>();

        [System.Serializable]
        public class Eligible
        {
            public GameObject gameObject;
            public bool early;
            public bool perfect;
            public bool late;
            public bool notPerfect() { return early || late; }
            public bool eligible() { return early || perfect || late; }
            public float createBeat;
        }


        /**
         * Schedule an Input for a later time in the minigame. Executes the methods put in parameters
         * 
         * float                     startBeat   : When the scheduling started (In beats)
         * float                     timer       : How many beats later should the input be recorded
         * InputType                 inputType   : The type of the input that's expected (Press, release, A, B, Directions..)
         * ActionEventCallbackState  OnHit       : Method to run if the Input has been Hit
         * ActionEventCallback       OnMiss      : Method to run if the Input has been Missed
         * ActionEventCallback       OnBlank     : Method to run whenever there's an Input while this is Scheduled (Shouldn't be used too much)
         */
        public PlayerActionEvent ScheduleInput(float startBeat,
            float timer,
            InputType inputType,
            PlayerActionEvent.ActionEventCallbackState OnHit,
            PlayerActionEvent.ActionEventCallback OnMiss,
            PlayerActionEvent.ActionEventCallback OnBlank)
        {

            GameObject evtObj = new GameObject("ActionEvent" + (startBeat+timer));
            evtObj.AddComponent<PlayerActionEvent>();

            PlayerActionEvent evt = evtObj.GetComponent<PlayerActionEvent>();

            evt.startBeat = startBeat;
            evt.timer = timer;
            evt.inputType = inputType;
            evt.OnHit = OnHit;
            evt.OnMiss = OnMiss;
            evt.OnBlank = OnBlank;

            evt.canHit = true;
            evt.enabled = true;

            evt.transform.parent = this.transform.parent;

            //Instantiate(evtObj);
            evtObj.SetActive(true);

            return evt;
        }

        // hopefully these will fix the lowbpm problem
        public static float EarlyTime()
        {
            return earlyTime;
        }

        public static float PerfectTime()
        {
            return perfectTime;
        }

        public static float LateTime()
        {
            return lateTime;
        }

        public static float EndTime()
        {
            return endTime;
        }

        public int firstEnable = 0;

        public virtual void OnGameSwitch(float beat)
        {
            //Below is a template that can be used for handling previous entities.
            //section below is if you only want to look at entities that overlap the game switch
            /*
            List<Beatmap.Entity> prevEntities = GameManager.instance.Beatmap.entities.FindAll(c => c.beat <= beat && c.datamodel.Split(0) == [insert game name]);
            foreach(Beatmap.Entity entity in prevEntities)
            {
                if(entity.beat + entity.length >= beat)
                {
                    EventCaller.instance.CallEvent(entity, true);
                }
            }
            */
        }

        public virtual void OnTimeChange()
        {

        }

        public int MultipleEventsAtOnce()
        {
            int sameTime = 0;
            for (int i = 0; i < EligibleHits.Count; i++)
            {
                float createBeat = EligibleHits[i].createBeat;
                if (EligibleHits.FindAll(c => c.createBeat == createBeat).Count > 0)
                {
                    sameTime += 1;
                }
            }

            if (sameTime == 0 && EligibleHits.Count > 0)
                sameTime = 1;

            return sameTime;
        }
    }
}
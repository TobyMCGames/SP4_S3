using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityControl : MonoBehaviour
{
    //private Ability CurAbility = null;
    //public Ability[] Abilities;
    private List<Ability> AbilitiesList = new List<Ability>();
    private float ScrollLimit = 2f;
    private float ScrollValue = 0f;
    private int AbilityCount = 0;
    private int currAbility = 0;
    DistactAbility distactAbility = new DistactAbility();
    LayEggAbility layEggAbility = new LayEggAbility();

    private void Start()
    {
        AbilitiesList.Add(layEggAbility);
        AbilitiesList.Add(distactAbility);
        AbilityCount = AbilitiesList.Count;
    }

    private void Update()
    {
        ScrollValue += Input.mouseScrollDelta.y;
        if(ScrollValue >= ScrollLimit)
        {
            currAbility += 1;
            ScrollValue = 0;

            if(currAbility >= AbilityCount)
            {
                currAbility = 0;
            }

            Debug.Log(currAbility);
        }
        else if (ScrollValue <= -ScrollLimit)
        {
            currAbility -= 1;
            ScrollValue = 0;

            if (currAbility < 0)
            {
                currAbility = AbilityCount - 1;
            }

            Debug.Log(currAbility);
        }

        if(Input.GetKeyDown(KeyCode.Q) && AbilitiesList[currAbility].CanUse())
        {
            AbilitiesList[currAbility].UseAbility();
        }

        layEggAbility.Update();
        distactAbility.Update();
    }

}

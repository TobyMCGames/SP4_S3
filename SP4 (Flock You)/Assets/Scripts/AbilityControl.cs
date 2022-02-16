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
    LayEgg layEggAbility = new LayEgg();

    private void Start()
    {
        AbilitiesList.Add(layEggAbility);
        AbilityCount = AbilitiesList.Count;
    }

    public void switchSkill(int i)
    {

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
        }
        else if (ScrollValue <= -ScrollLimit)
        {
            currAbility -= 1;
            ScrollValue = 0;

            if (currAbility < 0)
            {
                currAbility = AbilityCount - 1;
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            AbilitiesList[currAbility].UseAbility();
        }

        layEggAbility.Update();
    }

}

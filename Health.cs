using UnityEngine;
using System.Collections;

[System.Serializable]
public class Health {

    [System.NonSerialized]
    public int hull = 3, sheilds = 5;

    public int maxHull = 3, maxSheilds = 5;

    public float shieldRefreshRate = 0.5f;//rate that shields refresh at
    public float shieldRefreshDelay = 3f;//delay to start refreshing shields after taking damage

    [System.NonSerialized]
    public float currentRefresh = 0f;

    public float refreshNormalised { get { return currentRefresh / shieldRefreshRate ; } }

    public CanvasController canvasController;

    public GameObject sheildMesh;

    public Health()
    {
        //Debug.Log("is this work?");

        hull = maxHull;
        sheilds = maxSheilds;
    }

    public void SetStats()
    {
        hull = maxHull;
        sheilds = maxSheilds;
    }

    public int TakeDamage(int ammount, DamageType.DamageTypes dType = DamageType.DamageTypes.Default)
    {
        
        switch (dType)
        {
            case DamageType.DamageTypes.Default://Default
                TakeDamage_Default(ammount);
                break;

            case DamageType.DamageTypes.Direct://Direct
                TakeDamage_Direct(ammount);
                break;

            case DamageType.DamageTypes.ShieldsOnly://shields only
                TakeDamage_ShieldsOnly(ammount);
                break;

            default:
                TakeDamage_Default(ammount);
                break;
        }
       
        return hull;
    }

    void TakeDamage_Default(int ammount)
    {
        int hullDamage = TakeDamage_ShieldsOnly(ammount);

        TakeDamage_Direct(hullDamage);
    }

    void TakeDamage_Direct(int ammount)
    {
        hull -= ammount;

        if (hull < 0)
        {
            hull = 0;
        }
    }

    int TakeDamage_ShieldsOnly(int ammount)
    {

        if(sheilds == 0)
        {
            return ammount;
        }

        int remaningDamage = 0;
        sheilds -= ammount;

        if (sheilds < 0)
        {
            remaningDamage = sheilds * -1;
            sheilds = 0;
        }

        if(sheildMesh != null && sheilds < 1)
        {
            sheildMesh.SetActive(false);
        }

        return remaningDamage;
    }

    public IEnumerator RefreshSheilds()//will be controlled from fighter script
    {
        yield return new WaitForSeconds(shieldRefreshDelay);

        float t  = 0f;
        while (sheilds < maxSheilds)
        {
            currentRefresh += Time.deltaTime;
            t += Time.deltaTime;

            if(currentRefresh >= shieldRefreshRate)
            {               
                sheilds++;
                currentRefresh = currentRefresh - shieldRefreshRate;

                if (sheildMesh != null)
                {
                    sheildMesh.SetActive(true);
                }

                if (canvasController != null)
                {
                    canvasController.UpdateHealthBar(hull, sheilds);
                }        
            }            

            yield return null;
        }
        float x = shieldRefreshRate * maxSheilds;
        //Debug.Log(t + ", should be equal to " + x);
    }
	
}

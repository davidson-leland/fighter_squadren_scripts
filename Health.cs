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

    public int TakeDamage(int ammount)
    {
        
        sheilds -= ammount;
            

        if(sheilds < 0)
        {
            hull += sheilds;

            sheilds = 0;

            if(hull < 0)
            {
                hull = 0;                
            }
        }
       
        return hull;
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
                
                if(canvasController != null)
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

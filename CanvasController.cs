using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {

    [SerializeField]
    protected Canvas canvas;
    [SerializeField]
    protected RectTransform canvasTargetRet;

    [SerializeField]
    Color hullColor, hullBrokenColor, shieldColor, shieldBrokenColor, shieldRefreshColor;

    [SerializeField]
    RectTransform healthBarBackground;
    

    [SerializeField]
    GameObject healthBlipPrefab;

    struct HealthBlip
    {
        public string type;
       public RectTransform rectTransform;
        public Image image;

        public HealthBlip(string _type, RectTransform _rectTransform, Image _image)
        {
            type = _type;
            rectTransform = _rectTransform;
            image = _image;
        }
    }

    HealthBlip[] healthBlips;

    [SerializeField]
    PlayerController playerOwner;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InitHealthBar(int hull, int shields)
    {
        if(healthBlips != null)
        {
            ClearHealthBar();
        }

        int totalHealth = hull + shields;
        healthBarBackground.sizeDelta = new Vector2(70, 30 * (totalHealth) + 10);

        healthBlips = new HealthBlip[totalHealth];               
        Vector3 blipPosition = new Vector3(0, 20, 0);

        for (int i = 0; i < totalHealth; i++)
        {
            GameObject newBlip = Instantiate(healthBlipPrefab);
            RectTransform newRT = newBlip.GetComponent<RectTransform>();
            Image newImage = newBlip.GetComponent<Image>();

            newRT.position = blipPosition;
            blipPosition.y += 30;
            newBlip.transform.SetParent( healthBarBackground, false);           
            
            healthBlips[i] = new HealthBlip("Hull", newRT, newImage);

            if(i > hull -1)
            {
                healthBlips[i].type = "shield";
                healthBlips[i].image.color = shieldColor;
            }
        }
    }

    public void ClearHealthBar()
    {
        foreach(HealthBlip hB in healthBlips)
        {
            Destroy(hB.rectTransform.gameObject);
        }       
    }

    public void UpdateHealthBar(int newHull, int newShields)
    {
        //Debug.Log(newHull + "____" + newShields);
        for (int i = 0; i < healthBlips.Length; i++)
        {
            if(i < playerOwner.fighterHealth.hull)
            {
                if( i + 1 > newHull)
                {
                    //Debug.Log("test2");
                    healthBlips[i].image.color = hullBrokenColor;
                }
            }
            else
            {
                if( i - playerOwner.fighterHealth.maxHull + 1 > newShields)
                {
                    //Debug.Log("test3");
                    healthBlips[i].image.color = shieldBrokenColor;
                }
                else
                {
                    healthBlips[i].image.color = shieldColor;
                }
            }
        }
    }
}

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

    [SerializeField]
    GameObject spawnMenu;

    [SerializeField]
    Text spawnText;

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

    //create player health bar there is a glitch in here somewhere i will have to figure out later.
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
        for (int i = 0; i < healthBlips.Length; i++)
        {
            if(i < playerOwner.myFighter.health.hull)
            {
                if( i + 1 > newHull)
                {
                    healthBlips[i].image.color = hullBrokenColor;
                }
            }
            else
            {
                if( i - playerOwner.myFighter.health.maxHull + 1 > newShields)
                {
                    healthBlips[i].image.color = shieldBrokenColor;
                }
                else
                {
                    healthBlips[i].image.color = shieldColor;
                }
            }
        }
    }

    public void ShowSpawnMenu(bool showMenu)
    {
        spawnMenu.SetActive(showMenu);
    }

    public void SetSpawnText(string inString)
    {
        spawnText.text = "Spawn As : " + inString;
    }
}

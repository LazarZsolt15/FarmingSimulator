using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public GameObject tree1Prefab;
    public GameObject tree2Prefab;
    public GameObject tree3Prefab;
    
    private GameObject currentTreeInstance;
    public bool reset = false;  
    public UIControl uicon;
    public bool started = false;
    public bool hasgivenTime = false;
    private float nextSwitchTime;

    public void Initialize()
    {
        started = true;

    }

    private void SwitchToNextTree()
    {
            
                if (gameObject.GetComponent<PrefabIdentifier>().prefabType == PrefabType.Tree1)
                {
                    ReplaceTrees(tree2Prefab);
                }
                else if (gameObject.GetComponent<PrefabIdentifier>().prefabType == PrefabType.Tree2 && gameObject.tag != "Bush" && gameObject.tag != "Bush2")
                {
                    ReplaceTrees(tree3Prefab);
                }
                else if(gameObject.GetComponent<PrefabIdentifier>().prefabType == PrefabType.Tree3 && gameObject.tag != "Bush" && gameObject.tag != "Bush2")
                {
                    ReplaceTrees(tree1Prefab);
                }
                else if (gameObject.GetComponent<PrefabIdentifier>().prefabType == PrefabType.Tree2 && gameObject.tag == "Bush" || gameObject.tag == "Bush2")
                {
                    ReplaceTrees(tree1Prefab);
                }
        
    }
    
    private void ScheduleNextSwitch()
    {
        nextSwitchTime = Time.time + Random.Range(10, 20);
    }


    void Update()
    {
        if (started)
        {
            if (!hasgivenTime)
            {
                ScheduleNextSwitch();
                hasgivenTime = true;
            }
            if (Time.time >= nextSwitchTime)
            {
                hasgivenTime = false;
                SwitchToNextTree();
            }
        }

        if (reset && gameObject.tag != "Bush" && gameObject.tag != "Bush2")
        {
            if (gameObject.GetComponent<PrefabIdentifier>().prefabType == PrefabType.Tree1)
            {
                uicon.score+=1;
               
            }
            if (gameObject.GetComponent<PrefabIdentifier>().prefabType == PrefabType.Tree2)
            {
                uicon.score+=2;
             
            }
            if (gameObject.GetComponent<PrefabIdentifier>().prefabType == PrefabType.Tree3)
            {
                uicon.score+=3;
               
            }
            ResetTreeToFirst();
            reset = false;  
        }
        else if(reset && (gameObject.tag == "Bush" || gameObject.tag == "Bush2"))
        {
            if (gameObject.GetComponent<PrefabIdentifier>().prefabType == PrefabType.Tree1)
            {
                uicon.score += 1;
             
            }
            ResetTreeToFirst();
            reset = false; 
        }
    }


void ReplaceTrees(GameObject newPrefab)
{
        currentTreeInstance = gameObject;
    if (currentTreeInstance == null)
    {
        Debug.LogError("currentTreeInstance is null. Please ensure it's assigned correctly.");
        return;
    }

    GameObject oldInstance = currentTreeInstance;
    string tag = oldInstance.tag;
    Transform parent = oldInstance.transform.parent;
    Destroy(oldInstance);

    currentTreeInstance = Instantiate(newPrefab, transform.position, Quaternion.identity);
    currentTreeInstance.tag = tag;
    currentTreeInstance.transform.parent = parent;

    if (oldInstance != null)
    {
            CopyUpgradeComponent(oldInstance, currentTreeInstance);
            CopyPrefabIdentifierComponent(oldInstance, currentTreeInstance); 

        }

        if (currentTreeInstance.GetComponent<Upgrade>() == null)
        {
            currentTreeInstance.AddComponent<Upgrade>();
        }
    }

void ResetTreeToFirst()
{
        currentTreeInstance = gameObject;
       
        if (currentTreeInstance == null)
    {
        Debug.LogError("currentTreeInstance is null. Please ensure it's assigned correctly.");
        return;
    }

    GameObject oldInstance = currentTreeInstance;
    string tag = oldInstance.tag;
    Transform parent = oldInstance.transform.parent;
    Destroy(oldInstance);

    currentTreeInstance = Instantiate(tree1Prefab, transform.position, Quaternion.identity);
    currentTreeInstance.tag = tag;
    currentTreeInstance.transform.parent = parent;

        if (oldInstance != null)
        {
            CopyUpgradeComponent(oldInstance, currentTreeInstance);
            CopyPrefabIdentifierComponent(oldInstance, currentTreeInstance);
        }

        if (currentTreeInstance.GetComponent<Upgrade>() == null)
        {
            currentTreeInstance.AddComponent<Upgrade>();
        }

        hasgivenTime = true;
        ScheduleNextSwitch();
    }

    void CopyPrefabIdentifierComponent(GameObject oldInstance, GameObject newObject)
    {
        PrefabIdentifier oldIdentifier = oldInstance.GetComponent<PrefabIdentifier>();
        if (oldIdentifier != null)
        {
            if (newObject.GetComponent<PrefabIdentifier>() == null)
            {
                PrefabIdentifier newIdentifier = newObject.AddComponent<PrefabIdentifier>();
                if (oldInstance.CompareTag("Bush") || oldInstance.CompareTag("Bush2"))
                {
                    newIdentifier.prefabType = oldIdentifier.prefabType == PrefabType.Tree1 ? PrefabType.Tree2 : PrefabType.Tree1;
                }
                else
                {
                    newIdentifier.prefabType = oldIdentifier.prefabType;
                }
            }
        }
    }

    void CopyUpgradeComponent(GameObject oldInstance, GameObject newObject)
    {
        Upgrade oldUpgrade = oldInstance.GetComponent<Upgrade>();
        if (oldUpgrade != null)
        {
            Upgrade newUpgrade = newObject.AddComponent<Upgrade>();

            System.Reflection.FieldInfo[] fields = typeof(Upgrade).GetFields();
            foreach (var field in fields)
            {
                if (field.Name == "reset")
                {
                    field.SetValue(newUpgrade, false);
                }
                else
                {
                    field.SetValue(newUpgrade, field.GetValue(oldUpgrade));
                }
            }
        }
    }
}

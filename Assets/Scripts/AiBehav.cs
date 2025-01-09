using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using TMPro;

public class AiBehav : Agent
{
    public List<string> FifoCommands = new List<string>();
    public bool acceptState;
    private bool moveState;
    private bool taskState;
    private float percentage = 0f;
    public TextMeshProUGUI percentageText;

    public GameObject targetObject;
    public GameObject targetObject2;
    public InputControl targetInputControl; 

    public float activationDistance = 5f;
    public Transform currentTarget;
    
    private Rigidbody agentRigidbody;
    private Vector3 previousPosition; 
    private float previousDistance; 

    private float timer = 0f; 
    private float nextChangeTime = 0f;
    float distance = 0.0f;
    private float lastScore = 0.0f;

    private bool tempScore = false;
    
    private int lastvalue = -1;

    [SerializeField]
    private Material sucessMaterial;

    [SerializeField]
    private Material failMaterial;

    [SerializeField]
    private Material defaultMaterial;

    [SerializeField]
    private Terrain ground;

    private float episodeStartTime; 
    private float maxEpisodeDuration = 100.0f; 
    /*
    private IEnumerator SwapGround(Material mat, float time){
        ground.materialTemplate  = mat;      
        yield return new WaitForSeconds(time);
        ground.materialTemplate  = defaultMaterial;      
    }
    */
    void Start()
    {
        acceptState = true;
        moveState = false;
        taskState = false;
        percentageText.text = $"Accepting First";

        agentRigidbody = GetComponent<Rigidbody>();
        previousPosition = transform.localPosition;
    }
    void Update()
    {
        if (targetObject != null && targetInputControl != null){

            float distance = Vector3.Distance(transform.position, targetObject.transform.position); 

            if (distance <= activationDistance)
            {
                targetInputControl.visible = true; 
            }
            else
            {
                targetInputControl.visible = false; 
            }

        }
        else if (targetObject2 != null && targetInputControl != null){

            float distance = Vector3.Distance(transform.position, targetObject2.transform.position); 

            if (distance <= activationDistance)
            {
                targetInputControl.visible = true; 
            }
            else
            {
                targetInputControl.visible = false; 
            }

        }
        /*
        if (Time.time - episodeStartTime >= maxEpisodeDuration)
        {
            //transform.localPosition = new Vector3(4.37f,-2.929f,0.09f);
            Debug.Log("Fail: " + currentTarget.name);
           EndEpisode();
        }
        */
    if (currentTarget != null)
    {    
        tempScore = false;
        string targetName = currentTarget.gameObject.name;
        switch (targetName)
        {
            case "C1":
                tempScore = CollectTreeLevels("Corn");
                break;
            case "C3":
                tempScore = CollectTreeLevels("Tree");
                break;
            case "C7":
                tempScore = CollectTreeLevels("Tree2");
                break;
            case "C6":
                tempScore = CollectTreeLevels("Bush2");
                break;
            case "C5":
                tempScore = CollectTreeLevels("Bush");
                break;
            case "C4":
                tempScore = CollectTreeLevels("Corn2");
                break;
            default:
                tempScore = true;
                break;
        }
    }
        if (taskState)
        {
            percentage += Time.deltaTime * 10;
            percentage = Mathf.Clamp(percentage, 0, 100);
             if (percentageText != null)
            {
                percentageText.text = $"{percentage.ToString("0")}%";
            }
                if (distance > 6.0f)
                {
                    AddReward(-0.5f);
                }
                previousPosition = transform.localPosition;                            }

            if (percentage == 100)
            {
                taskState = false;
                acceptState = true;
                percentageText.text = $"Accepting";

                FifoCommands.Clear(); 
                EndEpisode();
                if (FifoCommands.Count != 0)
                {
                    ExecCommand(); 
                }
                else{
                    currentTarget = null;
                }
            }
        }
    public void pushCommand(string newCommand)
    {
        FifoCommands.Add(newCommand.ToUpper());
        EndEpisode();
    }

    public void SetNewDestination(Transform newTarget)
    {
        if (acceptState)
        {
            acceptState = false;
            moveState = true;

            currentTarget = newTarget;
            percentage = 0f;
            percentageText.text = $"Move";

            Debug.Log("###############");
            Debug.Log("AcceptState: OFF");
            Debug.Log("MoveState: ON");
        }
    }

    public void ExecCommand()
    {
        if (FifoCommands.Count == 0) return;

        string currentCommand = FifoCommands[0];
        FifoCommands.RemoveAt(0);

        GameObject targetObject = GameObject.Find(currentCommand);

        if (targetObject != null)
        {
            Transform newTarget = targetObject.transform;
            this.SetNewDestination(newTarget);
        }
        else
        {
            Debug.LogWarning("Hiba, nem található a GameObject.");
        }

    }

    public override void OnEpisodeBegin()
    {
        episodeStartTime = Time.time;
        acceptState = true;
        moveState = false;
        taskState = false;
        percentageText.text = $"Accepting First";
        ExecCommand();
    }


    public override void CollectObservations(VectorSensor sensor){     
        if(currentTarget != null){
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(currentTarget.localPosition);
        sensor.AddObservation(agentRigidbody.velocity.x);
        sensor.AddObservation(agentRigidbody.velocity.z);
        sensor.AddObservation(CanStartAction() ? 1.0f : 0.0f);

        Debug.Log("Start activity: " + CanStartAction());
        Debug.Log("Ide megyünk: " + currentTarget.gameObject.name);
        }
        else{
             sensor.AddObservation(0.0f);
             sensor.AddObservation(0.0f);
             sensor.AddObservation(0.0f);
             sensor.AddObservation(0.0f);
             sensor.AddObservation(0.0f);
             sensor.AddObservation(0.0f);
             sensor.AddObservation(0.0f);
             sensor.AddObservation(0.0f);
             sensor.AddObservation(0.0f);
        }
    }
    private bool CollectTreeLevels(string tag)
    {
        GameObject[] parentObjects = GameObject.FindGameObjectsWithTag(tag); 
        float temp = 0.0f;
        float i = 0.0f;
        float targetAreaScore = 0.0f;

        if (parentObjects != null)
        {
            foreach (var obj in parentObjects)
            {
                i++;
                PrefabIdentifier identifier = obj.GetComponent<PrefabIdentifier>();
                Upgrade upgradeScript = obj.GetComponent<Upgrade>();

                if (identifier != null && upgradeScript != null)
                {
                    switch (identifier.prefabType)
                    {
                        case PrefabType.Tree1:
                            temp += 1;
                            break;
                        case PrefabType.Tree2:
                            if(tag != "Bush" && tag != "Bush2") temp += 2;
                            break;
                        case PrefabType.Tree3:
                            temp += 3;
                            break;
                        default:
                            temp += 0;
                            break;
                    }
                }
            }
            if (tag != "Bush" && tag != "Bush2")
            {
                targetAreaScore = i * 3;
                targetAreaScore = targetAreaScore / 1.5f;
            }
            else
            {
                targetAreaScore = i;
                targetAreaScore = targetAreaScore / 2.0f;
            }
        }
        if(lastScore > temp){
            lastScore = temp;
            return false;
        } 
        lastScore = temp;
        Debug.Log("Nedded: " + targetAreaScore + " Actual: " + temp);
        return temp > targetAreaScore;
    }

    private bool CanStartAction(){
        if(Vector3.Distance(transform.position, currentTarget.transform.position) < 6.0f && tempScore){
            return true;
        }
        else{
            return false;
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if(currentTarget != null){
            int moveX = actionBuffers.DiscreteActions[0];
            int moveZ = actionBuffers.DiscreteActions[1];
            int buttonPressed = actionBuffers.DiscreteActions[2]; 

            Vector3 addForce = Vector3.zero;

            switch (moveX)
            {
                case 0: addForce.x = 0f; break;
                case 1: addForce.x = -1f; break;
                case 2: addForce.x = +1f; break;
            }
            switch (moveZ)
            {
                case 0: addForce.z = 0f; break;
                case 1: addForce.z = -1f; break;
                case 2: addForce.z = +1f; break;
            }

            float movespeed = 10.0f;
            agentRigidbody.velocity = addForce * movespeed + new Vector3(0, agentRigidbody.velocity.y, 0);
            distance = Vector3.Distance(transform.localPosition, currentTarget.localPosition); 

            if (currentTarget != null && distance < 6.0f)
            {
             AddReward(1.0f);
            //StartCoroutine(SwapGround(sucessMaterial,1f));

                if (buttonPressed == 1 && moveState && tempScore)
                {
                    Debug.Log("Távolság gombynomáskor: " + distance);
                    moveState = false;
                    acceptState = false;
                    taskState = true;
                    Debug.Log("###############");
                    Debug.Log("MoveState: OFF");
                    Debug.Log("AcceptState: OFF");
                    Debug.Log("TaskState: ON" + currentTarget.name);
                    Debug.Log(transform.parent.name);
                    AddReward(1.0f);
                 }
                else if(buttonPressed == 1 && !tempScore)
                { 
                    AddReward(-1.0f);
                    //StartCoroutine(SwapGround(failMaterial,0.5f));
                   //Debug.Log("Túl korai gombnyomás: " + tempScore);
                }
 
            }
            else if (buttonPressed == 1 && distance > 6.0f)
            {
                AddReward(-1.0f);
                //StartCoroutine(SwapGround(failMaterial,0.5f));
               // Debug.Log("Túl messze vagyunk a gombnyomáshoz");
            }
            else if(distance > 6.0f){
            }
            previousDistance = distance;
            previousPosition = transform.localPosition;
        }
    }    

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: discreteActions[0] = 1; break;
            case 0: discreteActions[0] = 0; break;
            case 1: discreteActions[0] = 2; break;
        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1: discreteActions[1] = 1; break;
            case 0: discreteActions[1] = 0; break;
            case 1: discreteActions[1] = 2; break;
        }

        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-1.0f);
            //StartCoroutine(SwapGround(failMaterial,0.5f));
            //Debug.Log("AI ütközött egy kerítéssel. -Reward!");
        }
    }
}

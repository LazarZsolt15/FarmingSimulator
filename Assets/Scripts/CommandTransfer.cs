using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Linq;


public class CommandTransfer : MonoBehaviour
{
    public TMP_InputField inputField;
    public AiBehav aiCharacter;

    public TMP_InputField chatInputField;

    public API chatgptAPI;

    public TMP_Text chatResponse;




    public void feedCommands(string apiResponse)
    {
        List<string> pipeLineCommands = processInput(apiResponse);

        //aiCharacters.GetCommand(command);

    foreach (string pipeLineCommand in pipeLineCommands)
        {
            Debug.Log("COMMAND Obj : Extracted content: " + pipeLineCommand);
            aiCharacter.pushCommand(pipeLineCommand);
                    
            Debug.Log("COMMAND Obj : Extracted content: " + pipeLineCommand);
        }
            //aiCharacter.ExecCommand();
        
    }


    public void UseChat()
    {
        string chatInput = chatInputField.text;
        Debug.LogWarning(chatInput);

        chatgptAPI.RequestOpenAIResponse(chatInput, this);

    }

    public void UseCommand()
    {
        string command = inputField.text;

        feedCommands(command);
    }


    public List<string> processInput(string commandLine)
    {
        List<string> resultList = new List<string>(commandLine.Split(',').Select(s => s.Trim()));

        return resultList;
    }

}

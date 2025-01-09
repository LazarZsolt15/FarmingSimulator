using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    public TMP_InputField tmpInputField; 
    public bool visible = false;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        manageInputField(tmpInputField, visible);
    }

    void manageInputField(TMP_InputField inputField, bool visible)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (inputField != null)
            {
                if (inputField.isFocused) // Ellen�rizz�k, hogy az InputField f�kuszban van-e
                {
                    inputField.DeactivateInputField(); // Deaktiv�ljuk, ha f�kuszban van
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); // F�kusz elt�vol�t�sa
                }
                else
                {
                    inputField.Select(); // Kiv�lasztja az InputField-et
                    inputField.ActivateInputField(); // Aktiv�lja a beviteli mez�t
                }
            }
        }
        if (visible)
        {
            inputField.gameObject.SetActive(true);

            //E bet�vel aktiv�ljuk az inputfield-et
            if (Input.GetKeyDown(KeyCode.E))
            {
                ActivateInputField(inputField);
            }

        }
        else
        {
            inputField.gameObject.SetActive(false);
            inputField.text = "";
        }
    }

    void ActivateInputField(TMP_InputField inputField)
    {
        inputField.ActivateInputField();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

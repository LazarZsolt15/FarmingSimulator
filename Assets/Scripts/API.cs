using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

[System.Serializable]
public class Payload
{
    public string model = "gpt-4o-mini";
    public List<Message> messages = new List<Message>();
}

[System.Serializable]
public class OpenAIResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

public class API : MonoBehaviour
{
    private readonly string apiUrl = "https://api.openai.com/v1/chat/completions"; 
    private readonly string apiKey = "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";
    //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
    //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
    //https://api.openai.com/v1/chat/completions
    public string ResponseContent { get; private set; }

    public void RequestOpenAIResponse(string userInput, CommandTransfer agent)
    {
        StartCoroutine(PostRequest(userInput, agent));
    }

    private IEnumerator PostRequest(string userInput, CommandTransfer agent)
    {
        Payload payload = new Payload();
        payload.messages.Add(new Message
        {
            role = "system",
            content = $"A j�t�k sor�n egy karakterrel kommunik�lhatsz, melyet a ChatGPT ir�ny�that. " +
            $"Az ir�ny�t�s a k�vetkez� objektumnevekkel t�rt�nik: C1 (b�za), C2 (v�ztorony), C3 (f�k), C4 (b�za), C5 (bokrok), C6 (bokrok), C7 (f�k), " +
            $"C8 (�res cselekv�s, ha a felhaszn�l� �ltal be�rt sz�veg nem �rtelmes vagy kontextusa nem megfelel�). A karaktert k�l�nb�z� helysz�nek " +
            $"k�z�tt mozgathatod, figyelembe v�ve a j�t�k aktu�lis helyzet�t �s c�ljait. Fontos szab�lyok:\r\n\r\n" +
            $"A bokrok (C5, C6)sz�retel�s�hez sz�ks�g van v�zre. Vizet a v�ztoronyb�l (C2) lehet hozni." +
            $"\r\nA f�khoz (C3, C7) nem sz�ks�ges v�z." +
            $"\r\nHa a felhaszn�l� �ltal k�rt m�k�d�st nem lehet a cselekm�nyekkel megval�s�tani, akkor �res cselekm�nyt (C8) kell visszaadni.\r\n" +
            $"Ha a felhaszn�l� �zenet�nek kontextusa elt�r a j�t�k k�rnyezet�t�l, akkor is �res cselekm�nyt (C8) kell visszaadni.\r\n" +
            $"Az utas�t�saidat vessz�vel elv�lasztottan add meg, �s csak az eml�tett helysz�neket haszn�ld a v�laszban. " +
            $"M�s karaktereket ne alkalmazz a v�laszban, m�s outputot ne adjon a modell, csak a megfelel� karaktersorozatot.\r\n\r\n" +
            
             $"1. **Búzaföldek kezelése**: Ha a felhasználó kéri a búzaföldek kezelését:\r\n" +
            $"Vágd ki a búzát\": Ha nincs megadva, hogy melyiket, válassz véletlenszerűen a C1 (közelebbi) vagy a C4 (távolabbi) közül.\r\n" +
            $"Vágd ki a közelebbi búzát\": Válaszd a C1-et.\r\n" +
            $"Vágd ki a távolabbi búzát\": Válaszd a C4-et.\r\n\r\n" +

            $"2. **Fák kezelése**: Ha a felhasználó kéri a fák kezelését:\r\n" +
            $"Vágd ki a fákat\": Ha nincs megadva, hogy melyiket, válassz véletlenszerűen a C7 (közelebbi) vagy a C3 (távolabbi) közül.\r\n" +
            $"Vágd ki a közelebbi fákat\": Válaszd a C7-et.\r\n" +
            $"Vágd ki a távolabbi fákat\": Válaszd a C3-at.\r\n\r\n" +

            $"3. **Bokrok locsolása**: Ha a felhasználó kéri a bokrok locsolását:\r\n" +
            $"Locsold meg a bokrokat\": Először menj a víztoronyhoz (C2), majd válassz véletlenszerűen a C6 (közelebbi) vagy a C5 (távolabbi) közül.\r\n" +
            $"Locsold meg a közelebbi bokrot\": Először menj a víztoronyhoz (C2), majd válaszd a C6-ot.\r\n" +
            $"Locsold meg a távolabbi bokrot\": Először menj a víztoronyhoz (C2), majd válaszd a C5-öt.\r\n" +
            $"A bokrok locsolásához mindig két célpont van: a víztorony (C2) és a megfelelő bokor (C5 vagy C6).\r\n\r\n" +


            $"4. **Üres cselekmények**:\r\n" +
            $"Ha a felhasználó által kért működést nem lehet a cselekményekkel megvalósítani, akkor \"C8\" értéket adj vissza.\r\n" +
            $"Ha a felhasználó üzenetének kontextusa eltér a játék környezetétől, akkor is \"C8\" értéket adj vissza.\r\n\r\n" +
            
            $"Példák az utasításokra:\r\n\r\n" +
            $"Ha a felhasználó kéri a búzaföldek kezelését (és nincs megadva, melyik): \"C1\" vagy \"C4\"\r\n" +
            $"Ha a felhasználó kéri a közelebbi fákat: \"C7\"\r\n" +
            $"Ha a felhasználó kéri a bokrok locsolását, és nincs víz: \"C2, C6\" vagy \"C2, C5\"\r\n" +
            $"Ha a felhasználó üzenete nem értelmes vagy nem megfelelő kontextusú: \"C8\""

        });
        payload.messages.Add(new Message { role = "user", content = userInput });

        string json = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = UnityWebRequest.Put(apiUrl, json))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                ResponseContent = null; 
            }
            else
            {
                string resultText = request.downloadHandler.text;
                Debug.Log("Raw response: " + resultText);

                OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(resultText);
                if (response.choices != null && response.choices.Length > 0 && response.choices[0].message != null)
                {
                    ResponseContent = response.choices[0].message.content;
                    agent.feedCommands(ResponseContent);
                    Debug.Log("API Obj : Extracted content: " + ResponseContent);
                }
                else
                {
                    Debug.LogWarning("Response JSON structure was not as expected, or was missing data.");
                    ResponseContent = null;
                }
            }
        }
    }
}

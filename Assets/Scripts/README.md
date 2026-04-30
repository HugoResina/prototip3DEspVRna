# Sobre els Scripts

## Scripts de gestió de veu

### [STT (Speech-To-Text)](STT.cs)

Captura l'àudio del micròfon i el transcriu en temps real utilitzant **Vosk** (model offline, per defecte en català).

Carrega el model des de `Assets/VoskModels/vosk-model-small-ca-0.4`.

### [GroqChat](GroqChat.cs)

Gestiona la comunicació amb l'API de Groq per obtenir respostes d'un LLM des del joc. S'utilitza per a passar el resultat de la veu a una IA, per a que aquesta retorni un index de resposta segons el que se li diu.

Carrega la API key des de `Assets/Resources/env.txt`: *GROQ_API_KEY=la_teva_clau*

``` C#
groqChat.SendMessage("Hola!", response => Debug.Log(response));
```

L'arxiu *env.txt* s'ha de crear per a que **GroqChat** pugui funcionar. Per crear-lo segueix aquestes instruccions:
- Crea o inicia secció en el teu compte de [Groq](https://console.groq.com).
- Accedeix a la secció de **API Keys** en la part superior dreta del menú superior de la web.
- Crea una *API Key* nova si no tens cap o s'hi no saps la *key* d'alguna.
- Un cop la crees copia la *key* que et dona, perquè no et deixarà tornar-la a copiar en cap altra ocasió, i guarda-la on sigui.
- Després simplement crea l'arxiu *env.txt* dins de `Assets/Resources/` i afegeix aquesta única línia:
    - GROQ_API_KEY=la_teva_clau
- Canvia *la_teva_clau* per la *key* que has obtinguit.

### [GroqObjs](GroqObjs.cs)

Defineix les estructures de dades per serialitzar/deserialitzar les peticions i respostes de l'API de Groq amb *JsonUtility*.

### [TTS (Text-To-Speech)](TTS.cs)

Converteix text a àudio utilitzant el model **MMS (Meta Massively Multilingual Speech)** via Unity Inference Engine (CPU).

Carrega el model des de `Assets/MMS-TTS Models/mms_tts_cat.onnx`.

## Scripts de gestió de les missions

Aquest són els scripts per a la gestió de les missións.

### [MissionLoader](MissionLoader.cs)

Carrega els arxius Json i els transforma en Objectes utilitzables per codi. A través del *id* d'una missió s'obté el seu Json que es deserialitza en un Objecte utilitzable pel codi.

``` C#
public static MissionData Load(string missionId)
{
    string filePath = $"{_jsonDirPath}/{missionId}.json";
    string json = System.IO.File.ReadAllText(filePath);

    return JsonConvert.DeserializeObject<MissionData>(json);
}
```

### [MissionManager](MissionManager.cs)

*Core* de cada missió, gestiona cada missió, així com tots els seus passos i decisions. Tots els scripts es communiquen a traves del **MissionManager**.

### [FlagSystem](FlagSystem.cs)

Sistema de *flags* per comrpovar i tenir un registre de totes les decisions que ha pres el jugador al llarg de la missió. S'utilitza també per a calcular el final segons els *flags* que estiguin actius.

``` C#
public void SetFlag(string key, bool value) => _boolFlags[key] = value;
public bool GetFlag(string key) => _boolFlags.TryGetValue(key, out var v) && v;
```

### [FreeRoamHandler](FreeRoamHandler.cs)

Gestiona els passos de tipus "*freeroam*". Activa i desactiva objectes o la interacció amb personatges per l'escena corresponents al pas actual dins de la missió. A través de tots els **MissionGameObject** registrats dins de l'escena, comprova quins apareixen en cada pas i activa o desactiva en funció d'això. Segons el tipus de *gameObject* de missió activa el propi *gameObject* o un script dins del *gameObject*.

``` C#
if (obj.TryGetComponent(out MissionTrigger _) || obj.TryGetComponent(out MissionWalkie _))
{
    obj.gameObject.SetActive(false);
}
else if (obj.TryGetComponent(out MissionInteractablePerson person))
{
    person.enabled = false;
}
```

### [DialogueHandler](DialogueHandler.cs)

Gestiona els passos de tipus "*dialeg*". Mostre per pantalla els textos del dialèg així com els butons de les respostes al dialeg. Es communica amb el **UIManager** a través d'events per actualitzar la UI per mostrar les converses.

### [MissionGameObject](MissionGameObject.cs)

*MonoBehaviour* que s'afegeix als *gameObject* que han d'activar-se o desactivar-se segons la missió.

``` C#
public void OnDecisionMade()
{
    MissionManager.Instance.OnDecisionMade(decision);
}
```

### [MissionInteractablePerson](MissionInteractablePerson.cs)

MonoBehaviour que hereta de **MissionGameObject** i **IIteractable** utilitzat per als elements interactuables dins de l'escena. Aquest a diferència de **MissionGameObject**, el **FreeRoamHandler** no activa o desactiva el propi *gameObject*, sino que activa o desactiva el component de **MissionInteractablePerson** per a que es pugui veure l'objecte interactuable, però no es pugui interactuar amb ell. Per prendre la decisió només s'ha d'interactuar amb el *gameObject* que conté aquest component.

``` C#
public void Interact(GameObject interactor)
{
    //[...]
    OnDecisionMade();
}
```

### [MissionTrigger](MissionTrigger.cs)

MonoBehaviour que hereta de **MissionGameObject**, en el qual les decisons es prenen entrant en la seva area de col·lisió de tipus *trigger*.

``` C#
private void OnTriggerEnter(Collider other)
{
    if (other.gameObject.TryGetComponent(out PlayerInteraction _))
    {
        OnDecisionMade();
    }
}
```

### [MissionWalkie](MissionWalkie.cs)

MonoBehaviour que al igual que **MissionInteractablePerson** hereta de **MissionGameObject** i **IIteractable**, aquest és utilitzar per a les communicacions per walkie. Per prendre la decisió només s'ha d'interactuar un cop sigui activat.

``` C#
public void Interact(GameObject interactor)
{
    //[...]
    OnDecisionMade();
}
```

### [DecisionsLogic](DecisionLogic.cs)

Gestiona la lògica que desencadena realitzar una decisió. El seu funcionament consisteix en, apartir del *id* de la decisió s'executa amb un *switch* el comportament desitjat a través d'un event estàtic.

``` C#
public static class DecisionLogic
{
    //[...]
    public static event Action ActivateABDEvent;
    //[...]

    public static void Execute(string decisionId)
    {
        switch (decisionId)
        {
            //[...]
            case "d_tornar_muntar_ABD":

                    ActivateABDEvent?.Invoke();
                    break;
            //[...]
        }
    }
}
```

### [EndingCalculator](EndingCalculator.cs)

Calcula quin final és el resultant segons els *flags* actius per el [FlagSystem](FlagSystem.cs).


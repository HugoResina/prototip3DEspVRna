# Sobre els Scripts

## Scripts de gestió de veu



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


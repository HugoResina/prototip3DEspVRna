# ESCENES

- EstacioTest
    - Escena inicial, en una estació de bombers, hauria de funcionar com una mena de hub previ a la simulació com a tal, la idea és que puguis interactuar amb props per l'escenari i que a l'interactuar amb el bomber que hi ha a l'escena puguis carregues la missió que vulguis.

- MovementScene
    - Escena per fer probes de moviment, interacció, dialegs... 
- nav mesh navigation test
    - Escena principal de la simulació de la fuita d'amoníac


## nav mesh navigation Test
### Espai per explicar una mica per sobre els elements en l'escena
- GameManager i UIManger:
    - scripts per gestionar ascpectes del joc i la UI
- SphereWaypoints
    - Conté els objectes que es fan servir en els behaviours dels npcs per asignar destins
- EventSystem 
    - Propi de unity, imprescindible per que funcionin els events d'input
- almacen
    - model de la escena principal
- player
    - conte tota la logica pertinent al player
- NavMesh Surface
    - estableix una mesh per la que els npcs amb un navmeshagent es poden moure esquivant obstacles asignats
- abd
    - ABD, objecte de l'Area Basica de Descontaminació, ha de estar desactivat al principi i s'activa en un punt en concret per script
- smoke
    - sistema de particules que representa el fum de la fuita de quimics
- capMagatzem
    - model que representa el npc del cap de l'empresa
- policies 
    - objecte que conte els npc policia, en aquest punt no fan res
- vehicles
    - conte tots els models dels vehicles que van apareixent
- cap bombers
    - NPC amb qui se suposa que interactues
- Bomber 1 / Bomber 2
    - Npcs que entren a l'edifici quan s'ha de rescatar a la victima
- camilla
    - objecte amb totes les animacions de treure a la victima amb la llitera, s'activa a la que els bombers 1 i 2 entren al edifici
- Groq
    - gestiona el procesament de la ia a traves de l'api
- limits entrada edifici navmesh, cube 1, cube 2
    - objectes fets servir per delimitar el navmesh de l'entrada de l'edifici, no activar si no es vol fer un nou bake del navmesh
- parte 1
    - zona inicial a on hi ha una cinematica introductoria i un espai de temps per comunicarse amb la estació i preguntar sobre la situació de la missió
- workerCry
    - npc que representa a un treballador afectat per la fuita amb els ulls escolits pel quimic
- acortinador
    - objecte que conte un efecte de particules que representa l'acortinador fet servir per controlar el nubol de gas
- missionWalkies
    - conte els objectes necesaris per la part dels dialegs amb els walkietalkies
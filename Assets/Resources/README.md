# Sobre els Json de missions

**<span style="color:red">CADA MISSIÓ REQUEREIX EL SEU PROPI ARXIU JSON</span>**

## Propietats

- **missionId**: Identificador de la missió. Aquest indentificador ha de coincidir amb el nom del arxiu.
- **title**: Títol llegible de la missió.
- **description**: Descripció de la missió.
- **entry**: Primer *step*  de la missió.
- **steps**: Array que conté el cos de la missió, així com tots els camins a seguir des del prncipi fins al final de la missió.
    - **stepId**: Identificador del pas, sempre comença per "*step_*" per a fàcil identificació.
    - **type**: Tipus de pas. Actualment hi ha dos tipus:
        - "*freeroam*": Es refereix a que el jugador es pot moure i interactuar durant aquest pas.
        - "*dialogue*": Es refereix a que el jugador esta en un diàleg amb un altre personatge, normalment format per exchanges en comptes de decisions (veure més endavant).
    - **objectiveTitle**: Títol llegible del pas. Títol del problema que planteja aquest pas, visible pel jugador.
    - **objectiveText**: Descripció llegible del pas. Descripció més detallada amb el que planteja el títol del pas, també visible pel jugador.
    - **speaker**: Nom del personatge que ha de parlar. Només utilitzat per a passos de tipus "*dialogue*".
    - **activate**: Array de noms d'objectes que s'activaràn durant aquest pas.
    - **decisions**: Array de decision que pot prendre el jugador al llarg d'aquest camí. Cada decisió pot portar a una altre decisió, o a un altre pas.
        - **decisionId**: Identificador de la decisió, sempre comença per "*d_*" per a fàcil identificació.
        - **triggeredBy**: Objecte dins de l'array de "**activate**" que ha activat aquesta decisió.
        - **requires**: Array de flags que es requereixen per a que aquesta decisió es pugui mostrar (Veure "**effects**" per a l'estructura d'un "*flag*").
        - **effects**: Array de flags que activa realitzar aquesta decisió.
            - **flag**: Identificador del flag.
            - **value**: Valor per al flag.
        - **next**: Decisió següent a aquesta.
    - **exchanges**: Array que conté la conversa, formada per exchanges. Només utilitzat per a passos de tipus "*dialogue*".
        - **exchangeId**: Identificador del diàleg. Sempre començat per "*exchange_*". Sempre ha d'haver un dialèg amb identificador "**exchange_root**", que representa el dialèg principal o arrel des d'on comença el diàleg complet.
        - **text**: Text que diu el personatge.
        - **decisions**: Array de decisions per a aquest dialèg. Té una estructura semblant a "**decisions**" (vist anteriorment), però específicament per dialègs. Es mosntren al jugador en format de text.
            - **decisionId**: Veure "**decisions**" dins de "**steps**", explicat més amunt.
            - **label**: Text de la decisió.
            - **requires**: Veure "**decisions**" dins de "**steps**", explicat més amunt.
            - **effects**: Veure "**decisions**" dins de "**steps**", explicat més amunt.
            - **next**: A diferència del "**next**" de "**decisions**" dins de "**steps**", aquest pot portar a un nou pas o a un altre dialèg. Si "**next**" comença per "*exchange_*" portarà a un altre dialèg dins de la mateixa conversa, i si comença per "*step_*" portarà a un altre pas.


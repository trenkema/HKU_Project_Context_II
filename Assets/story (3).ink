EXTERNAL isTalking(npcName)
EXTERNAL startQuest(questId)
EXTERNAL completeQuest(questId)

=== piet ===
~isTalking("piet")
Hallo ik ben Piet
* [Doei piet]
ok doei.
-> DONE
* [Hallo Piet, mag ik een quest van jou?]
~startQuest(0)
Hier een leuke quest1
-> DONE
=== klaas ===
~isTalking("klaas")
Hallo ik ben klaas
* [Hallo Klaas, heb jij een drone quest voor mij?]
    ~startQuest(1)
    JA!
    -> DONE
* [Doei klaas]
    Nouja zeg
    -> DONE
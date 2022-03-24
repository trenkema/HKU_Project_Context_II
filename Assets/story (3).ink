-> mom
EXTERNAL isTalking(npcName)
//Fallback function for test inside inky
=== function isTalking(npcName) ===
~return npcName
EXTERNAL startQuest(questId)
//Fallback function for test inside inky
=== function startQuest(questId) ===
~return questId
EXTERNAL completeQuest(questId)
//Fallback function for test inside inky
=== function completeQuest(questId) ===
~return questId

VAR woodChopped = false
VAR placedPillars = false
VAR talkedToFarmer = false

=== mom ===
~isTalking("mom")
Ola Rosa, I’ve heard that there are a few residents that need help. 
Would you like to use my boat to see if you could help them?
*[Of course mom! I’d love to discover and help the community!]
Great! But remember to be back at 17:30 for dinner!
-> DONE

=== forester ===
~isTalking("forester")
Hi there! What can I do for you?
*[Can I help you with something?]
    Oh yes! Thank you for asking. There are some sick trees in the forest that need to be chopped down quickly! I got an axe for you if you’d like to help.
    ** [Sure, I would love to!]
    -> DONE
+[Do you know anyone who I can help?]
    {
    - woodChopped: -> helpbuilder
    - !talkedToFarmer: -> talktofarmer
    - talkedToFarmer || woodChopped: -> dideverything
    }
+[Goodbye]
Bye!
-> DONE

= dideverything
    Not at the moment no.
    +[Alright, thanks for your time.]
    -> DONE
= talktofarmer
    I’ve heard that our local farmer needed some help. Maybe you can check in on him?
    +[Alright, thanks for your time.]
    -> DONE
= helpbuilder
    You could ask if this wood can be used for the Ecoduct that the Builder is building!
    +[Alright, thanks for your time.]
    -> DONE

-> DONE
=== builder ===
~isTalking("builder")
{ -placedPillars: -> goodjob} 
Hi! How can I help you? -> options
= options
+[What are you building?]
    After the water level started rising, the population of foxes was split into two due to the water barrier. 
    ++[Continue]
    I’m building an Ecoduct for the fox populations on the two islands, so they can meet again.
    +++[Continue]
    Is there anything else I can help you with? -> options
+[Can I help with something?]
    Do you know where I can find some pillars?
    ** {woodChopped && !placedPillars} [I’ve chopped some sick trees, I could place them for you if you want?]
    I’d love that! The foxes and the community will be proud of you!
    -> DONE
    ++ {!woodChopped} [I have no clue.]
    Oh. -> options
+[Goodbye!]
Bye!
-> DONE
= goodjob
Thank you so much for placing the pillars!

-> DONE
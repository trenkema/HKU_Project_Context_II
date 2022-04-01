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
VAR pumpedWater = false
VAR talkedToFarmer = false

=== mom ===
{
   - !intro: -> intro
   - else: -> fallback
}

= intro
~isTalking("mom")
Ola Rosa, I’ve heard that there are a few residents that need help.
*[Continue]
Would you like to use my boat to see if you could help them?
**[Of course mom! I’d love to discover and help the community!]
Great! But remember to be back at 17:30 for dinner!
***[Yes Mom!]
And don't forget to regularly check your tablet with [TAB] for information!
-> DONE

= fallback
Remember to be back before dinner!
+[I will!]
Goodbye!
-> DONE

=== forester ===
~isTalking("forester")
Hi there! What can I do for you?
*[Can I help you with something?]
    Oh yes! Thank you for asking. There are some sick trees in the forest that need to be chopped down quickly!
     **[Continue]
    The sick trees are easily recognisable, they have no leaves and are pale.
    ***[Continue]
    I got an axe for you if you’d like to help, toggle your axe with [Q].
    **** [Sure, I would love to!]
    Thank you so much!
    ~ startQuest(2)
    -> DONE
+[Do you know anyone who I can help?]
    {
    - woodChopped: -> helpbuilder
    - !pumpedWater: -> talktofarmer
    - pumpedWater || woodChopped: -> dideverything
    }
* {woodChopped} [I chopped the trees you asked for!]
Whew, those trees were on the verge of infecting all tress around!
**[Continue]
Thanks to you the forest is saved!
-> DONE
+[Goodbye]
Bye!
-> DONE

= dideverything
    Not at the moment no.
    +[Alright, thanks for your time.]
    Sorry, maybe later?
    -> DONE
= talktofarmer
    I’ve heard that our local farmer needed some help. Maybe you can check in on him?
    +[I will, thanks for your time.]
    No problem!
    -> DONE
= helpbuilder
    You could ask if this wood can be used for the Ecoduct that the Builder is building!
    +[I will, thanks for your time.]
    You're welcome!
    -> DONE

-> DONE
=== builder ===
~isTalking("builder")
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
    ++ {woodChopped && !placedPillars} [I’ve chopped some sick trees, I could place them for you if you want?]
    I’d love that! The foxes and the community will be proud of you!
        ~ startQuest(0)
    -> DONE
    ++ {!woodChopped} [I have no clue.]
    Oh. -> options
* {placedPillars} [I placed the pillars you asked for!]
I couldn't have done it without you!
**[Continue]
Can you help me vegetate the ecoduct? I have a drone you can use.
***[Sounds fun! I'll do it!]
Thank you!
~ startQuest(1)
-> DONE
+[Goodbye!]
Bye!
-> DONE

=== farmer ===
~isTalking("farmer")
{ 
-!intro: ->intro
 - else: ->options
}

= intro
Hey kiddo, I've been havin' some trouble getting this piece 'o land dry.
*[Continue]
I've been pumpin' and pumpin' but I ain't strong enough. Can you lend me a hand?
**[Sure!, what can I do?]
Go over yonder and give that old pump a few tugs, that oughta do it.
***[I'll try my best.]
Thank you.
~ startQuest(3)
-> DONE

= options
Can I help you with something kiddo?
* [No.]
Oh, see you later then!
-> DONE
* {pumpedWater} [I've pumped all the water!]
You did?! That's great news!
-> DONE

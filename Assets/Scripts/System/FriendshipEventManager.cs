using UnityEngine;

/// <summary>
/// Manages tier 100 friendship events — one per NPC, triggered automatically
/// the first time the player talks to them at relationship 100.
/// Uses StoryDialogueUI for the moment (no choices — just a single profound beat
/// with a follow-up dismissed by X).
/// </summary>
public class FriendshipEventManager : MonoBehaviour
{
    public static FriendshipEventManager Instance { get; private set; }

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Public API — called from NPC.OpenDialogue before normal dialogue
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Returns true if a friendship event should fire for this NPC.
    /// </summary>
    public bool ShouldFire (string npcName, int relationship)
    {
        if (GameState.Instance == null) return false;
        if (relationship < 100) return false;
        return !GameState.Instance.HasFriendshipEventFired (npcName);
    }

    /// <summary>
    /// Fires the friendship event for the given NPC.
    /// </summary>
    public void Fire (string npcName, NPC npc)
    {
        if (GameState.Instance == null) return;
        GameState.Instance.SetFriendshipEventFired (npcName);
        FireEvent (npcName, npc);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Events
    // ══════════════════════════════════════════════════════════════════════

    private void FireEvent (string npcName, NPC npc)
    {
        switch (npcName)
        {
            case "Nikias":    FireNikias ();    break;
            case "Lydia":     FireLydia ();     break;
            case "Chloe":     FireChloe ();     break;
            case "Argos":     FireArgos ();     break;
            case "Eudoros":   FireEudoros ();   break;
            case "Phaedra":   FirePhaedra ();   break;
            case "Demetrios": FireDemetrios (); break;
            case "Theron":    FireTheron ();    break;
            case "Kallias":   FireKallias ();   break;
            case "Miriam":    FireMiriam ();    break;
            case "Stephanos": FireStephanos (); break;
            case "Xanthos":   FireXanthos ();   break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Main 6
    // ══════════════════════════════════════════════════════════════════════

    private void FireNikias ()
    {
        GameState.Instance.AddHonour (5);
        GameState.Instance.ChangeRelationship ("Lydia", 5);

        StoryDialogueUI.Instance.Open (
            "Nikias",
            "Nikias stops you in a quiet moment away from the Agora crowd. He has the careful look of a man who has rehearsed what he is about to say. \"I have known many people in this city,\" he begins. \"Business partners. Rivals. Acquaintances who called themselves friends.\" He pauses. \"You are the only person outside my blood I have ever truly trusted.\" He meets your eyes directly. \"If something were to happen to me — and I am not a young man — would you be willing to be named as guardian to Lydia?\"",
            "\"It would be an honour.\"",
            "\"I'm not sure I'm worthy of that.\"",
            () => {
                GameState.Instance.ChangeRelationship ("Nikias", 5);
                GameState.Instance.ChangeRelationship ("Lydia", 8);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "Nikias exhales slowly. \"Good,\" he says. \"Then it is decided.\" He grips your hand briefly — not like a merchant sealing a deal, but like a father trusting someone with something irreplaceable. Lydia does not know yet. But she will.");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Nikias", 8);
                GameState.Instance.AddHonour (3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "Nikias looks at you for a long moment. Then he laughs — quietly, genuinely. \"That is exactly why I am asking you,\" he says. \"A man who knows his own limits is worth ten who don't.\" He grips your hand. \"You are worthy. I have watched you long enough to know.\"");
            }
        );
    }

    private void FireLydia ()
    {
        StoryDialogueUI.Instance.Open (
            "Lydia",
            "Lydia approaches you with something tucked carefully under her arm — a small rolled papyrus, tied with a piece of wool thread. She holds it out. \"I finished it,\" she says. Her voice is steady but her hands are not quite. \"It took me four months. I want you to read it and tell me honestly — not kindly. Honestly — what you think.\" She watches your face as you unroll it. The writing is careful and clear, the argument winding but sound. It is about whether a life lived quietly can be as worthy as one lived loudly. It is, unmistakably, about her.",
            "\"This is genuinely good, Lydia.\"",
            "\"It needs work, but the thinking is yours.\"",
            () => {
                GameState.Instance.ChangeRelationship ("Lydia", 8);
                GameState.Instance.AddHonour (3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "She is very still for a moment. Then she takes the scroll back and holds it against her chest. \"Thank you,\" she says quietly. \"I needed someone to say that who I believed.\" She looks at you with something that has no easy name. \"You have no idea what this means to me.\"");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Lydia", 10);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "She nods slowly, eyes bright. \"Good,\" she says. \"Tell me what needs work.\" You do. She listens without flinching, asks sharp questions, pushes back twice where you are wrong. By the end you are both laughing. \"This is the best conversation I have had in months,\" she says. \"Maybe ever.\"");
            }
        );
    }

    private void FireChloe ()
    {
        GameState.Instance.AddHonour (3);

        StoryDialogueUI.Instance.Open (
            "Chloe",
            "Chloe appears beside you without warning — which is her way — and says simply: \"Come with me. I want to show you something.\" She leads you through a part of the city you barely know, up a winding path behind the Kerameikos, to a flat outcropping of rock that overlooks the whole of Athens. The Acropolis from this angle is extraordinary. The city hums below you. \"I found this when I was twelve,\" she says. \"I have never brought anyone here before.\" She sits down and looks out. \"I thought you should see it.\"",
            "Sit beside her in silence.",
            "\"Why me?\"",
            () => {
                GameState.Instance.ChangeRelationship ("Chloe", 8);
                GameState.Instance.AddHonour (3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You sit. The city spreads out below you. Neither of you speaks for a long time. Eventually she says, very quietly: \"This is the only place in Athens where I feel like myself.\" A pause. \"I am glad you are here.\" You stay until the light changes.");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Chloe", 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "She turns and looks at you with that direct gaze of hers. \"Because you are the only person I have met who I think actually sees things,\" she says. \"Most people look at Athens and see what they want. You look at it and see what's there.\" She turns back to the view. \"That is rarer than you know.\"");
            }
        );
    }

    private void FireArgos ()
    {
        GameState.Instance.AddDrachma (0f); // No drachma reward — this is about meaning
        GameState.Instance.AddHonour (5);

        StoryDialogueUI.Instance.Open (
            "Argos",
            "Argos is waiting for you outside his workshop with something wrapped in cloth on the bench beside him. When you approach, he unwraps it without ceremony. It is the pot — the one you saw him make, the finest thing you have ever seen produced in that workshop. He holds it out to you. \"Take it,\" he says. \"I made it for you.\" You start to protest. He shakes his head. \"I have been making things for forty years,\" he says. \"This is the best one. It belongs with someone who understands what things cost to make well.\"",
            "Accept it with both hands.",
            "\"I can't take something this valuable.\"",
            () => {
                GameState.Instance.ChangeRelationship ("Argos", 8);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You take it carefully. It is heavier than it looks. Argos watches you hold it and something in his face relaxes — as if he has been waiting a long time to give something to someone who would hold it exactly like that. \"Good,\" he says. He goes back inside. That is all. It is enough.");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Argos", 10);
                GameState.Instance.AddHonour (3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "Argos looks at you for a long moment. Then he puts the pot back on the bench and folds his hands. \"Nothing is too valuable to give to the right person,\" he says quietly. \"I have spent forty years learning that. Take it.\" His voice is not loud. It does not need to be. You take it.");
            }
        );
    }

    private void FireEudoros ()
    {
        GameState.Instance.ChangeFavour (GameState.PatronGod.Athena, 8);
        GameState.Instance.AddHonour (5);

        StoryDialogueUI.Instance.Open (
            "Eudoros",
            "Eudoros finds you in the morning with a look you have not seen on him before — something between peace and purpose. \"I need your help with something,\" he says. \"Come to the Acropolis with me.\" He carries the small marble figure of Thalia wrapped carefully in cloth. At the base of one of the lesser columns, in a recess that catches the afternoon light, he sets her down. \"This is where she should be,\" he says. \"Where the light touches her every day.\" He steps back. \"I need a witness. Someone who knew why she matters.\"",
            "\"I'll remember her.\"",
            "Stand in silence beside him.",
            () => {
                GameState.Instance.ChangeRelationship ("Eudoros", 8);
                GameState.Instance.AddHonour (3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "\"Then she will last,\" he says. He does not cry — he is too old and too tired for tears — but something in his posture changes, as if a weight has been properly placed at last. He puts his hand briefly on your shoulder. \"You are a good person,\" he says. \"I do not say that lightly.\"");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Eudoros", 10);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You stand beside him as the afternoon light moves across the stone and finds Thalia's face. Neither of you speaks. The city goes about its business far below. After a long time, Eudoros says: \"She would have liked you.\" It is the best thing he has ever said to you. You both know it.");
            }
        );
    }

    private void FirePhaedra ()
    {
        GameState.Instance.ChangeFavour (GameState.Instance.patronGod, 10);
        GameState.Instance.AddHonour (5);

        StoryDialogueUI.Instance.Open (
            "Phaedra",
            "Phaedra meets you at the altar at an hour when no one else is present. She is in her full priestly vestments and her expression is composed in a way that suggests deliberate ceremony. \"I am going to perform a blessing for you,\" she says. \"Not a public one. A private one, reserved for people the gods have marked.\" She pauses. \"I have performed this twice in my life. Once for my teacher. Once for myself.\" She looks at you steadily. \"You are the third. Stand still.\"",
            "Stand still and receive it.",
            "\"Are you certain I deserve this?\"",
            () => {
                GameState.Instance.ChangeFavour (GameState.Instance.patronGod, 5);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "The words are ancient and she speaks them quietly. When it is finished she steps back. The air feels different — or perhaps you do. \"The gods know your name now,\" she says. \"In a way they did not before.\" She does not explain further. She does not need to.");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Phaedra", 8);
                GameState.Instance.AddHonour (5);
                GameState.Instance.ChangeFavour (GameState.Instance.patronGod, 5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "Phaedra regards you for a long moment. \"That question,\" she says finally, \"is why you do.\" She begins the blessing. Her voice is low and precise. When it is finished she says: \"The gods do not mark the certain ones. They mark the ones who ask.\"");
            }
        );
    }

    // ══════════════════════════════════════════════════════════════════════
    // Secondary 6
    // ══════════════════════════════════════════════════════════════════════

    private void FireDemetrios ()
    {
        GameState.Instance.AddHonour (8);

        StoryDialogueUI.Instance.Open (
            "Demetrios",
            "Demetrios sends word that you are to stand beside him at the military review at the Gymnasium — an honour normally reserved for officers and council members. When you arrive he positions you at his right hand without explanation. As his soldiers march past in formation, he turns to the nearest captain and says simply: \"This man. Remember him. Athens can trust him.\" The captain nods. So do the others. You are being introduced to an army.",
            "Stand straight and acknowledge the soldiers.",
            "Ask him why he did this.",
            () => {
                GameState.Instance.ChangeRelationship ("Demetrios", 8);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You hold yourself well. The soldiers look at you — assessing, as soldiers do — and most of them nod. Demetrios says nothing more. He does not need to. You have just been vouched for by the city's general in front of his entire command. Athens will feel different after today.");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Demetrios", 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "\"Because someone should,\" he says, without looking at you. \"And because you have earned it without asking for it.\" He pauses. \"The ones who ask are usually the wrong ones.\" The review continues. You do not ask anything else. You understand.");
            }
        );
    }

    private void FireTheron ()
    {
        GameState.Instance.AddCareerXP (30);
        GameState.Instance.AddHonour (5);

        StoryDialogueUI.Instance.Open (
            "Theron",
            "Theron appears at your door — or rather, intercepts you on your morning walk — with a sealed invitation in his hand. \"My symposium,\" he says. \"Tonight. You are invited.\" He says it simply, as though it is not significant. It is very significant. His symposium is attended by philosophers, generals, poets, and the most interesting minds in Athens. You have heard of it for years. \"The others will have opinions about you,\" he says. \"Good ones, I think. I have prepared them.\"",
            "\"I am honoured.\"",
            "\"What have you told them about me?\"",
            () => {
                GameState.Instance.ChangeRelationship ("Theron", 8);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "That evening you take your place among people whose names you have heard for years. They receive you as an equal. Theron watches from across the room with something that might be pride. You stay until the lamps burn low, and the conversation is the finest you have ever had.");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Theron", 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "\"The truth,\" he says. \"Which I find is sufficient.\" That evening, you discover what the truth apparently is: they have been expecting you. The philosopher beside you says Theron has spoken of you for months. \"He does not do that,\" she says, \"for everyone.\"");
            }
        );
    }

    private void FireKallias ()
    {
        GameState.Instance.AddDrachma (30f);
        GameState.Instance.AddHonour (5);

        StoryDialogueUI.Instance.Open (
            "Kallias",
            "Kallias hosts a dinner at his estate — intimate, perhaps twenty guests — and spends most of it moving you around the room like a chess piece, introducing you to each person with deliberate care. Ship owners. Council members. A visiting dignitary from Corinth. Each introduction is warm and specific: not \"this is my friend\" but \"this is someone you should know, and here is why.\" By the end of the evening you have made more useful connections than you have in years. As the guests leave, Kallias takes you aside. \"My house is always open to you,\" he says. \"Not as a guest. As a member of this network.\"",
            "\"You have given me something remarkable.\"",
            "\"Why have you done all this for me?\"",
            () => {
                GameState.Instance.ChangeRelationship ("Kallias", 8);
                GameState.Instance.AddHonour (3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "Kallias smiles — the real one, not the social one. \"Athens rewards those who invest wisely,\" he says. \"I invested in you early. Tonight was the return.\" He refills your cup. \"There will be more.\"");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Kallias", 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "He considers the question seriously. \"Because you are the kind of person this city needs more of,\" he says finally. \"And because helping people like you is the most interesting thing I have found to do with money.\" He means it. That is the most surprising part.");
            }
        );
    }

    private void FireMiriam ()
    {
        GameState.Instance.AddDrachma (40f);
        GameState.Instance.AddHonour (5);

        StoryDialogueUI.Instance.Open (
            "Miriam",
            "Miriam hands you a folded letter at the harbour — sealed with wax, addressed in a script you don't recognise. \"Open it,\" she says. Inside is a letter of commendation, written in three languages, addressed to her trading partners from Alexandria to Ephesus. It describes you in terms that are both precise and generous — your character, your reliability, your judgment. \"I have kept records of every dealing I have ever had with you,\" she says. \"I decided recently that you deserved to know what those records say.\"",
            "\"I don't know what to say.\"",
            "\"You kept records of me?\"",
            () => {
                GameState.Instance.ChangeRelationship ("Miriam", 8);
                GameState.Instance.AddHonour (3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "\"Then say nothing,\" she says, not unkindly. \"The letter speaks for itself. Every port between here and Alexandria will know your name now.\" She turns back to her cargo. \"You have earned it. That is all there is to say.\"");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Miriam", 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "\"I keep records of everyone I deal with,\" she says. \"It is how I survive in a city that is not mine.\" A pause. \"Yours are the only ones I have ever decided to share.\" She says it simply, without sentiment. Which makes it mean more.");
            }
        );
    }

    private void FireStephanos ()
    {
        GameState.Instance.AddHonour (8);

        StoryDialogueUI.Instance.Open (
            "Stephanos",
            "Stephanos finds you alone — which is unusual for him — and says, without his usual performance: \"I need to tell you something.\" He sits down. \"In the years I have known you, I have had opportunities to say things about you. Things that would have been useful to me. Interesting to others.\" He pauses. \"I never said any of them.\" He looks at you directly. \"I need you to know that. Not because I want credit. Because I want you to understand that there is one person in this city whose name I have never put in a rumour. Yours.\"",
            "\"That means more than you know.\"",
            "\"Why tell me now?\"",
            () => {
                GameState.Instance.ChangeRelationship ("Stephanos", 8);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "He nods, once. \"Good.\" He stands up and something in his posture shifts back toward his usual self — lighter, more guarded. But you have seen the other version now. You will not forget it. \"Don't make it sentimental,\" he says, and walks away.");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Stephanos", 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "He thinks about this. \"Because you are about to reach the kind of standing in this city where people will start trying to use your name without asking,\" he says. \"I wanted you to know that I am not one of them. Before it becomes relevant.\" He stands. \"Also because I am capable of doing the right thing occasionally. Don't get used to it.\"");
            }
        );
    }

    private void FireXanthos ()
    {
        GameState.Instance.AddDrachma (20f);
        GameState.Instance.AddHonour (5);

        StoryDialogueUI.Instance.Open (
            "Xanthos",
            "Xanthos waves you over at the harbour with the unhurried authority of a man who has managed this waterfront for twenty years. He shows you a ledger — a column of names, berths, rates, priority designations. He points to a line near the top. Your name. \"Permanent priority berth,\" he says. \"Best position in the harbour. Protected from weather, first in and first out.\" He closes the ledger. \"For any ships you own now or in future.\" He says it matter-of-factly, as though it is the simplest thing. \"The harbour looks after its own,\" he says. \"You are one of ours now.\"",
            "\"I won't forget this.\"",
            "\"What did I do to earn this?\"",
            () => {
                GameState.Instance.ChangeRelationship ("Xanthos", 8);
                GameState.Instance.AddHonour (3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "\"See that you don't,\" he says, but there is warmth in it. He returns to his work immediately, as is his way. But as you leave the harbour you notice three of his workers nod at you — differently than before. The harbour knows. The harbour remembers.");
            },
            () => {
                GameState.Instance.ChangeRelationship ("Xanthos", 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "He looks up from his ledger. \"You treated my people well,\" he says. \"You remembered their names. You didn't complain about the weather or the fees or the wait.\" He pauses. \"And you once helped one of my dock workers when you didn't have to.\" He closes the ledger. \"I notice things. It is my job.\"");
            }
        );
    }
}
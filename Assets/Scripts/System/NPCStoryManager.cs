using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages NPC story beats. Each NPC has 3 beats at relationship 20, 50, 80.
/// Beat indices: Nikias 0-2, Lydia 3-5, Chloe 6-8, Argos 9-11, Eudoros 12-14, Phaedra 15-17
/// </summary>
public class NPCStoryManager : MonoBehaviour
{
    public static NPCStoryManager Instance { get; private set; }

    // ── Beat Data ──────────────────────────────────────────────────────────
    public struct StoryBeat
    {
        public string dialogue;
        public string choiceA;
        public string choiceB;
        public System.Action onChoiceA;
        public System.Action onChoiceB;
    }

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Public API
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Returns the beat index that should fire for this NPC at the current
    /// relationship level, or -1 if no beat is pending.
    /// </summary>
    public int GetPendingBeat (string npcName, int relationship)
    {
        int baseIndex = GetBaseIndex (npcName);
        if (baseIndex < 0) return -1;

        // Beat 0 fires at 20, beat 1 at 50, beat 2 at 80
        int[] thresholds = { 20, 50, 80 };

        for (int i = 2; i >= 0; i--)
        {
            int beatIndex = baseIndex + i;
            if (relationship >= thresholds[i]
                && !GameState.Instance.HasStoryBeatFired (beatIndex))
                return beatIndex;
        }
        return -1;
    }

    /// <summary>
    /// Returns the StoryBeat data for a given beat index.
    /// </summary>
    public StoryBeat GetBeat (int beatIndex)
    {
        switch (beatIndex)
        {
            // ── NIKIAS ──────────────────────────────────────────────────────
            case 0: return new StoryBeat
            {
                dialogue = "Nikias pauses mid-sentence and looks at you with an appraising eye. \"You strike me as someone who pays attention,\" he says quietly. \"I like that. Most people in this city see only what they want to see.\" He laughs softly, but there is something strained beneath it.",
                choiceA  = "\"What are you not seeing, Nikias?\"",
                choiceB  = "\"Athens rewards the observant.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Nikias", 3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("He meets your gaze for a moment longer than is comfortable. \"Perhaps more than I would like,\" he admits. \"But that is a conversation for another time.\" He changes the subject smoothly. You note it.");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Nikias", 2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("\"Indeed it does,\" he agrees, back to the merchant's smile. \"And punishes the careless.\" Something in his tone suggests he is speaking from recent experience.");
                }
            };

            case 1: return new StoryBeat
            {
                dialogue = "Nikias draws you aside from the crowd. His voice is low. \"I am going to trust you with something because I believe you have proven yourself.\" He exhales. \"I owe a significant debt to a man named Krateros. He is not a patient man. I have until the end of the season.\"",
                choiceA  = "\"How much do you need?\"",
                choiceB  = "\"What does Krateros want if you cannot pay?\"",
                onChoiceA = () => {
                    GameState.Instance.nikiasToldAboutDebt = true;
                    GameState.Instance.ChangeRelationship ("Nikias", 5);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Nikias looks genuinely moved. \"Two hundred drachma. I am not asking you for it — only telling you.\" He grips your arm briefly. \"But knowing you would ask... that means something.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.nikiasToldAboutDebt = true;
                    GameState.Instance.ChangeRelationship ("Nikias", 3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("His jaw tightens. \"My ship. The one Lydia was born on.\" He looks away. \"I will not let that happen. But I wanted someone to know, in case things go poorly.\"");
                }
            };

            case 2: return new StoryBeat
            {
                dialogue = GameState.Instance.nikiasToldAboutDebt
                    ? "Nikias finds you before you find him. His face is older than you remember. \"Krateros has moved the deadline forward. I have one week.\" He looks at you steadily. \"I will not ask for money. But I need to know — if the worst happens, will you look after Lydia?\""
                    : "Nikias stops you in the street. He looks tired. \"I owe you an apology,\" he says. \"I have been... struggling. I did not want to say. But I think you have sensed it.\" He pauses. \"Would you help me if I asked?\"",
                choiceA  = "\"You have my word.\"",
                choiceB  = "\"I cannot make that promise.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Nikias", 8);
                    GameState.Instance.ChangeRelationship ("Lydia", 5);
                    GameState.Instance.AddHonour (3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Something in Nikias visibly releases. \"Then I can face this,\" he says quietly. \"Thank you. Whatever Athens thinks of me, I know what kind of man you are.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.nikiasBetrayedByPlayer = true;
                    GameState.Instance.ChangeRelationship ("Nikias", -5);
                    GameState.Instance.ChangeRelationship ("Lydia", -3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Nikias nods slowly. \"I understand,\" he says, in a tone that makes clear he does not. He walks away without another word. You have the sense something between you has closed permanently.");
                }
            };

            // ── LYDIA ────────────────────────────────────────────────────────
            case 3: return new StoryBeat
            {
                dialogue = "Lydia falls into step beside you and says, without preamble: \"Do you think a woman can be wise?\" She is watching your face carefully. \"Not clever. Not charming. Wise. The way Socrates was wise.\"",
                choiceA  = "\"Absolutely. Wisdom has no gender.\"",
                choiceB  = "\"Athens does not tend to think so.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Lydia", 5);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Her whole face changes. \"Good,\" she says simply. \"I was not sure about you.\" She smiles then — not the careful social smile, but something unguarded. \"I am glad I was wrong to be uncertain.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Lydia", 1);
                    StoryDialogueUI.Instance?.ShowFollowUp ("\"Athens,\" she says carefully, \"is often wrong.\" She does not seem angry, only thoughtful. \"I appreciate that you answered honestly.\" She moves on, but glances back once.");
                }
            };

            case 4: return new StoryBeat
            {
                dialogue = "Lydia pulls you aside near the Agora. She is holding a small scroll, half-hidden in her shawl. \"I have been teaching myself to read,\" she whispers. \"Father does not know. He would say it is not appropriate for a woman.\" She looks at you, asking without asking.",
                choiceA  = "\"That is remarkable. Keep going.\"",
                choiceB  = "\"Your father may have a point.\"",
                onChoiceA = () => {
                    GameState.Instance.lydiaAmbitionSupported = true;
                    GameState.Instance.ChangeRelationship ("Lydia", 7);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She lets out a slow breath, as though she has been holding it for months. \"I have not told anyone,\" she says. \"Thank you.\" She tucks the scroll away carefully. Something in how she looks at you afterward is different — more open.");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Lydia", -2);
                    GameState.Instance.ChangeRelationship ("Nikias", 2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Her expression closes like a door. \"I see,\" she says quietly. She puts the scroll away. \"I will not mention it again.\" She does not, and for a while she is noticeably more guarded with you.");
                }
            };

            case 5: return new StoryBeat
            {
                dialogue = GameState.Instance.lydiaAmbitionSupported
                    ? "Lydia finds you alone. She is carrying more scrolls than before. \"I have been thinking about what you said,\" she begins. \"If I were free to choose — truly free — I think I would want to teach. Teach children, perhaps. Is that foolish?\""
                    : "Lydia approaches you quietly. \"I owe you honesty,\" she says. \"I was hurt before. But I think I was wrong to assume the worst.\" She hesitates. \"Can I ask you something? What do you think I should do with my life?\"",
                choiceA  = "\"It is not foolish. Athens needs teachers.\"",
                choiceB  = "\"You should speak to your father first.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Lydia", 8);
                    GameState.Instance.AddHonour (2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("\"You always say the right thing,\" she says softly. Then: \"No. That is not fair. You mean it, and that is different.\" She looks at you for a long moment. \"I hope you know that you have changed things for me.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Lydia", 3);
                    GameState.Instance.ChangeRelationship ("Nikias", 3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She nods slowly. \"You are probably right.\" She looks down at her hands. \"I am not sure I am ready for that conversation. But you are right.\" She seems to settle something within herself.");
                }
            };

            // ── CHLOE ────────────────────────────────────────────────────────
            case 6: return new StoryBeat
            {
                dialogue = "Chloe is sitting alone outside the Agora, watching people pass. When she notices you, she tilts her head. \"You know what I notice about you? You actually listen when people talk. Most people in Athens are just waiting for their turn to speak.\"",
                choiceA  = "\"You seemed worth listening to.\"",
                choiceB  = "\"Most conversations are worth hearing.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Chloe", 5);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She laughs — a real laugh, not the polished one she uses with most people. \"Careful,\" she says. \"Flattery like that could get you in trouble.\" But she is clearly pleased, in spite of herself.");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Chloe", 3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("\"Spoken like a philosopher,\" she says, not unkindly. \"But you are deflecting.\" She gives you a long look. \"I will find out what you actually think eventually.\"");
                }
            };

            case 7: return new StoryBeat
            {
                dialogue = "Chloe is quieter than usual. When you sit with her, she says after a long pause: \"My mother died when I was four. I barely remember her face.\" She is not crying — she is very controlled. \"Father never speaks of her. I think it hurts him too much. But sometimes I wonder what she was like.\"",
                choiceA  = "\"She must have been remarkable to raise someone like you.\"",
                choiceB  = "\"Have you ever asked Argos directly?\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Chloe", 7);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She is quiet for a long time. Then: \"That was kind.\" Her voice is a little unsteady. \"I do not usually let people say things like that to me.\" She looks away, then back. \"Thank you.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Chloe", 4);
                    GameState.Instance.ChangeRelationship ("Argos", 2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("\"Once,\" she says. \"He cried. I have not asked again.\" She is thoughtful rather than sad. \"Maybe someday. Not yet.\" She glances at you. \"You give practical advice. I respect that.\"");
                }
            };

            case 8: return new StoryBeat
            {
                dialogue = GameState.Instance.chloeFeltSeen
                    ? "Chloe stops you in the street with an unusually direct look. \"I want to ask you something and I want an honest answer.\" A pause. \"Do you see me as a person? Not a potter's daughter. Not a pretty face. A person with thoughts of her own.\""
                    : "Chloe approaches you, and there is something deliberate in how she does it. \"I owe you a proper conversation,\" she says. \"I have been unfair to you. Can I ask you something directly?\" She does not wait for permission. \"What do you actually think of me?\"",
                choiceA  = "\"Yes. That is exactly how I see you.\"",
                choiceB  = "\"You are many things. I am still learning which.\"",
                onChoiceA = () => {
                    GameState.Instance.chloeFeltSeen = true;
                    GameState.Instance.ChangeRelationship ("Chloe", 8);
                    GameState.Instance.AddHonour (2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Something in her face softens in a way you have not seen before. \"Good,\" she says. \"That is all I needed to know.\" She holds your gaze for a moment. \"You should know that is rarer than you think.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Chloe", 4);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She considers this. \"That is a careful answer,\" she says. \"But I think it is honest.\" She nods slowly. \"I can work with honest.\" She seems satisfied, if not entirely won over.");
                }
            };

            // ── ARGOS ────────────────────────────────────────────────────────
            case 9: return new StoryBeat
            {
                dialogue = "Argos is working clay at the wheel when you approach, and he does not stop as he speaks. \"You know what this teaches you?\" he asks, nodding at the clay. \"That you can force a thing or you can guide it. Force never works. Not with clay. Not with people.\" He finally looks up. \"Which do you do?\"",
                choiceA  = "\"I try to guide.\"",
                choiceB  = "\"Sometimes you have to force.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Argos", 4);
                    StoryDialogueUI.Instance?.ShowFollowUp ("He holds your gaze for a moment, then nods. \"Good answer.\" He returns to his work. \"Come back. I would like to talk more.\" From Argos, this is high praise.");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Argos", 2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("He does not argue. \"Sometimes,\" he agrees. \"But you remember the things you forced. They never sit right.\" He shapes the clay in silence. \"Think on it.\"");
                }
            };

            case 10: return new StoryBeat
            {
                dialogue = "Argos puts down his tools and sits heavily. \"I was wealthy once,\" he says, without preamble. \"A merchant. Not unlike Nikias.\" A long pause. \"I made bad decisions. Trusted the wrong people. Lost everything.\" He looks at his hands. \"I came to pottery because it was honest work. You cannot lie to clay.\"",
                choiceA  = "\"That took courage.\"",
                choiceB  = "\"Do you miss who you were?\"",
                onChoiceA = () => {
                    GameState.Instance.argosRespected = true;
                    GameState.Instance.ChangeRelationship ("Argos", 6);
                    StoryDialogueUI.Instance?.ShowFollowUp ("He looks at you with something like surprise. \"Most people say it was a fall,\" he says slowly. \"You are the first to call it courage.\" He is quiet a moment. \"Maybe it was. I had not thought of it that way.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Argos", 4);
                    StoryDialogueUI.Instance?.ShowFollowUp ("\"No,\" he says, without hesitation. Then: \"Sometimes I miss the feeling of possibility. But the man I was then — he was not good.\" He picks up his tools again. \"This man is better. Even if he has less.\"");
                }
            };

            case 11: return new StoryBeat
            {
                dialogue = GameState.Instance.argosRespected
                    ? "Argos is waiting for you. He has a finished pot in his hands — simple, perfectly made. \"I made this for you,\" he says. \"No reason. Only that you are the first person in years who made me think differently about my own life.\" He pauses. \"What do you think defines a man's worth?\""
                    : "Argos approaches you slowly. \"I have been thinking about what we spoke of,\" he says. \"I think I owe you more honesty.\" He looks at the city around him. \"What do you think defines a man's worth? I want to know what you actually believe.\"",
                choiceA  = "\"What he builds and leaves behind.\"",
                choiceB  = "\"How he treats the people around him.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Argos", 6);
                    GameState.Instance.AddHonour (3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Argos turns the pot in his hands. \"Yes,\" he says quietly. \"I think so too.\" He hands it to you. \"Then take this. Something I built. Something to leave behind.\" It is one of the finest objects you have ever held.");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Argos", 7);
                    GameState.Instance.ChangeRelationship ("Chloe", 3);
                    GameState.Instance.AddHonour (2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Something in Argos's face opens. \"Chloe's mother used to say that,\" he says softly. He is quiet for a long time. \"I think you are right. I think I knew it then and forgot.\" He grips your arm briefly. \"Thank you.\"");
                }
            };

            // ── EUDOROS ──────────────────────────────────────────────────────
            case 12: return new StoryBeat
            {
                dialogue = "Eudoros pauses in his work and looks at you with the steady gaze of a man who has lived long enough to stop performing. \"I knew Pericles,\" he says simply. \"Before Athens was what it became. Before the Parthenon.\" He gestures vaguely upward. \"Do you know what he said when they first showed him the plans?\"",
                choiceA  = "\"What did he say?\"",
                choiceB  = "\"What was he like as a man?\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Eudoros", 4);
                    StoryDialogueUI.Instance?.ShowFollowUp ("\"He said: 'Make it so that generations who never knew us will grieve that they missed us.'\" Eudoros shakes his head slowly. \"We were young. We believed we could do it.\" He pauses. \"I think we did.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Eudoros", 5);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Eudoros is quiet for a moment. \"Difficult,\" he says finally. \"Brilliant. Lonely, I think, though he would never have admitted it.\" He smiles faintly. \"Most great men are. The vision outpaces the company.\"");
                }
            };

            case 13: return new StoryBeat
            {
                dialogue = "Eudoros has been working on something he keeps covered when others pass. Today he shows you — a small marble figure, rough still, but unmistakably a woman. Young. Laughing. \"Her name was Thalia,\" he says quietly. \"She died in the plague. Thirty years ago.\" He looks at the figure. \"I never finished her.\"",
                choiceA  = "\"Why are you finishing her now?\"",
                choiceB  = "\"She was someone you loved.\"",
                onChoiceA = () => {
                    GameState.Instance.eudorosSharedPain = true;
                    GameState.Instance.ChangeRelationship ("Eudoros", 6);
                    StoryDialogueUI.Instance?.ShowFollowUp ("\"Because I am old enough now to understand that unfinished things do not wait,\" he says. \"They haunt.\" He covers the figure again gently. \"I think you understand that. You have the look of someone who finishes things.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.eudorosSharedPain = true;
                    GameState.Instance.ChangeRelationship ("Eudoros", 7);
                    StoryDialogueUI.Instance?.ShowFollowUp ("He does not answer immediately. \"Yes,\" he says at last. \"In the way that changes what you are.\" He traces the marble's face lightly with one finger. \"You are the first person I have shown this to. I am not sure why I trust you. I am glad I do.\"");
                }
            };

            case 14: return new StoryBeat
            {
                dialogue = GameState.Instance.eudorosSharedPain
                    ? "Eudoros meets you at the Acropolis at dusk. The figure of Thalia is finished — small, perfect, glowing in the last light. He places it carefully against the base of a column. \"I thought she should be here,\" he says. \"Where beauty lives.\" He turns to you. \"Was it worth the pain it cost?\""
                    : "Eudoros is sitting very still when you find him, staring at his hands. \"I have been thinking about beauty,\" he says slowly. \"Whether it is worth what it costs.\" He looks up. \"The Parthenon cost three lives in its construction. Was it worth it?\"",
                choiceA  = "\"Yes. Beauty outlasts grief.\"",
                choiceB  = "\"Only the people who survived can answer that.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Eudoros", 7);
                    GameState.Instance.ChangeFavour (GameState.PatronGod.Apollo, 5);
                    GameState.Instance.AddHonour (3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Eudoros closes his eyes briefly. \"Yes,\" he says. \"I believe that too. I needed to hear someone else say it.\" He is quiet for a moment. \"She would have liked you, I think. She liked people who meant what they said.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Eudoros", 6);
                    GameState.Instance.AddHonour (2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("He looks at you sharply, then slowly nods. \"That is the most honest answer anyone has ever given me,\" he says. \"Most people give me the comfortable answer.\" He turns back to the city. \"You are good, you know. At being a person.\"");
                }
            };

            // ── PHAEDRA ──────────────────────────────────────────────────────
            case 15: return new StoryBeat
            {
                dialogue = "Phaedra is standing very still at the edge of the Acropolis, looking out over the city. When she hears you approach, she does not turn. \"Do you pray?\" she asks. \"Not at altars. Just — to whatever is there. In the quiet.\"",
                choiceA  = "\"Sometimes. I am not sure anyone hears.\"",
                choiceB  = "\"I try to. I think it matters.\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Phaedra", 4);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She turns then, and looks at you with an expression you cannot quite read. \"I appreciate that,\" she says carefully. \"The honesty.\" A pause. \"I have been having the same doubt lately. I did not expect to hear it from someone else.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Phaedra", 3);
                    GameState.Instance.ChangeFavour (GameState.Instance.patronGod, 3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She finally turns, and there is something relieved in her face. \"Good,\" she says softly. \"I needed to hear that someone still believes it.\" She does not explain why she needed it. You do not ask.");
                }
            };

            case 16: return new StoryBeat
            {
                dialogue = "Phaedra sits beside you uninvited, which she has never done before. \"I am going to tell you something I have not said aloud,\" she begins. \"I have not felt the gods' presence in four months.\" Her voice is steady but her hands are tight in her lap. \"I perform the rites. I say the words. But I feel — nothing.\"",
                choiceA  = "\"Maybe the gods are there even when you cannot feel them.\"",
                choiceB  = "\"What does that feel like?\"",
                onChoiceA = () => {
                    GameState.Instance.ChangeRelationship ("Phaedra", 6);
                    GameState.Instance.ChangeFavour (GameState.PatronGod.Athena, 5);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She is very still. Then: \"That is what I keep telling myself.\" She exhales. \"It is harder to believe than to say. But you saying it — it helps. Somehow.\" She straightens. \"Thank you. I mean that.\"");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Phaedra", 7);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She seems surprised by the question. Most people would have given her reassurance. \"Empty,\" she says after a pause. \"Like speaking into a room you thought was occupied.\" She looks at you. \"You are the first person to ask that instead of answering for the gods.\"");
                }
            };

            case 17: return new StoryBeat
            {
                dialogue = GameState.Instance.phaedraFaithRestored
                    ? "Phaedra finds you at the altar. She is calm in a way she has not been for months. \"It came back,\" she says simply. \"This morning. I was praying and — it came back.\" She pauses. \"I think talking to you helped. You reminded me that doubt is not the same as absence.\""
                    : "Phaedra approaches you slowly. She looks tired but not broken. \"I have been thinking about what you said,\" she begins. \"About the gods.\" She looks at her hands. \"I want to ask you something. What do you believe happens when we die?\"",
                choiceA  = "\"I believe the gods remember us.\"",
                choiceB  = "\"I believe the people we touched remember us.\"",
                onChoiceA = () => {
                    GameState.Instance.phaedraFaithRestored = true;
                    GameState.Instance.ChangeRelationship ("Phaedra", 8);
                    GameState.Instance.ChangeFavour (GameState.PatronGod.Athena, 8);
                    GameState.Instance.AddHonour (3);
                    StoryDialogueUI.Instance?.ShowFollowUp ("Something settles in Phaedra's face. \"Then every prayer matters,\" she says softly. \"Even the ones that feel empty.\" She meets your eyes. \"I am glad we met. I think the gods arranged it.\" She says it simply, without performance. You believe she means it.");
                },
                onChoiceB = () => {
                    GameState.Instance.ChangeRelationship ("Phaedra", 7);
                    GameState.Instance.AddHonour (2);
                    StoryDialogueUI.Instance?.ShowFollowUp ("She is quiet for a long time. Then: \"That is a kind of faith too,\" she says. \"Maybe the truest kind.\" She looks at you with something like peace. \"You have given me more to think about than I expected. I do not say that to many people.\"");
                }
            };

            default:
                return new StoryBeat { dialogue = "", choiceA = "", choiceB = "" };
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    private int GetBaseIndex (string npcName)
    {
        switch (npcName)
        {
            case "Nikias":  return 0;
            case "Lydia":   return 3;
            case "Chloe":   return 6;
            case "Argos":   return 9;
            case "Eudoros": return 12;
            case "Phaedra": return 15;
            default:        return -1;
        }
    }
}
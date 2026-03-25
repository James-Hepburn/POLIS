using UnityEngine;

/// <summary>
/// Manages god trials — one per god, triggered immediately when favour hits 80.
/// Called directly from GameState.ChangeFavour via CheckForTrial().
/// Uses StoryDialogueUI for the choice moment.
/// </summary>
public class GodTrialManager : MonoBehaviour
{
    public static GodTrialManager Instance { get; private set; }

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Called from GameState.ChangeFavour immediately after favour changes
    // ══════════════════════════════════════════════════════════════════════

    public void CheckForTrial (GameState.PatronGod god, int newFavour)
    {
        if (GameState.Instance == null) return;
        if (newFavour < 80) return;
        if (GameState.Instance.HasTrialFired (god)) return;

        GameState.Instance.SetTrialFired (god);
        FireTrial (god);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Fire a trial
    // ══════════════════════════════════════════════════════════════════════

    private void FireTrial (GameState.PatronGod god)
    {
        if (StoryDialogueUI.Instance == null) return;

        switch (god)
        {
            case GameState.PatronGod.Hermes:     FireHermesTrial ();     break;
            case GameState.PatronGod.Ares:       FireAresTrial ();       break;
            case GameState.PatronGod.Aphrodite:  FireAphroditeTrial ();  break;
            case GameState.PatronGod.Apollo:     FireApolloTrial ();     break;
            case GameState.PatronGod.Hephaestus: FireHephaestusTrial (); break;
            case GameState.PatronGod.Athena:     FireAthenaTrial ();     break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Hermes — The Stranger's Deal
    // ══════════════════════════════════════════════════════════════════════

    private void FireHermesTrial ()
    {
        StoryDialogueUI.Instance.Open (
            "Hermes Tests You",
            "A stranger intercepts you at the edge of the Agora. His eyes are quick and his smile is older than his face. He holds out a sealed contract — a trade opportunity that seems almost impossibly favourable. \"One risk,\" he says. \"One chance. The gods reward those who move.\" Something in you recognises this moment as more than it appears.",
            "Take the deal.",
            "Walk away.",
            () => {
                GameState.Instance.AddDrachma (100f);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Hermes, 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You sign without reading the small print. That evening you open the sealed amphora and find it full of silver coins. The stranger is nowhere to be found. Hermes laughs somewhere you cannot hear. (+100 drachma, +10 Hermes favour)");
            },
            () => {
                GameState.Instance.AddHonour (8);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Hermes, 5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You shake your head and walk on. Behind you, you hear the stranger laugh quietly. \"Wise,\" he says. \"Or cowardly. Hard to tell.\" But there is approval in his voice. Athens speaks well of those who cannot be rushed. (+8 honour, +5 Hermes favour)");
            }
        );
    }

    // ══════════════════════════════════════════════════════════════════════
    // Ares — The Soldier's Test
    // ══════════════════════════════════════════════════════════════════════

    private void FireAresTrial ()
    {
        StoryDialogueUI.Instance.Open (
            "Ares Watches",
            "Near the Gymnasium you see it — a soldier, still in partial armour, shoving a merchant against a wall. The merchant is older and clearly afraid. A small crowd has gathered but nobody moves. The soldier is drunk and armed. Your eyes meet the merchant's across the square.",
            "Step in and confront the soldier.",
            "Report it to the city guard.",
            () => {
                GameState.Instance.AddHonour (10);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Ares, 10);
                GameState.Instance.ChangeRelationship ("Demetrios", 10);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You step forward. The soldier turns on you instead — and sees something in your face that makes him reconsider. He leaves. The merchant grips your arm with both hands. The crowd disperses quietly. You feel something settle in your chest that you have no word for. (+10 honour, +10 Ares favour, +10 Demetrios)");
            },
            () => {
                GameState.Instance.AddHonour (5);
                GameState.Instance.ChangeRelationship ("Demetrios", 15);
                GameState.Instance.ChangeRelationship ("Theron", 5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You find a guard and report what you saw. By the time they arrive the soldier is gone, but the merchant remembers your name. Theron, who witnessed everything, nods at you the next day in the Agora. You did the right thing, even if it was the slower one. (+5 honour, +15 Demetrios, +5 Theron)");
            }
        );
    }

    // ══════════════════════════════════════════════════════════════════════
    // Aphrodite — The Love Letter
    // ══════════════════════════════════════════════════════════════════════

    private void FireAphroditeTrial ()
    {
        StoryDialogueUI.Instance.Open (
            "Aphrodite Smiles",
            "A young woman stops you near the Theatre — flushed, nervous, beautiful. She holds out a sealed letter. \"I cannot deliver this myself,\" she says. \"Please. Take it to Eudoros. He will understand.\" She presses it into your hands before you can respond and disappears into the crowd. The letter is sealed but not wax-bound. It would be easy to read.",
            "Deliver it sealed and unread.",
            "Read it before delivering.",
            () => {
                GameState.Instance.ChangeFavour (GameState.PatronGod.Aphrodite, 10);
                GameState.Instance.ChangeRelationship ("Eudoros", 10);
                GameState.Instance.ChangeRelationship ("Lydia", 8);
                GameState.Instance.ChangeRelationship ("Chloe", 8);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You find Eudoros and hand him the letter without a word. He reads it slowly, then folds it with great care. \"You did not read it,\" he says. It is not a question. \"No,\" you say. He looks at you a long time. \"Good. There are fewer of you than there used to be.\" (+10 Aphrodite favour, +10 Eudoros, +8 Lydia, +8 Chloe, +5 honour)");
            },
            () => {
                GameState.Instance.ChangeFavour (GameState.PatronGod.Aphrodite, 3);
                GameState.Instance.ChangeRelationship ("Eudoros", 5);
                GameState.Instance.AddHonour (-3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "The words inside are private and moving — a confession you were not meant to read. You deliver it anyway and say nothing. Eudoros thanks you. But something in how you carry yourself the rest of the day is different. You know something you were not supposed to know. (+3 Aphrodite favour, +5 Eudoros, -3 honour)");
            }
        );
    }

    // ══════════════════════════════════════════════════════════════════════
    // Apollo — The Debate Judgement
    // ══════════════════════════════════════════════════════════════════════

    private void FireApolloTrial ()
    {
        StoryDialogueUI.Instance.Open (
            "Apollo Illuminates",
            "You are called upon unexpectedly to judge a public debate in the Agora — two philosophers arguing about whether courage is a virtue or a performance. One argues brilliantly but unpopularly. The other says what the crowd wants to hear. They look to you. The crowd leans in. This is the kind of moment that travels.",
            "Judge on the argument's merit.",
            "Rule for the crowd favourite.",
            () => {
                GameState.Instance.AddHonour (10);
                GameState.Instance.AddCareerXP (30);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Apollo, 10);
                GameState.Instance.ChangeRelationship ("Eudoros", 8);
                GameState.Instance.ChangeRelationship ("Nikias", 5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You rule for the unpopular philosopher. The crowd mutters. Then, slowly, someone begins to applaud. Then another. By the time you leave the Agora, people are repeating your name. Not because you pleased them — because you were right and they know it. (+10 honour, +30 career XP, +10 Apollo favour)");
            },
            () => {
                GameState.Instance.AddDrachma (40f);
                GameState.Instance.AddCareerXP (10);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Apollo, 3);
                GameState.Instance.AddHonour (-3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "The crowd cheers. The losing philosopher looks at you with something close to pity. Three merchants press coins into your hand. You count them walking home and try not to think about the argument you just dismissed. It was the better one. (+40 drachma, -3 honour, +3 Apollo favour)");
            }
        );
    }

    // ══════════════════════════════════════════════════════════════════════
    // Hephaestus — The Craftsman's Commission
    // ══════════════════════════════════════════════════════════════════════

    private void FireHephaestusTrial ()
    {
        StoryDialogueUI.Instance.Open (
            "Hephaestus Guides",
            "Your tools fail at the worst possible moment — a critical piece of work ruined before your eyes. Before you can curse your luck, Argos appears beside you. \"I saw that,\" he says quietly. \"I can fix your tools tonight if you help me finish a commission first. Client needs it by morning.\" He does not wait for your answer. He just starts walking toward his workshop.",
            "Help Argos finish the commission.",
            "Pay someone else to fix your tools.",
            () => {
                GameState.Instance.ChangeRelationship ("Argos", 12);
                GameState.Instance.AddCareerXP (25);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Hephaestus, 10);
                GameState.Instance.AddHonour (5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You work beside Argos until well past midnight. The commission is extraordinary. He fixes your tools without ceremony and sends you home. The next morning they work better than they ever have. Some things you cannot pay for. (+12 Argos, +25 career XP, +10 Hephaestus favour, +5 honour)");
            },
            () => {
                GameState.Instance.SpendDrachma (30f);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Hephaestus, 3);
                GameState.Instance.ChangeRelationship ("Argos", -3);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You pay a smith across the Agora to fix your tools. They are serviceable. Argos says nothing when you see him next, but something in how he looks at you has changed — not angry, just a door that was briefly open and is now closed again. (-30 drachma, -3 Argos, +3 Hephaestus favour)");
            }
        );
    }

    // ══════════════════════════════════════════════════════════════════════
    // Athena — The Civic Question
    // ══════════════════════════════════════════════════════════════════════

    private void FireAthenaTrial ()
    {
        StoryDialogueUI.Instance.Open (
            "Athena Takes Notice",
            "A city councillor stops you in the Agora and asks your opinion publicly on a contentious proposal — a new tax on foreign merchants that would benefit Athenian traders but damage the city's reputation for fairness. Half the crowd supports it. Half opposes it. Everyone is watching. The councillor waits.",
            "Speak honestly — oppose the tax.",
            "Say what the crowd wants to hear.",
            () => {
                GameState.Instance.AddHonour (12);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Athena, 12);
                GameState.Instance.ChangeRelationship ("Miriam", 10);
                GameState.Instance.ChangeRelationship ("Kallias", 8);
                GameState.Instance.ChangeRelationship ("Nikias", 5);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "\"Athens was built on fair dealing,\" you say. \"A city that cheats its guests cheats itself.\" Half the crowd goes quiet. The other half nods slowly. The councillor writes something down. Your name is in a record somewhere now. You hope it is the right one. (+12 honour, +12 Athena favour, relationships improved)");
            },
            () => {
                GameState.Instance.AddDrachma (50f);
                GameState.Instance.AddHonour (-5);
                GameState.Instance.ChangeFavour (GameState.PatronGod.Athena, 3);
                GameState.Instance.ChangeRelationship ("Miriam", -8);
                StoryDialogueUI.Instance.ShowFollowUp (
                    "You side with the crowd. They cheer. Miriam, the foreign trader, watches from the edge of the crowd with an expression you will not forget. Athens rewarded you today. You are not sure it should have. (+50 drachma, -5 honour, -8 Miriam, +3 Athena favour)");
            }
        );
    }
}
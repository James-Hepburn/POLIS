using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class WikiManager : MonoBehaviour
{
    [Header ("Navigation")]
    public Button backButton;
    public Button npcButton;
    public Button godsButton;
    public Button festivalsButton;
    public Button professionsButton;
    public Button worldButton;

    [Header ("Content Panel")]
    public TextMeshProUGUI contentTitle;
    public TextMeshProUGUI contentBody;
    public ScrollRect      contentScroll;

    [Header ("Scene Index")]
    public int mainMenuSceneIndex = 1;

    // ══════════════════════════════════════════════════════════════════════
    private void Start ()
    {
        backButton.onClick.AddListener         (OnBack);
        npcButton.onClick.AddListener          (() => ShowSection (Section.NPCs));
        godsButton.onClick.AddListener         (() => ShowSection (Section.Gods));
        festivalsButton.onClick.AddListener    (() => ShowSection (Section.Festivals));
        professionsButton.onClick.AddListener  (() => ShowSection (Section.Professions));
        worldButton.onClick.AddListener        (() => ShowSection (Section.World));

        ShowSection (Section.NPCs);
    }

    private void OnBack ()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToScene (mainMenuSceneIndex);
        else
            SceneManager.LoadScene (mainMenuSceneIndex);
    }

    // ══════════════════════════════════════════════════════════════════════
    private enum Section { NPCs, Gods, Festivals, Professions, World }

    private void ShowSection (Section section)
    {
        switch (section)
        {
            case Section.NPCs:         ShowNPCs ();         break;
            case Section.Gods:         ShowGods ();         break;
            case Section.Festivals:    ShowFestivals ();    break;
            case Section.Professions:  ShowProfessions ();  break;
            case Section.World:        ShowWorld ();        break;
        }

        StartCoroutine (RebuildAfterFrame ());
    }

    private IEnumerator RebuildAfterFrame ()
    {
        yield return null; // wait one frame
        Canvas.ForceUpdateCanvases ();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate (
            contentBody.rectTransform);
        if (contentScroll != null)
            contentScroll.verticalNormalizedPosition = 1f;
    }

    private void SetContentHeight (float height)
    {
        RectTransform rt = contentBody.rectTransform;
        rt.sizeDelta = new Vector2 (rt.sizeDelta.x, height);
        if (contentScroll != null)
            contentScroll.verticalNormalizedPosition = 1f;
    }

    // ══════════════════════════════════════════════════════════════════════
    // NPC Section
    // ══════════════════════════════════════════════════════════════════════

    private void ShowNPCs ()
    {
        contentTitle.text = "Citizens of Athens";
        contentBody.text =
@"EUDOROS — The Sculptor

A weathered craftsman whose hands have shaped marble since boyhood. He tends the Acropolis with quiet reverence, believing beauty is an offering to the gods.

Relationship tip: Speak to him near the Acropolis for the warmest reception.

Daily life: Splits his time between the Acropolis and the streets of Athens.

───────────────────────────

NIKIAS — The Merchant Father

A prosperous trader with sharp eyes and a sharper mind. Father to Lydia. He respects those who demonstrate wealth and honour before asking anything of him.

Relationship tip: Build your drachma and reputation before approaching him about Lydia.

Daily life: Visits the Agora and Harbour to manage his trade interests.

───────────────────────────

DEMETRIOS — The Veteran

A former soldier turned city elder. He carries old wounds with dignity and judges character by deeds, not words.

Relationship tip: A military profession earns his respect fastest.

Daily life: Frequents the Gymnasium to watch the youth train.

───────────────────────────

THERON — The Philosopher

Quick-witted and always questioning. He believes wisdom is Athens' greatest export and enjoys spirited debate with anyone willing.

Relationship tip: A Philosopher profession opens deeper conversation.

Daily life: Found at the Gymnasium and Agora, where ideas flow freely.

───────────────────────────

CHLOE — The Weaver's Daughter

Bright and independent, Chloe has opinions on everything and shares them freely. Father Argos keeps a watchful eye.

Romance: Can be courted once your honour and relationship are strong enough.

Daily life: Moves between the streets of Athens and the Agora.

───────────────────────────

XANTHOS — The Athlete

Young, ambitious, and obsessed with glory at the games. He respects strength and discipline above all else.

Relationship tip: Train often and build your honour to earn his friendship.

Daily life: Spends long hours at the Gymnasium.

───────────────────────────

KALLIAS — The Aristocrat

Old money, old manners. Kallias is polished and pleasant but slow to trust those outside his class. Wealth impresses him.

Relationship tip: A high drachma count and career rank open doors.

Daily life: A regular at the Agora, where status is on display.

───────────────────────────

STEPHANOS — The Gossip

He knows everything about everyone and is not shy about sharing it. His rumours can help or harm your reputation.

Note: Stephanos generates daily gossip that affects your relationships with others. Keep your honour high.

Daily life: Wanders Athens all day, always listening.

───────────────────────────

PHAEDRA — The Priestess

Devoted and serene, Phaedra serves the gods with total conviction. She is slow to befriend those who neglect their divine duties.

Relationship tip: Maintain high favour with at least one god to earn her respect.

Daily life: Divides her time between the Acropolis and the Theatre.

───────────────────────────

MIRIAM — The Foreign Trader

A merchant from distant shores, Miriam brings rare goods and rare perspectives. She values honesty and commercial savvy.

Relationship tip: A Merchant profession and steady drachma impress her.

Daily life: Often found at the Harbour and Agora.

───────────────────────────

ARGOS — The Potter

A quiet, principled craftsman. Father to Chloe. He will not give his daughter to anyone whose character he doubts.

Romance gate: He must approve your betrothal to Chloe. Relationship and honour are everything to him.

Daily life: Works at the Kerameikos pottery district.

───────────────────────────

LYDIA — The Merchant's Daughter

Thoughtful and graceful, Lydia is well-read for her time and quietly ambitious. She takes courtship seriously.

Romance: Can be courted once your honour and relationship with her father Nikias are strong enough.

Daily life: Moves between Athens and the Agora.\n\n\n
";
      SetContentHeight (2670);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Gods Section
    // ══════════════════════════════════════════════════════════════════════

    private void ShowGods ()
    {
        contentTitle.text = "The Gods of Olympus";
        contentBody.text =
@"Divine favour shapes your life in Athens. Pray and offer at the altar to gain favour with your patron god. Neglect the gods and face their displeasure.

Favour decays each day. Keep it high to receive blessings on your work.

At 50 favour, a god may intervene directly — once per deity, once per playthrough.

───────────────────────────

ATHENA — Goddess of Wisdom & War

Domain: Knowledge, craft, strategy.

Profession affinity: Priest

Blessing: Improved earnings and honour from all work.

Displeasure: Reduced honour gains.

Divine intervention: A moment of perfect clarity grants bonus honour and career XP.

───────────────────────────

ZEUS — King of the Gods

Domain: Law, order, sky.

Profession affinity: All

Blessing: Broad bonus to honour gains across all activities.

Displeasure: Honour losses are magnified.

Divine intervention: A thunderous endorsement — large honour boost and improved NPC relationships.

───────────────────────────

HERMES — God of Trade & Luck

Domain: Commerce, travel, messages.

Profession affinity: Merchant

Blessing: Increased drachma from trading.

Displeasure: Reduced drachma earnings.

Divine intervention: A windfall — a significant one-time drachma bonus.

───────────────────────────

ARES — God of War

Domain: Courage, conflict, strength.

Profession affinity: Soldier

Blessing: Improved XP and drachma from military training.

Displeasure: Training yields less reward.

Divine intervention: A surge of martial fury — large XP boost and honour gain.

───────────────────────────

APOLLO — God of the Arts & Sun

Domain: Music, poetry, prophecy, light.

Profession affinity: Philosopher

Blessing: Improved XP and drachma from debate and performance.

Displeasure: Reduced rewards from intellectual work.

Divine intervention: Inspired brilliance — career rank advances and relationships improve.

───────────────────────────

HEPHAESTUS — God of the Forge

Domain: Fire, craftsmanship, metallurgy.

Profession affinity: Craftsman

Blessing: Higher drachma and XP from crafting work.

Displeasure: Work yields less reward.

Divine intervention: A masterwork moment — large drachma bonus and a permanent crafting multiplier.

───────────────────────────

HOW FAVOUR WORKS

Gaining favour: Pray at the altar (small gain, free), offer drachma (larger gain, costs 10–50 drachma).

Decay: Each day, favour with all gods decreases slightly. Stay active at the altar.

Tiers:
  +50 and above — Strong blessing, intervention possible
  +20 to +49    — Mild blessing on relevant work
  -1 to +19     — Neutral, no effect
  -20 and below — Displeasure, penalties apply";
      SetContentHeight (2000);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Festivals Section
    // ══════════════════════════════════════════════════════════════════════

    private void ShowFestivals ()
    {
        contentTitle.text = "Festival Calendar";
        contentBody.text =
@"Athens celebrates eight great festivals across the year. On festival days, NPCs gather at special locations and share unique dialogue. Work is suspended — the city rests and prays.

───────────────────────────

SPRING

CITY DIONYSIA — Early Spring

In honour of Dionysus, god of theatre and wine.

The Theatre comes alive with performance and celebration. Citizens gather to watch and be seen.

Effect: Relationship gains from conversation are doubled.

THARGELIA — Late Spring

A purification festival in honour of Apollo and Artemis.

The city is cleansed of ill omens. Sacred processions wind through the streets.

Effect: Divine favour decay is halted for the day. Offerings yield double favour.

───────────────────────────

SUMMER

PANATHENAIA — Midsummer

The greatest Athenian festival, held in honour of Athena herself.

The Acropolis is the heart of the celebration. Athletic competitions, prayers, and a great procession to the Parthenon.

Effect: All honour gains are doubled. Athena favour increases passively.

HEPHAESTIA — Late Summer

A torch-race festival honouring Hephaestus, god of the forge.

Craftsmen and athletes compete in torch relay races through the city.

Effect: Craftsman earnings are doubled. Hephaestus favour increases passively.

───────────────────────────

AUTUMN

THESMOPHORIA — Early Autumn

A sacred festival in honour of Demeter, goddess of the harvest.

A time of reflection and offering. The city gives thanks for the harvest.

Effect: Drachma from all work is slightly increased for the day.

PYANOPSIA — Late Autumn

A harvest festival honouring Apollo, with offerings of first fruits.

Simple and communal. Families share food and give thanks.

Effect: Relationship gains from conversation are increased.

───────────────────────────

WINTER

LENAIA — Midwinter

A winter festival of theatre in honour of Dionysus.

Smaller than the City Dionysia but beloved. The Theatre hosts intimate performances.

Effect: Philosopher and Priest work yields bonus XP.

HALOA — Late Winter

A festival of Demeter and Dionysus celebrating the first pressing of wine.

A joyful, earthy celebration. Citizens mingle freely and spirits are high.

Effect: All NPC relationship gains are increased for the day.";
      SetContentHeight (2000);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Professions Section
    // ══════════════════════════════════════════════════════════════════════

    private void ShowProfessions ()
    {
        contentTitle.text = "Professions";
        contentBody.text =
@"Choose your path at the start of the game. Your profession determines where you work, how you earn drachma, which god blesses your labour, and what legacy you leave.

Each profession has three career levels. Gain XP by working. Reach level 3 to unlock your pinnacle bonus — a permanent reward that defines your legacy.

───────────────────────────

MERCHANT

Work: Trade at the Agora

Earnings: 8 drachma per session

XP per session: 10

Patron god: Hermes

Career levels:
  Level 1 — Apprentice Trader
  Level 2 — Established Merchant
  Level 3 — Master of Commerce

Pinnacle bonus: All drachma earnings permanently increased by 50%.

───────────────────────────

SOLDIER

Work: Train at the Gymnasium or Harbour

Earnings: 5 drachma per session

XP per session: 15

Patron god: Ares

Career levels:
  Level 1 — Recruit
  Level 2 — Hoplite
  Level 3 — Strategos

Pinnacle bonus: All honour gains permanently doubled.

───────────────────────────

PHILOSOPHER

Work: Debate at the Agora or Gymnasium

Earnings: 6 drachma per session

XP per session: 12

Patron god: Apollo

Career levels:
  Level 1 — Student
  Level 2 — Thinker
  Level 3 — Sage of Athens

Pinnacle bonus: Relationship gains from all conversations permanently increased.

───────────────────────────

CRAFTSMAN

Work: Work at the Kerameikos

Earnings: 9 drachma per session

XP per session: 10

Patron god: Hephaestus

Career levels:
  Level 1 — Apprentice
  Level 2 — Journeyman
  Level 3 — Master Craftsman

Pinnacle bonus: Highest drachma earnings of any profession at level 3.

───────────────────────────

PRIEST

Work: Perform rites at the Acropolis

Earnings: 4 drachma per session

XP per session: 8

Patron god: Athena

Career levels:
  Level 1 — Acolyte
  Level 2 — Priest
  Level 3 — High Priest of Athens

Pinnacle bonus: Each work session grants bonus divine favour to your patron god. Divine favour never decays below 0.

───────────────────────────

CAREER PROGRESSION

XP thresholds are the same for all professions:
  Level 2 unlocked at 100 XP
  Level 3 unlocked at 250 XP

Divine favour affects your earnings. High favour with your patron god increases drachma and XP. Low favour reduces them. Choose a patron god that matches your profession for best results.";
      SetContentHeight (2000);
    }

    // ══════════════════════════════════════════════════════════════════════
    // World Section
    // ══════════════════════════════════════════════════════════════════════

    private void ShowWorld ()
    {
        contentTitle.text = "The World of POLIS";
        contentBody.text =
@"ATHENS, 440 BC

You are a citizen of the greatest city in the ancient world. Athens is at the height of its golden age — the Parthenon gleams new on the Acropolis, the Agora buzzes with philosophy and commerce, and the gods are very much present in daily life.

Your life is your own to shape. Work hard, tend your relationships, honour the gods, and when your time comes, the city will remember you.

───────────────────────────

TIME

The day runs from dawn to dusk. Each activity — working, talking, praying — costs time. When the day ends you must rest.

Time passes in seasons: Spring, Summer, Autumn, Winter.

Each season lasts several days. Years accumulate.

Festival days are marked on the calendar (press C to open).

───────────────────────────

LOCATIONS

THE AGORA — The heart of Athenian public life. Merchants trade, philosophers debate, and citizens gather. Work here as a Merchant or Philosopher.

THE ACROPOLIS — The sacred hill above the city. The Parthenon stands here, dedicated to Athena. Work here as a Priest. Pray at the altar for divine favour.

THE GYMNASIUM — Where Athens trains its body and mind. Athletes, soldiers, and philosophers all pass through. Work here as a Soldier or Philosopher.

THE HARBOUR (PIRAEUS) — Athens' gateway to the sea. Merchants and soldiers find purpose here. Work here as a Soldier or Merchant.

THE KERAMEIKOS — The potters' district, where craftsmen shape clay into art. Work here as a Craftsman.

THE THEATRE — Home to drama, comedy, and the festivals of Dionysus. A place of culture and reflection.

YOUR HOME — Rest here to end the day. At high legacy, open the Legacy panel (press L) to end your story and receive your epitaph.

───────────────────────────

THREE PILLARS

CAREER & WEALTH — Work daily to earn drachma and career XP. Reach level 3 in your profession to unlock your pinnacle bonus. Wealth funds offerings, dowries, and your legacy score.

RELATIONSHIPS & ROMANCE — Talk to citizens daily to build relationships. High relationships unlock deeper dialogue, romance, and marriage. The Gossip Stephanos spreads rumours — keep your honour high.

DIVINE FAVOUR — Pray and offer at the altar. The gods reward devotion and punish neglect. At 50 favour, your patron god may intervene directly — a once-in-a-lifetime blessing.

───────────────────────────

LEGACY SCORING

When you end your life (press L in your home), your legacy is scored across four categories:

  Wealth       — Total drachma accumulated
  Honour       — Total honour points earned
  Relationships — Strength of your friendships and bonds
  Divine Favour — Favour standing with all six gods

Your final score determines your epitaph — the words Athens carves in your memory.";
      SetContentHeight (2000);
    }
}
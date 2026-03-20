using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stephanos generates one rumour per day at end of day.
/// Rumours are based on the player's actual state and affect
/// relationships with relevant NPCs.
/// </summary>
public static class GossipManager
{
    private struct Rumour
    {
        public string                message;
        public string[]              affected;   // NPC names
        public int                   delta;      // relationship change per NPC
    }

    // ══════════════════════════════════════════════════════════════════════
    public static void ProcessDailyGossip ()
    {
        if (GameState.Instance == null) return;

        GameState gs = GameState.Instance;
        List<Rumour> candidates = new List<Rumour> ();

        // ── Positive rumours ───────────────────────────────────────────────

        if (gs.honour >= 60)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"That one has integrity. You don't see it often.\"",
                affected = new[] { "Nikias", "Eudoros", "Demetrios" },
                delta    = 3
            });

        if (gs.careerLevel >= 3)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"Apparently they're the best in their field now. Athens is talking.\"",
                affected = new[] { "Nikias", "Theron", "Xanthos" },
                delta    = 3
            });

        if (gs.GetFavour (gs.patronGod) >= 60)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"The priests say the gods smile on that one. I believe it.\"",
                affected = new[] { "Eudoros", "Phaedra" },
                delta    = 3
            });

        if (gs.drachma >= 300f)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"Word is they've done very well for themselves lately.\"",
                affected = new[] { "Nikias", "Miriam", "Kallias" },
                delta    = 3
            });

        if (CountRelationshipsAbove (gs, 50) >= 4)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"Everyone seems to like them. I can see why.\"",
                affected = new[] { "Lydia", "Chloe" },
                delta    = 2
            });

        // ── Negative rumours ───────────────────────────────────────────────

        if (gs.honour <= 20)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"I hear they've been cutting corners. Just saying.\"",
                affected = new[] { "Nikias", "Eudoros" },
                delta    = -3
            });

        if (AnyGodFavourBelow (gs, -30))
            candidates.Add (new Rumour {
                message  = "Stephanos: \"Something is off with them and the gods. I can feel it.\"",
                affected = new[] { "Eudoros", "Phaedra" },
                delta    = -3
            });

        if (gs.drachma <= 20f)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"Struggling a bit financially I think. Not my business.\"",
                affected = new[] { "Nikias", "Kallias" },
                delta    = -2
            });

        if (gs.careerLevel <= 1 && TimeManager.Instance != null
            && TimeManager.Instance.GetCurrentDay () > 20)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"Still finding their feet, aren't they. Bless them.\"",
                affected = new[] { "Theron", "Xanthos" },
                delta    = -2
            });

        // ── Festival flavour ───────────────────────────────────────────────

        if (FestivalManager.Instance != null && FestivalManager.Instance.IsFestivalDay)
            candidates.Add (new Rumour {
                message  = "Stephanos: \"Did you see them at the festival? Quite something.\"",
                affected = new string[0],
                delta    = 0
            });

        // ── Always-available flavour ───────────────────────────────────────

        candidates.Add (new Rumour {
            message  = "Stephanos: \"I saw them talking to Nikias yesterday. Looked serious.\"",
            affected = new string[0],
            delta    = 0
        });

        candidates.Add (new Rumour {
            message  = "Stephanos: \"Early riser, that one. I notice these things.\"",
            affected = new string[0],
            delta    = 0
        });

        // ── Pick one and apply ─────────────────────────────────────────────

        if (candidates.Count == 0) return;

        Rumour chosen = candidates[Random.Range (0, candidates.Count)];

        // Apply relationship changes
        foreach (string npc in chosen.affected)
            gs.ChangeRelationship (npc, chosen.delta);

        // Store message for summary screen
        gs.lastGossipMessage = chosen.message;

        Debug.Log ($"Gossip: {chosen.message}");
    }

    // ══════════════════════════════════════════════════════════════════════

    private static int CountRelationshipsAbove (GameState gs, int threshold)
    {
        int[] rels = {
            gs.relationshipNikias,   gs.relationshipDemetrios, gs.relationshipTheron,
            gs.relationshipArgos,    gs.relationshipEudoros,   gs.relationshipChloe,
            gs.relationshipKallias,  gs.relationshipLydia,     gs.relationshipMiriam,
            gs.relationshipPhaedra,  gs.relationshipStephanos, gs.relationshipXanthos
        };
        int count = 0;
        foreach (int r in rels)
            if (r >= threshold) count++;
        return count;
    }

    private static bool AnyGodFavourBelow (GameState gs, int threshold)
    {
        return gs.favourHermes     < threshold
            || gs.favourAres       < threshold
            || gs.favourAphrodite  < threshold
            || gs.favourApollo     < threshold
            || gs.favourHephaestus < threshold
            || gs.favourAthena     < threshold;
    }
}
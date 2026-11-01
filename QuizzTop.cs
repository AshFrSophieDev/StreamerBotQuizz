using System;
using System.Collections.Generic;
using System.Linq;

public class CPHInline
{
    public bool Execute()
    {
        var scores = CPH.GetGlobalVar<Dictionary<string, int>>("Quizz_Scores", false);

        if (scores == null || scores.Count == 0)
        {
            CPH.SendMessage("Aucun score enregistré pour le moment.");
            return true;
        }

        // Tri par score décroissant et sélection du top 5
        var topScores = scores.OrderByDescending(kv => kv.Value).Take(5).ToList();

        var output = new List<string>();
        for (int i = 0; i < topScores.Count; i++)
        {
            output.Add($"#{i + 1} {topScores[i].Key} ({topScores[i].Value} pt{(topScores[i].Value > 1 ? "s" : "")})");
        }

        CPH.SendMessage("Classement : " + string.Join(" | ", output));

        return true;
    }
}

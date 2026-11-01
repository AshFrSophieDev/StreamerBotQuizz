using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

public class CPHInline
{
    public bool Execute()
    {
        // Vérifie si un quizz est en cours
        if (!CPH.GetGlobalVar<bool>("Quizz_IsActive", false))
            return true;

        // Vérifie si la question n'a pas déjà été trouvée
        if (CPH.GetGlobalVar<bool>("Quizz_QuestionAnswered", false))
            return true;

        string targetAnswer = CPH.GetGlobalVar<string>("Quizz_CurrentAnswer", false);
        if (string.IsNullOrEmpty(targetAnswer))
            return true;

        // Récupération des données du message Twitch
        string userMessage = "";
        if (CPH.TryGetArg("message", out string rawMsg))
        {
            userMessage = rawMsg;
        }

        string user = "";
        if (CPH.TryGetArg("user", out string rawUser))
        {
            user = rawUser;
        }

        // Normalisation (sans accents, minuscules)
        string normalizedInput = NormalizeText(userMessage);

        // Comparaison exacte (ou utilisez .Contains() si vous voulez accepter la réponse au sein d'une phrase)
        if (normalizedInput.Equals(targetAnswer, StringComparison.OrdinalIgnoreCase))
        {
            // Verrouille la question
            CPH.SetGlobalVar("Quizz_QuestionAnswered", true, false);
            CPH.SetGlobalVar("Quizz_Winner", user, false);

            // Mise à jour des points
            var scores = CPH.GetGlobalVar<Dictionary<string, int>>("Quizz_Scores", false) ?? new Dictionary<string, int>();
            if (scores.ContainsKey(user))
            {
                scores[user]++;
            }
            else
            {
                scores[user] = 1;
            }

            CPH.SetGlobalVar("Quizz_Scores", scores, false);
        }

        return true;
    }

    private string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        string normalizedString = text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (char c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}

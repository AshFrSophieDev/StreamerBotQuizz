using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;

public class CPHInline
{
    private const string FILE_PATH = @"C:\chemin\vers\votre\MON_FICHIER.txt";

    public bool Execute()
    {
        // Vérification de sécurité : éviter de lancer 2 quizz en simultané
        if (CPH.GetGlobalVar<bool>("Quizz_IsActive", false))
        {
            CPH.SendMessage("Un quizz est déjà en cours !");
            return true;
        }

        if (!File.Exists(FILE_PATH))
        {
            CPH.SendMessage($"Erreur : Le fichier {FILE_PATH} est introuvable.");
            return false;
        }

        // Lecture et extraction des questions/réponses
        var questions = new List<Tuple<string, string>>();
        foreach (var line in File.ReadAllLines(FILE_PATH, Encoding.UTF8))
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains("|"))
                continue;

            var parts = line.Split(new[] { '|' }, 2);
            questions.Add(new Tuple<string, string>(parts[0].Trim(), parts[1].Trim()));
        }

        int totalQuestions = questions.Count;
        if (totalQuestions == 0)
        {
            CPH.SendMessage("Le fichier de questions est vide ou mal formaté.");
            return false;
        }

        // Initialisation de la session et réinitialisation des scores
        CPH.SetGlobalVar("Quizz_Scores", new Dictionary<string, int>(), false);
        CPH.SetGlobalVar("Quizz_IsActive", true, false);

        CPH.SendMessage($"Début du quizz de {totalQuestions} questions ! Vous avez 60 secondes pour répondre à chaque question.");
        CPH.Wait(3000);

        for (int i = 0; i < totalQuestions; i++)
        {
            string question = questions[i].Item1;
            string rawAnswer = questions[i].Item2;

            // Enregistrement de la réponse attendue et normalisée
            CPH.SetGlobalVar("Quizz_CurrentAnswer", NormalizeText(rawAnswer), false);
            CPH.SetGlobalVar("Quizz_QuestionAnswered", false, false);
            CPH.SetGlobalVar("Quizz_Winner", "", false);

            CPH.SendMessage($"[{i + 1}/{totalQuestions}] {question}");

            // Attente de 60 secondes max (par tranches de 500 ms)
            int elapsedMs = 0;
            const int maxWaitMs = 60000;
            const int stepMs = 500;

            while (elapsedMs < maxWaitMs)
            {
                if (CPH.GetGlobalVar<bool>("Quizz_QuestionAnswered", false))
                {
                    break;
                }

                CPH.Wait(stepMs);
                elapsedMs += stepMs;
            }

            // Clôture de la question en cours
            bool answered = CPH.GetGlobalVar<bool>("Quizz_QuestionAnswered", false);
            string winner = CPH.GetGlobalVar<string>("Quizz_Winner", false);

            if (answered)
            {
                CPH.SendMessage($"Bravo @{winner} ! La bonne réponse était bien : {rawAnswer}");
            }
            else
            {
                CPH.SendMessage($"Temps écoulé ! La bonne réponse était : {rawAnswer}");
            }

            // Nettoyage de l'état courant
            CPH.SetGlobalVar("Quizz_CurrentAnswer", "", false);
            CPH.SetGlobalVar("Quizz_QuestionAnswered", false, false);

            // Courte pause entre les questions (3 secondes)
            CPH.Wait(3000);
        }

        // Clôture du quizz
        CPH.SetGlobalVar("Quizz_IsActive", false, false);
        CPH.SendMessage("Le quizz est terminé ! Voici les scores :");
        CPH.Wait(1000);

        // Appel automatique de l'action QuizzTop
        CPH.RunAction("QuizzTop", true);

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

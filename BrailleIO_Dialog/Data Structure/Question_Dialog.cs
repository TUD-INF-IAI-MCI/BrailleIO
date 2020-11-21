//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs
{
    public class Question_Dialog : Dialog
    {
        private static LL ll = new LL(BrailleIO.Dialogs.Properties.Resources.Language);
        DialogEntry Question;
        QuestionDialogType QuestionType;
        QuestionDialogButtonType QuestionEntryType;


        public Question_Dialog(
            string title,
            string id,
            DialogType type = DialogType.Question,
            string question = "",
            QuestionDialogType qType = QuestionDialogType.Unknown,
            bool isActivated = false,
            Dialog parentDialog = null)
            : base(title, id, type, isActivated, parentDialog)
        {
            Question = new SelfRenderingDialogEntry(id + "_Question", ll.GetTrans("question.qType." + qType.ToString()) + ": " + question, ll.GetTrans("question.question.help.question"), DialogEntryType.Label);
            
            QuestionType = qType;

            setQuestionDialogEntryTypes(qType);
            addQuestionDialogEntries(id);
        }

        private void addQuestionDialogEntries(String id)
        {
            AddEntry(Question);

            if (QuestionEntryType.HasFlag(QuestionDialogButtonType.Back))
            {
                AddEntry(new SelfRenderingDialogEntry(id + "_Back", ll.GetTrans("question.button.back.label"), ll.GetTrans("question.button.back.help"), DialogEntryType.Button));
            }

            if (QuestionEntryType.HasFlag(QuestionDialogButtonType.OK))
            {
                AddEntry(new SelfRenderingDialogEntry(id + "_OK", ll.GetTrans("question.button.ok.label"), ll.GetTrans("question.button.ok.help"), DialogEntryType.Button));
            }
            
            if (QuestionEntryType.HasFlag(QuestionDialogButtonType.Continue))
            {
                AddEntry(new SelfRenderingDialogEntry(id + "_Continue", ll.GetTrans("question.button.continue.label"), ll.GetTrans("question.button.continue.help"), DialogEntryType.Button));
            }
            if (QuestionEntryType.HasFlag(QuestionDialogButtonType.Abort))
            {
                AddEntry(new SelfRenderingDialogEntry(id + "_Abort", ll.GetTrans("question.button.abort.label"), ll.GetTrans("question.button.abort.help"), DialogEntryType.Button));
            }
            
            if (QuestionEntryType.HasFlag(QuestionDialogButtonType.Yes))
            {
                AddEntry(new SelfRenderingDialogEntry(id + "_Yes", ll.GetTrans("question.button.yes.label"), ll.GetTrans("question.button.yes.help"), DialogEntryType.Button));
            }
            if (QuestionEntryType.HasFlag(QuestionDialogButtonType.No))
            {
                AddEntry(new SelfRenderingDialogEntry(id + "_No", ll.GetTrans("question.button.no.label"), ll.GetTrans("question.button.no.help"), DialogEntryType.Button));
            }
            if (QuestionEntryType.HasFlag(QuestionDialogButtonType.Finish))
            {
                AddEntry(new SelfRenderingDialogEntry(id + "_Finish", ll.GetTrans("question.button.finish.label"), ll.GetTrans("question.button.finish.help"), DialogEntryType.Button));
            }
        }

        private void setQuestionDialogEntryTypes(QuestionDialogType qType)
        {
            switch (qType)
            {
                case QuestionDialogType.Unknown:
                    break;
                case QuestionDialogType.Information:
                    QuestionEntryType = QuestionDialogButtonType.OK; 
                    break;
                case QuestionDialogType.Question:
                    QuestionEntryType = QuestionDialogButtonType.Yes | QuestionDialogButtonType.No;
                    break;
                case QuestionDialogType.Warning:
                    QuestionEntryType = QuestionDialogButtonType.OK | QuestionDialogButtonType.Abort;
                    break;
                case QuestionDialogType.Error:
                    QuestionEntryType = QuestionDialogButtonType.OK | QuestionDialogButtonType.Abort;
                    break;
                default: break;
            }
        }


        [Flags]
        public enum QuestionDialogButtonType: int
        {
            Unknown = 0, 
            OK = 1, 
            Abort=2, 
            Continue =4,
            Back = 8,
            Yes = 16, 
            No = 32, 
            Finish = 64
        }

        public enum QuestionDialogType: int
        {
            Unknown = 0,
            Information = 1,
            Question = 2,
            Warning = 3,
            Error = 4,
        }
    }
}

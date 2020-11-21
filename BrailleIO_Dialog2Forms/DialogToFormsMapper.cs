//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrailleIO.Dialogs
{
    /// <summary>
    /// Class for converting a BrailleIO_Dialog to a BrailleIO_Dialog2Forms item.
    /// </summary>
    public class DialogToFormsMapper
    {

        /// <summary>
        /// Converts a Dialog to a Dialog_ToolStripMenuItem with all its branches.
        /// </summary>
        /// <param name="dlg">Dialog to be converted</param>
        /// <returns>Dialog_ToolStripMenuItem</returns>
        public static DialogToolStripMenuItem ConvertDialogToDialogToolStripItem(Dialog dlg, Boolean IntegrateQuestions = true)
        {

            Dialog_ToolStripMenuItem menuItem = null;

            if (dlg != null)
            {
                menuItem = buildDialogToolStripMenuItemStructure(dlg, IntegrateQuestions);
            }
     
            return menuItem;
        }

        /// <summary>
        /// Builds the Dialog_ToolStripMenuItem structure. Calls it self recursive with a Submenu entry.
        /// </summary>
        /// <param name="dlg">Dialog to be converted</param>
        /// /// <param name="IntegrateQuestions">IntegrateQuestions to integrate or leave out QuestionTypes</param>
        /// <returns>Dialog_ToolStripMenuItem</returns>
        private static Dialog_ToolStripMenuItem buildDialogToolStripMenuItemStructure(Dialog dlg, Boolean IntegrateQuestions = true)
        {
            Dialog_ToolStripMenuItem menuStripItem = null;

             if (dlg != null){

                 if (!IntegrateQuestions && dlg.Type == DialogType.Question)
                 {
                     Console.Out.WriteLine(dlg.Title +  "  is a Question!");
                 }
                 else
                 {

                     menuStripItem = new Dialog_ToolStripMenuItem(dlg);

                     List<DialogEntry> dlgEntryList = dlg.GetEntryList();

                     if (dlgEntryList != null && dlgEntryList.Count > 0)
                     {
                         bool needsGroupSeperator = false;
                         int addSeperatorAfter = 0;

                         foreach (DialogEntry item in dlgEntryList)
                         {
                             DialogToolStripMenuItem newMenuItem = null;

                             if (item != null && item.Type != null)
                             {
                                 if (item.Type == DialogEntryType.Submenu && item.GetSubmenu() != null)
                                 {
                                     if (!IntegrateQuestions && item.GetSubmenu().Type == DialogType.Question)
                                     {
                                         Console.Out.WriteLine(item.Title + "  is an entry directing to a Question!");
                                     }
                                     else
                                     {
                                         newMenuItem = buildDialogToolStripMenuItemStructure(item.GetSubmenu(), IntegrateQuestions);
                                         newMenuItem.dialogEntryItem = item;
                                     }
                                     
                                 }
                                 else
                                 {
                                     newMenuItem = new DialogEntry_ToolStripMenuItem(item);

                                     if (item.Type == DialogEntryType.Group)
                                     {
                                         needsGroupSeperator = true;
                                         addSeperatorAfter = item.GetChildEntryList().Count;
                                         menuStripItem.DropDownItems.Add(new ToolStripSeparator());
                                     }
                                 }

                             }

                             if (menuStripItem != null && newMenuItem != null)
                             {
                                 newMenuItem.Parent = menuStripItem;
                                 menuStripItem.DropDownItems.Add(newMenuItem);

                                 if (needsGroupSeperator && item.IsInGroup())
                                 {
                                     addSeperatorAfter--;
                                     if (addSeperatorAfter == 0)
                                     {
                                         menuStripItem.DropDownItems.Add(new ToolStripSeparator());
                                         needsGroupSeperator = false;
                                     }
                                 }
                             }
                    
                         }

                     }
                 }

             }

             return menuStripItem;
        }

    }


}

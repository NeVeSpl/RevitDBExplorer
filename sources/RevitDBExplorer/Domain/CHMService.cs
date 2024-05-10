using System.Windows;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Properties;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal class CHMService
    {
        public static void OpenCHM(SnoopableMember snoopableMember)
        {
            var helpFileName = GetCHMFilePath();
            if (helpFileName != null)
            {
                string postfix = "";
                switch (snoopableMember.MemberKind)
                {
                    case Domain.DataModel.Members.Base.MemberKind.Property:
                        postfix = " property";
                        break;
                    case Domain.DataModel.Members.Base.MemberKind.Method:
                    case Domain.DataModel.Members.Base.MemberKind.StaticMethod:
                    case Domain.DataModel.Members.Base.MemberKind.AsArgument:
                        postfix = " method";
                        break;
                }
            
                System.Windows.Forms.Help.ShowHelp(null, helpFileName,
                    System.Windows.Forms.HelpNavigator.KeywordIndex,
                    $"{snoopableMember.DeclaringType.BareName}.{snoopableMember.Name}{postfix}");
            }         
        }

        public static void OpenCHM()
        {
            var helpFileName = GetCHMFilePath();
            if (helpFileName != null)
            {
                System.Windows.Forms.Help.ShowHelp(null, helpFileName,
                    System.Windows.Forms.HelpNavigator.KeywordIndex,
                    null
                    );
            }
        }
        public static void OpenCHM(string keyword)
        {
            var helpFileName = GetCHMFilePath();
            if (helpFileName != null)
            {
                System.Windows.Forms.Help.ShowHelp(null, helpFileName,
                   System.Windows.Forms.HelpNavigator.KeywordIndex,
                   keyword);
            }
        }

        private static string GetCHMFilePath()
        {
            string helpFileName = AppSettings.Default.RevitAPICHMFilePath;
            helpFileName = helpFileName.Replace("\"", "");
            if (System.IO.File.Exists(helpFileName))
            {
                return helpFileName;
            }
            else
            {
                MessageBox.Show($".chm file does not exist at the given location: {helpFileName}. Please set the correct location in the configuration.");
            }
            return null;
        }
    }
}
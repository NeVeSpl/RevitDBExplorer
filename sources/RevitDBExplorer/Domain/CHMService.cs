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
            string helpFileName = AppSettings.Default.RevitAPICHMFilePath;
            helpFileName = helpFileName.Replace("\"", "");
            if (System.IO.File.Exists(helpFileName))
            {
                string postfix = "";
                switch (snoopableMember.MemberKind)
                {
                    case Domain.DataModel.Streams.Base.MemberKind.Property:
                        postfix = " property";
                        break;
                    case Domain.DataModel.Streams.Base.MemberKind.Method:
                    case Domain.DataModel.Streams.Base.MemberKind.StaticMethod:
                    case Domain.DataModel.Streams.Base.MemberKind.AsArgument:
                        postfix = " method";
                        break;
                }
            
                System.Windows.Forms.Help.ShowHelp(null, helpFileName,
                    System.Windows.Forms.HelpNavigator.KeywordIndex,
                    $"{snoopableMember.DeclaringType.BareName}.{snoopableMember.Name}{postfix}");
            }
            else
            {
                MessageBox.Show($".chm file does not exist at the given location: {helpFileName}. Please set the correct location in the configuration.");
            }            
        }
    }
}
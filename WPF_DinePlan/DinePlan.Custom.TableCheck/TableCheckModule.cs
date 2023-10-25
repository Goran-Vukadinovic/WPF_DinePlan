using DinePlan.Presentation.Common;
using Prism.Mef.Modularity;
using Prism.Modularity;

namespace DinePlan.Custom.TableCheck
{
    [ModuleExport(typeof(TableCheckModule), InitializationMode = InitializationMode.WhenAvailable)]
    public class TableCheckModule : ModuleBase
    {
    }
}
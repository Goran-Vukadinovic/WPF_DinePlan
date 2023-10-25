using System.ComponentModel.Composition;
using DinePlan.Custom.TableCheck.Model;
using DinePlan.Custom.TableCheck.Service;
using DinePlan.Services.Common;

namespace DinePlan.Custom.TableCheck
{
    [Export(typeof(IActionType))]
    public class UpdatePosJournal : ActionType
    {
        private readonly ITableCheckService _tableCheckService;

        [ImportingConstructor]
        public UpdatePosJournal(ITableCheckService tableCheckService)
        {
            _tableCheckService = tableCheckService;
        }

        public override void Process(ActionData actionData)
        {
            var journalId = actionData.GetDataValue<string>("PosJournalId");
            var journal = actionData.GetDataValue<PosJournalInputModel>("PosJournal");

            if (journal != null)
            {
                _tableCheckService.UpdatePosJournal(journalId, journal);
            }
        }

        protected override object GetDefaultData()
        {
            return new { };
        }

        protected override string GetActionName()
        {
            return "UpdatePosJournal";
        }

        protected override string GetActionKey()
        {
            return "UpdatePosJournal";
        }
    }
}

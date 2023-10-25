using System.Collections.Generic;
using DinePlan.Custom.TableCheck.Model;

namespace DinePlan.Custom.TableCheck.Service
{
    public interface ITableCheckService
    {
        ReservationListModel GetReservations();
        ReservationListModel GetReservations(string date);
        ReservationListModel GetReservationsByTableCurrent(string tableName);
        ReservationListModel GetReservationsByTableToday(string tableName);
        ReservationModel GetReservation(string refOrId);
        PosJournalOutputListModel GetPosJournals();
        PosJournalOutputModel GetPosJournals(string posJournalId);
        ReservationListModel UpdateReservation(string refOrId, ReservationListModel input);
        PosJournalOutputModel UpdatePosJournal(string posJournalId, PosJournalInputModel input);
        PosJournalOutputModel CreatePosJournal(PosJournalInputModel input);
        bool DeleteReservation(string refOrId);
        bool DeletePosJournal(string posJournalId);

        void StartTableCheckService();
    }
}

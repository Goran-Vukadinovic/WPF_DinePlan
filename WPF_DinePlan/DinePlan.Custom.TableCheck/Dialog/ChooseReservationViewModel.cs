using DinePlan.Domain.Models.Reserve;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;

namespace DinePlan.Custom.TableCheck.Dialog
{
    [Export]
    public class ChooseReservationViewModel : DialogViewModelBase<Reservation>
    {
        private IList<Reservation> _reservations;

        [ImportingConstructor]
        public ChooseReservationViewModel()
        {
            AssignCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Assign).ToUpper(), OnAssignReservation, CanAssignReservation);
            RefreshCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Refresh).ToUpper(), OnRefresh);
            CancelCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Cancel).ToUpper(), OnCancel);
        }

        private void OnCancel(string obj)
        {
            DialogResult = null;
            Dialog.Hide();
        }

        private void OnRefresh(string obj)
        {
            Reservations = ReserveService.GetReservations(DateTime.Today.Date, DateTime.Today.Date.AddDays(1), 0, ReservationSource.TableCheck);
        }

        public Reservation SelectedReservation { get; set; }

        private void OnAssignReservation(string obj)
        {
            if (SelectedReservation != null)
            {
                DialogResult = SelectedReservation;
                Dialog.Hide();
            }
        }

        private bool CanAssignReservation(string arg)
        {
            return SelectedReservation != null;
        }

        public IList<Reservation> Reservations
        {
            get => _reservations;
            set
            {
                _reservations = value;
                RaisePropertyChanged(nameof(Reservations));
            }
        }

        protected override Window OnCreateDialog()
        {
            
            return new ChooseReservationView(this)
            {
                Owner = Application.Current.MainWindow
            };
        }

        public ICaptionCommand AssignCommand { get; set; }

        public ICaptionCommand RefreshCommand { get; set; }

        public ICaptionCommand CancelCommand { get; set; }

        private void InitData()
        {

        }

        protected override Reservation OnShowDialog()
        {
            try
            {
                OnRefresh(null);
                if (Dialog.ShowDialog() == true) return DialogResult;
            }
            catch (Exception)
            {
                // ignored
            }

            return DialogResult;
        }
    }
}

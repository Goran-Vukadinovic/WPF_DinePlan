using DinePlan.Common.App;
using DinePlan.Common.Model.API;
using DinePlan.Custom.TableCheck.Model;
using DinePlan.Domain.Models.Reserve;
using DinePlan.Services;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Timers;

namespace DinePlan.Custom.TableCheck.Service
{
    [Export(typeof(ITableCheckService))]
    public class TableCheckService : ITableCheckService
    {
        private readonly ISettingService _settingService;
        private readonly ILogService _logService;
        private readonly IReserveService _reserveService;
        private readonly Timer _timer;

        [ImportingConstructor]
        public TableCheckService(ISettingService settingService
            , ILogService logService
            , IReserveService reserveService)
        {
            _settingService = settingService;
            _logService = logService;
            _reserveService = reserveService;

            var interval = 5 * 1000; // Run every 5 seconds (should be configurable)
            _timer = new Timer(interval);
            _timer.Elapsed += Elapsed;
        }

        public void StartTableCheckService()
        {
            _logService.Log("Bootstrapping Table Check");
            _timer.Start();
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            var maxDate = _reserveService.GetMaxDateReservation();
            if (maxDate == null)
            {
                maxDate = DateTime.UtcNow.Date;
            }

            var reservations = GetReservations(maxDate.Value.ToUniversalIso8601());
            if (reservations != null && reservations.Reservations != null)
            {
                foreach (var rev in reservations.Reservations)
                {
                    _reserveService.AddReservation(new Domain.Models.Reserve.Reservation()
                    {
                        PhoneNumber = rev.Code,
                        Name = rev.CustomerName,
                        Pax = rev.Pax.ToString(),
                        CreatedDate = rev.CreatedAt,
                        CreatedTime = DateTime.Now,
                        Status = (int)ReservationStatus.Created,
                        RemoteId = rev.Id,
                        Duration = rev.Duration,
                        Notes = string.Join(",", rev.TableNames),
                        ReservationSource = (int)ReservationSource.TableCheck,
                        OtherDetails = JsonConvert.SerializeObject(rev),
                    });
                }
            }
        }

        public ReservationListModel GetReservations()
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/reservations";
                return JsonConvert.DeserializeObject<ReservationListModel>(GetApi(requestUrl));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call GetReservations ERROR: ", false);
            }

            return null;
        }

        public ReservationListModel GetReservations(string date)
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/reservations?created_at_min=" + date;
                return JsonConvert.DeserializeObject<ReservationListModel>(GetApi(requestUrl));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call GetReservations ERROR: ", false);
            }

            return null;
        }

        // NOt see from API
        public ReservationListModel GetReservationsByTableCurrent(string tableName)
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/reservations/by_table_current/" + tableName;
                return JsonConvert.DeserializeObject<ReservationListModel>(GetApi(requestUrl));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call GetReservationsByTableCurrent ERROR: ", false);
            }

            return null;
        }

        // NOt see from API
        public ReservationListModel GetReservationsByTableToday(string tableName)
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/reservations/by_table_today/" + tableName;
                return JsonConvert.DeserializeObject<ReservationListModel>(GetApi(requestUrl));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call GetReservationsByTableToday ERROR: ", false);
            }

            return null;
        }

        public ReservationModel GetReservation(string refOrId)
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/reservations/" + refOrId;
                return JsonConvert.DeserializeObject<ReservationModel>(GetApi(requestUrl));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call GetReservation ERROR: ", false);
            }

            return null;
        }

        public PosJournalOutputListModel GetPosJournals()
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/pos_journals";
                return JsonConvert.DeserializeObject<PosJournalOutputListModel>(GetApi(requestUrl));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call GetReservation ERROR: ", false);
            }

            return null;
        }

        public PosJournalOutputModel GetPosJournals(string posJournalId)
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/pos_journals/" + posJournalId;
                return JsonConvert.DeserializeObject<PosJournalOutputModel>(GetApi(requestUrl));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call GetReservation ERROR: ", false);
            }

            return null;
        }

        public ReservationListModel UpdateReservation(string refOrId, ReservationListModel input)
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/reservations/" + refOrId;
                return JsonConvert.DeserializeObject<ReservationListModel>(PutApi(requestUrl,
                    JsonConvert.SerializeObject(input)));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call UpdateReservation ERROR: ", false);
            }

            return null;
        }

        public PosJournalOutputModel UpdatePosJournal(string posJournalId, PosJournalInputModel input)
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/pos_journals/" + posJournalId;
                return JsonConvert.DeserializeObject<PosJournalOutputModel>(PutApi(requestUrl,
                    JsonConvert.SerializeObject(input)));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call UpdateReservation ERROR: ", false);
            }

            return null;
        }

        public PosJournalOutputModel CreatePosJournal(PosJournalInputModel input)
        {
            try
            {
                var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/pos_journals/";
                return JsonConvert.DeserializeObject<PosJournalOutputModel>(PostApi(requestUrl,
                    JsonConvert.SerializeObject(input)));
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, ">> Call UpdateReservation ERROR: ", false);
            }

            return null;
        }

        public bool DeleteReservation(string refOrId)
        {
            var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/reservations/" + refOrId;
            return DeleteApi(requestUrl);
        }

        public bool DeletePosJournal(string posJournalId)
        {
            var requestUrl = _settingService.ProgramSettings.TableCheckApiUrl + "/pos_journals/" + posJournalId;
            return DeleteApi(requestUrl);
        }

        private string GetApi(string requestUrl)
        {
            if (string.IsNullOrEmpty(_settingService.ProgramSettings.TableCheckApiUrl)) return null;

            try
            {
                var client = new RestClient(requestUrl);
                var request = CreateRequest(Method.GET);
                _logService.Log($"Call Service: {requestUrl}");
                _logService.Log($"WebReq Timeout: {client.Timeout}");
                _logService.Log("GetApi request: " + requestUrl);

                var apiResponse = (RestResponse)client.Execute(request);

                var log = $"TableCheck response {apiResponse.Content}\r\n";
                if (!string.IsNullOrEmpty(apiResponse.ErrorMessage))
                    log += $" - Error message: {apiResponse.ErrorMessage}";
                _logService.Log(log);

                if (apiResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(apiResponse.Content))
                {
                    _logService.Log("GetApi response: " + apiResponse.Content);
                    return apiResponse.Content;
                }

                _logService.Log(apiResponse.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        private string PostApi(string requestUrl, string input)
        {
            if (string.IsNullOrEmpty(_settingService.ProgramSettings.TableCheckApiUrl)) return null;

            try
            {
                var client = new RestClient(requestUrl);
                var request = CreateRequest(Method.POST);


                _logService.Log($"Call Service: {requestUrl}");
                _logService.Log($"WebReq Timeout: {client.Timeout}");
                _logService.Log("UpSertApi request: " + requestUrl + "-" + input);

                request.AddParameter("application/json", input, ParameterType.RequestBody);

                var apiResponse = (RestResponse)client.Execute(request);

                var log = $"TableCheck response {apiResponse.Content}\r\n";
                if (!string.IsNullOrEmpty(apiResponse.ErrorMessage))
                    log += $" - Error message: {apiResponse.ErrorMessage}";
                _logService.Log(log);

                if (apiResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(apiResponse.Content))
                {
                    _logService.Log("UpSertApi response: " + apiResponse.Content);
                    return apiResponse.Content;
                }

                _logService.Log(apiResponse.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        private string PutApi(string requestUrl, string input)
        {
            if (string.IsNullOrEmpty(_settingService.ProgramSettings.TableCheckApiUrl)) return null;

            try
            {
                var client = new RestClient(requestUrl);
                var request = CreateRequest(Method.PUT);


                _logService.Log($"Call Service: {requestUrl}");
                _logService.Log($"WebReq Timeout: {client.Timeout}");
                _logService.Log("UpSertApi request: " + requestUrl + "-" + input);

                var apiResponse = (RestResponse)client.Execute(request);

                var log = $"TableCheck response {apiResponse.Content}\r\n";
                if (!string.IsNullOrEmpty(apiResponse.ErrorMessage))
                    log += $" - Error message: {apiResponse.ErrorMessage}";
                _logService.Log(log);

                if (apiResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(apiResponse.Content))
                {
                    _logService.Log("UpSertApi response: " + apiResponse.Content);
                    return apiResponse.Content;
                }

                _logService.Log(apiResponse.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        private bool DeleteApi(string requestUrl)
        {
            if (string.IsNullOrEmpty(_settingService.ProgramSettings.TableCheckApiUrl)) return false;

            try
            {
                var client = new RestClient(requestUrl);
                var request = CreateRequest(Method.DELETE);

                _logService.Log($"Call Service: {requestUrl}");
                _logService.Log($"WebReq Timeout: {client.Timeout}");
                _logService.Log("DeleteApi request: " + requestUrl);

                var apiResponse = (RestResponse)client.Execute(request);

                var log = $"TableCheck response {apiResponse.Content}\r\n";
                if (!string.IsNullOrEmpty(apiResponse.ErrorMessage))
                    log += $" - Error message: {apiResponse.ErrorMessage}";
                _logService.Log(log);

                if (apiResponse.StatusCode == HttpStatusCode.OK)
                {
                    _logService.Log("DeleteApi success ");
                    return true;
                }

                _logService.Log(apiResponse.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        private RestRequest CreateRequest(Method method)
        {
            var request = new RestRequest(method)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", _settingService.ProgramSettings.TableCheckAuthorizeKey);
            request.AddHeader("Content-Type", "application/json");
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return request;
        }

       
    }
}

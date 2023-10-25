using DinePlan.Common.Model;
using System.Collections.Generic;

namespace DinePlan.Modules.UserModule.ViewModels.Dtos
{
    public class ConnectMemberListDto : IOutputDto
    {
        public List<ConnectMemberDto> Items { get; set; }
        public ConnectMemberListDto()
        {
            Items = new List<ConnectMemberDto>();
        }
    }
    public class CreateMemberInput : IInputDto
    {
        public ConnectMemberDto Member { get; set; }
        public int TenantId { get; set; }
    }
    public class MessageOutputDto : IOutputDto
    {
        public MessageOutputDto()
        {
            OutputMessageList = new List<string>();
            ErrorMessageList = new List<string>();
            SuccessMessageList = new List<string>();
        }
        public bool SuccessFlag { get; set; }
        public bool Exists { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> OutputMessageList { get; set; }
        public List<string> ErrorMessageList { get; set; }
        public List<string> SuccessMessageList { get; set; }
        public int Id { get; set; }
        public bool DuplicateEmployeeError { get; set; }
        public string DuplicateEmployeeErrorMessage { get; set; }
        public bool PrWithIn2Years { get; set; }
        public string PrWithin2YearsAlertMessage { get; set; }
    }
}

using System.Collections.Generic;

namespace DinePlan.Common.Model.Table
{
    public class ApiConnectTableListOutput
    {
        public int TotalCount { get; set; }
        public List<ApiConnectTableGroupOutput> Items { get; set; }
    }

    public class ApiConnectTableGroupOutput
    {
        public ApiConnectTableGroupOutput()
        {
            ConnectTables = new List<ApiConnectTable>();
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public string Locations { get; set; }
        public List<ApiConnectTable> ConnectTables { get; set; }
    }

    public class ApiConnectTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Pax { get; set; }
    }
}
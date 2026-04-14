using System;

namespace DataTransferObject.Domain.Historico
{
    public class HistoricoDTO
    {
        public int history_id { get; set; }
        public int data_item_type { get; set; }
        public long data_item_id { get; set; }
        public string the_field { get; set; }
        public string previous_value { get; set; }
        public string new_value { get; set; }
        public string description { get; set; }
        public DateTime set_date { get; set; }
        public string entered_by { get; set; }
        public int site_id { get; set; }
        public int flag { get; set; }
    }
}
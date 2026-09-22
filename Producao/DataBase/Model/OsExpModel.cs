using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Producao.DataBase.Model
{
    [Table("tbl_os_exp", Schema = "expedicao")]
    public class OsExpModel : INotifyPropertyChanged
    {
        private long? _nOsDesbaiamento;
        private string? _sigla;

        [Key]
        public long? n_os_desbaiamento
        {
            get => _nOsDesbaiamento;
            set
            {
                if (_nOsDesbaiamento == value) return;
                _nOsDesbaiamento = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(n_os_desbaiamento)));
            }
        }
        public int? antigo { get; set; }
        public string? codvol { get; set; }
        public DateTime? data { get; set; }
        public string? resp { get; set; }
        public string? setor { get; set; }
        public string? motivo { get; set; }
        public long? quantidade { get; set; }
        public string? obs { get; set; }
        public long? coddetalhescompl { get; set; }
        public string? sigla
        {
            get => _sigla;
            set
            {
                if (_sigla == value) return;
                _sigla = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(sigla)));
            }
        }
        public string? solicitante { get; set; }
        public string? local_shopp { get; set; }
        public string? inserido_por { get; set; }
        public DateTime? inserido_em { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

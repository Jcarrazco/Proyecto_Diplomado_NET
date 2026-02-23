using IndigoAssistMVC.Models;

namespace IndigoAssistMVC.ViewModels
{
    public class ActivoIndexViewModel
    {
        public ActivoFiltroViewModel Filtro { get; set; } = new ActivoFiltroViewModel();
        public IEnumerable<ActivoViewModel>? Resultados { get; set; }
        public int TotalCount => Resultados?.Count() ?? 0;
    }
}

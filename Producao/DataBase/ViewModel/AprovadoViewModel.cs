using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Producao
{
    internal class AprovadoViewModel
    {
        public async Task<ObservableCollection<AprovadoModel>> GetAprovados()
        {
            IList<AprovadoModel> aprovados;
            try
            {
                using DatabaseContext db = new();
                var data = await db.Aprovados.OrderBy(c => c.Ordem).ToListAsync();
                //var data = await query.ToListAsync();
                return new ObservableCollection<AprovadoModel>(data);

            }
            catch (Exception)
            {
                throw;
            }

            
            //return aprovados;
        }

        public async Task SaveAsync(AprovadoModel aprovado)
        {
            try
            {
                using DatabaseContext db = new();
                AprovadoModel found = await db.Aprovados.FindAsync(aprovado.IdAprovado);
                db.Entry(found).CurrentValues.SetValues(aprovado);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

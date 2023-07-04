using Microsoft.EntityFrameworkCore;
using NLayer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        //miras alacağımız yerler olduğu için protected olarak tanımladık
        protected AppDbContext _context;

        // readonly olarak tnımladığımızda değişkenlere bu esnada değer atayabiliriz ya da, constructor da değer atayabiliriz.
        //constructor da değer atayacağız ve set etmeyeceğimiz için readonly olarak tanımladık.
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // GENERIC, TEMEL OPERASYONLAR TÜM ENTITY'LER İÇİN GEÇERLİ OLSUN DİYE
        public async Task AddAsync(T entity)
        {
            // geriye bir şey döndürmeyeceğimiz için async yaptık.
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            // bool değer döndürdüğü için return dedik
            return await _dbSet.AnyAsync(expression);
        }

        public IQueryable<T> GetAll()
        {
            // "AsNoTracking()" dememizin sebebi, EFCore çekmiş olduğu verileri hafızaya almasın diye. (Daha performanslı çalışması için)
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            // Remove sadece o entity nin durumunu delete olarak işaretler (state=delete), bu yüzden asenkron metodu yoktur saveChanges metotu çağırıldığında delete olarak işaretlenenler bulunur ve silinir.

            //_context.Entry(entity).State = EntityState.Deleted;

            // propert sini setlediğimix bir metotun asenkron olmasına gerek yok, ağır bir iş yapılmıyor.

            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
           _dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
    }
}

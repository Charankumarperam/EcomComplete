using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.IRepository;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
namespace DataAccess.Repository
{
    public class GenericRepository<T>:IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IList<T>> GetAllAsync()
        {
            var entities= await _context.Set<T>().ToListAsync();
            return entities;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var result= await _context.Set<T>().FindAsync(id);
            if(result==null)
                {
                throw new Exception("Entity not found");
            }
            return result;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

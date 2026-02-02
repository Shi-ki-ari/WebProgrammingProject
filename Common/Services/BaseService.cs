using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Common.Services;

public class BaseService<T>
    where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> Items;

    public BaseService(AppDbContext context)
    {
        Context = context;
        Items = Context.Set<T>();
    }

    public List<T> GetAll()
    {
        return Items.ToList();
    }

    public T GetById(int id)
    {
        return Items.FirstOrDefault(item => item.Id == id);
    }

    public void Save(T item)
    {
        if (item.Id > 0)
            Items.Update(item); 
        else
            Items.Add(item);    

        Context.SaveChanges();
    }

    public void Delete(T item)
    {
        Items.Remove(item);
        Context.SaveChanges();
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecam.Framework;

namespace Ecam.Models {

    public class ServiceFactory:IServiceFactory {
        public IService<T> Create<T>() where T:BaseEntity {
            Type type = typeof(Service<>).MakeGenericType(typeof(T));
            return (IService<T>)Activator.CreateInstance(type);
        }

        public void Register<T>(IService<T> service) where T:BaseEntity {
            throw new NotImplementedException();
        }
    }

    public class Service<T>:IService<T> where T:BaseEntity {

        public Service() {
            _errorProvider = new ErrorProvider();
        }

        private IErrorProvider _errorProvider = null; // ErrorProvider.Current;

        public IErrorProvider Errors {
            get { return _errorProvider; }
            set { _errorProvider = value; }
        }

        public virtual IQueryable<T> GetById(int id) {
            return All().Where(x => x.id == id);
        }

        public virtual IQueryable<T> GetByIds(List<int> ids) {
            return All().Where(x => ids.Contains(x.id));
        }

        public virtual IQueryable<T> All() {
            return Context;
        }

        private DbSet<T> _context = null;

        protected DbSet<T> Context {
            get {
                if(_context == null) {
                    //EcamContext context = new EcamContext();
                    EcamContext context = ContextFactory.Instance;
                    _context = context.Set<T>();
                }
                return _context;
            }
        }

        public T Save(T obj,bool delaySave = false) {
            if(obj.id > 0) {
                Update(obj,delaySave);
            } else {
                Create(obj,delaySave);
            }
            return obj;
        }

        public T Create(T obj,bool delaySave = false) {
            //using(EcamContext context = new EcamContext()) {
            EcamContext context = ContextFactory.Instance;
            //if (context.Entry(obj).State == System.Data.EntityState.Detached) {
            //    context.Set<T>().Attach(obj);
            //}
            context.Set<T>().Add(obj);
            if(!delaySave) {
                SaveChanges();
            }
            return obj;
        }

        public bool Update(T obj,bool delaySave = false) {
            //using (EcamContext context = new EcamContext()) {
            EcamContext context = ContextFactory.Instance;
            //_context = context.Set<T>();
            // Drawback:  setting the state to Modified forces that all properties are sent to the DB in an UPDATE statement, no matter if they changed or not
            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            if(!delaySave) {
                SaveChanges();
            }
            return true;
        }


        //public void Update(T entity) {
        //    using (EcamContext _context = new EcamContext()) {
        //        DbEntityEntry<T> oldEntry = _context.Entry(entity);

        //        if (oldEntry.State == System.Data.EntityState.Detached) {
        //            _context.Set<T>().Attach(oldEntry.Entity);
        //        }

        //        oldEntry.CurrentValues.SetValues(entity);

        //        _context.SaveChanges();
        //    }
        //}

        public void Delete(int id,bool delaySave = false) {
            Destroy(id);
        }

        public void Delete<T>(T obj,bool delaySave = false) where T:BaseEntity {
            Destroy(obj);
        }

        public void Destroy(int id,bool delaySave = false) {
            // Since we are creating only one context per request ( using EcamContext _context = ContextFactory.Instance;), we need to make sure that we are not
            // attaching the same entity with the same key more than once. This can occur in the cases when we fetch the entity to make sure it is valid, and then
            // some other place(eg: here in destroy), we try to attach again to delete it *
            // * the following code was being used
            // <!-- Start
            //EcamContext context = ContextFactory.Instance;
            //T entity = Activator.CreateInstance(typeof (T)) as T;
            //entity.id = id;
            ////Approach 1
            //var obj = context.Entry(entity);
            //if (obj.State == System.Data.EntityState.Detached) {
            //    context.Set(typeof (T)).Attach(obj.Entity); -- Error ** was being thrown
            //}
            //context.Set(typeof (T)).Remove(obj.Entity);
            // END -->
            // ** An object with the same key already exists in the ObjectStateManager. The ObjectStateManager cannot track multiple objects with the same key.

            // The reason in that If you load the entity from the context you cannot attach an entity with the same key again. The first entity is still kept in internal context cache and context can hold only one instance with given key value per type (it is called identity map and it is  described  here : http://stackoverflow.com/questions/3653009/entity-framework-and-connection-pooling/3653392#3653392
            // You can solve it by detaching former instance but you don't have to. If you only need to save new values you can use this:
            // context.Entry(oldEntity).CurrentValues.SetValues(newEntity);

            //using (EcamContext context = new EcamContext()) {
            //    T targetEntity = context.Set<T>().Find(id);
            //    context.Set(typeof(T)).Remove(targetEntity);
            //    context.SaveChanges();
            //}

            EcamContext _context = ContextFactory.Instance;
            T entity = Activator.CreateInstance(typeof(T)) as T;
            entity.id = id;
            // context.Entry registers the entity with the context if the entity is not already present in the context (in which case the state will be detached). 
            // If you try to attach the entity to the context, but the context already contains an entity with the same key, then you will get the error **
            var entry = _context.Entry<T>(entity);
            if(entry.State == System.Data.Entity.EntityState.Detached) {
                var set = _context.Set<T>();
                T attachedEntity = set.Find(entity.id); // You need to have access to key
                if(attachedEntity != null) {
                    // meaning that the entity is already present in the context, probably because of a previous get operation
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntity.EntityState = EntityState.Deleted; // Ecam.Framework.EntityState.Deleted;
                } else { // the entry with the same key (entity.id) was not found in the context
                    entry.State = System.Data.Entity.EntityState.Deleted; // This should attach entity
                }
            }

            if(!delaySave) {
                SaveChanges();
            }

            //using (EcamContext context = new EcamContext()) {
            //EcamContext context = ContextFactory.Instance;
            //T entity = Activator.CreateInstance(typeof (T)) as T;
            //entity.id = id;
            ////Approach 1
            //var obj = context.Entry(entity);
            //if (obj.State == System.Data.EntityState.Detached) {
            //    context.Set(typeof (T)).Attach(obj.Entity);
            //}
            //context.Set(typeof (T)).Remove(obj.Entity);
            //// Approach 2
            //context.Entry(entity).State = System.Data.EntityState.Deleted;
            //if (!delaySave) {
            //    SaveChanges();
            //}
        }

        public void Destroy<T>(T obj,bool delaySave = false) where T:BaseEntity {
            EcamContext _context = ContextFactory.Instance;
            Type entityType = typeof(T).GetGenericArguments()[0];
            var entry = _context.Entry<T>(obj);
            if(entry.State == System.Data.Entity.EntityState.Detached) {
                _context.Set(typeof(T)).Attach(entry.Entity);
            }
            //_context.Set(typeof (T)).Remove(entry.Entity);
            // typeof(T) returns BaseEntity<T>. The context doesnt know about BaseEntity<T>, so it will 
            // throw the following exception
            // The entity type BaseEntity`1 is not part of the model for the current context.
            // Correct code:
            _context.Set(entityType).Remove(entry.Entity);

            if(!delaySave) {
                SaveChanges();
            }
        }

        public void SaveChanges() {
            try {
                EcamContext context = ContextFactory.Instance;
                context.SaveChanges();
            } catch(System.Data.Entity.Infrastructure.DbUpdateException updateEx) {
                string errorMsg = "DbUpdateException Error=" + updateEx.Message + Environment.NewLine;
                if(updateEx.InnerException != null) {
                    if(updateEx.InnerException.InnerException.Message.Contains("Cannot delete")) {
                        this.Errors.Add("","Cann't Delete! Child records found!");
                    } else {
                        this.Errors.Add(string.Empty,updateEx.InnerException.InnerException.Message);
                    }
                    errorMsg += updateEx.InnerException.InnerException.Message + Environment.NewLine;
                } else {
                    this.Errors.Add(string.Empty,updateEx.Message);
                }
                Helper.DBErrorLog("Error =" + errorMsg);
            } catch(System.Data.Entity.Validation.DbEntityValidationException ex) {
                foreach(DbEntityValidationResult validationResult in ex.EntityValidationErrors) {
                    StringBuilder sb = new StringBuilder();
                    foreach(var validationError in validationResult.ValidationErrors) {
                        sb.AppendLine(string.Format("{0}:{1}",validationError.PropertyName,
                                                    validationError.ErrorMessage));
                    }
                    this.Errors.Add(string.Empty,sb.ToString());
                    Helper.DBErrorLog("DbEntityValidationException Error=" + sb.ToString());
                }
            } catch(Exception ex) {
                string errorMsg = "Exception Error=" + ex.Message + Environment.NewLine;
                if(ex.InnerException != null) {
                    errorMsg += ex.InnerException.InnerException.Message + Environment.NewLine;
                }
                Helper.DBErrorLog("Exception Error=" + errorMsg);
            }
        }
    }

    public class ContextFactory {
        //[ThreadStatic] private static EcamContext _context = null;

        public static EcamContext Instance {
            get {
                EcamContext _context = HttpContextFactory.GetHttpContext().Items["Context"] as EcamContext;
                if(_context == null) {
                    _context = new EcamContext();
                    HttpContextFactory.GetHttpContext().Items.Add("Context",_context);
                }
                return _context;
            }
        }

        public static void Dispose() {
            if(Instance != null) {
                Instance.Dispose();
            }
        }
    }
}

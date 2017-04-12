using Ecam.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ecam.Models {
	public class BaseEntity {

		public BaseEntity() {
			this._errorProvider = new ErrorProvider();
		}

		protected static IServiceFactory _factory = null;
		public static void ConfigureServiceFactory(IServiceFactory factory) {
			_factory = factory;
		}

		public int id { get; set; }

		public DateTime? created_date { get; set; }
		public int? created_by { get; set; }
		public DateTime? last_updated_date { get; set; }
		public int? last_updated_by { get; set; }

		private EntityState _entityState = EntityState.UnChanged;

		[NotMapped]
		public EntityState EntityState {
			get { return _entityState; }
			set { _entityState = value; }
		}

		private IErrorProvider _errorProvider;

		[NotMapped]
		public IErrorProvider Errors {
			get { return _errorProvider; } //set { _errorProvider = value; } 
		}

	}

	[Serializable]
	public enum EntityState {
		Added = 1,
		Deleted = 2,
		Modified = 3,
		UnChanged = 4,
		Destroy = 5
	}

	public class BaseEntity<T> : BaseEntity where T : BaseEntity {
		protected bool IsValid = true;

		private static IService<T> _service = null;
		public static IService<T> Service {
			get {
				_service = _factory.Create<T>();
				return _service;
			}
			//set { _service = value; }
			set { _factory.Register(value); }
		}

		public virtual bool Validate() {
			// return true;            
			IEnumerable<ErrorInfo> errors = ValidationHelper.Validate(this);
			foreach (var errorInfo in errors) {
				this.Errors.Add(errorInfo);
			}
			return errors.Count() <= 0;
		}

        public virtual bool ValidateChecking() {
            OnSaving();
            return this.Validate();
        }

		public virtual void OnSaving() {
			// Set the EntityID
			if (this.id > 0) {
				this.EntityState = EntityState.Modified;
			} else {
				this.EntityState = EntityState.Added;
			}
		}



		public virtual void Save() {            
			OnSaving();
            //switch (this.EntityState) {
            //    case EntityState.Added:
            //        this.Create();
            //        break;
            //    case EntityState.Modified:
            //        this.Update();
            //        break;
            //    case EntityState.Deleted:
            //        this.Delete();
            //        break;
            //}
            //if (this.id > 0)            
            if (this.EntityState == EntityState.Modified) {
				IsValid = this.Validate();
				if (IsValid) {
					OnUpdating();
					Update();
					OnUpdated();
                    OnSaved();
				}
			} else {                
				IsValid = this.Validate();
				if (IsValid) {
					OnCreating();
					Create();
					OnCreated();
                    OnSaved();
				}
			}
		}

		public virtual void OnSaved() {
		}

		/// <summary>
		/// Inserts the entity into the database.
		/// </summary>
		public virtual void OnCreating() {
			this.created_by = Authentication.CurrentUserID;
			this.created_date = DateTime.Now.ToUniversalTime();
		}

		public virtual void OnCreated() {
		}

		/// <summary>
		/// Inserts the entity into the database.
		/// </summary>
		/// <param name="performValidation">
		/// True: perform validation.
		/// <para>
		/// False: do not perform validation.
		/// </para>
		/// </param>
		//public virtual void OnCreate(bool performValidation) {
		//    //For Validation - the entity state must be set to use some custom validation
		//    this.EntityState = EntityState.Added;
		//    if (performValidation) {
		//        IsValid = this.Validate(ValidationRuleSets.Create);
		//    }
		//}

		/// <summary>
		/// Updates the entity in the database.
		/// </summary>
		public virtual void OnUpdating() {
			this.last_updated_by = Authentication.CurrentUserID;
			this.last_updated_date = DateTime.Now.ToUniversalTime();
		}

		public virtual void OnUpdated() {

		}

		/// <summary>
		/// Updates the entity in the database.
		/// </summary>
		/// <param name="performValidation">
		/// True: perform validation.
		/// <para>
		/// False: do not perform validation.
		/// </para>
		/// </param>
		//public virtual void OnUpdate(bool performValidation) {
		//    this.EntityState = EntityState.Modified;
		//    if (performValidation) {
		//        IsValid = this.Validate(ValidationRuleSets.Update);
		//    }
		//}

		protected virtual void Create() {
			Service.Create((T)((object)this));
		}

		protected virtual void Update() {
			Service.Update((T)((object)this));
		}

		public virtual void OnDeleting() {
		}

		public virtual void Delete() {
			OnDeleting();
            if(this.Errors.Errors.Any() == false) {
                //Repository.Delete(this.id);
                OnServiceDelete();
                if(this.Errors.Errors.Any() == false) {
                    OnDeleted();
                }
            }
		}

        public virtual void OnServiceDelete() {
            Service.Delete(this);
        }

		public virtual void OnDeleted() {
		}

		public virtual void OnDestroying() {
		}

		protected virtual void Destroy() {
			OnDestroying();
			Service.Delete(this.id);
			OnDestroyed();
		}

		public virtual void OnDestroyed() {
		}

		#region EntityFactory methods
		public static IQueryable<T> GetById(int? id) {
			if (id.HasValue) {
				return GetById(id.Value);
			} else {
				return All();
			}
		}

		public static IQueryable<T> GetById(int id) {
			return Service.GetById(id);
		}

		public static IQueryable<T> GetByIds(List<int> ids) {
			return Service.GetByIds(ids);
		}

		/// <summary>
		/// Use this method if you know that the Entity doesnt have EntityID. The only difference from All is that Query doesnt filter on EntityID
		/// </summary>
		/// <returns></returns>
		public static IQueryable<T> Query() {
			return Service.All();
		}

		public static IQueryable<T> All() {
			return Service.All();
		}

		public static IQueryable<T> Where(T criteria) {
			return All().Where(criteria);
		}
		#endregion
	}

	public interface IEntityFactory<T> where T : BaseEntity<T> {
		IQueryable<T> GetById(int? id);
		IQueryable<T> GetById(int id);
		IQueryable<T> GetByIds(List<int> ids);
		IQueryable<T> Query();
		IQueryable<T> All(bool entityFilter = true);
		IQueryable<T> Where(T criteria);
		T Instance { get; }
	}


	public static class EntityFactory<T> where T : BaseEntity<T> {
		public static IQueryable<T> GetById(int? id) {
			return BaseEntity<T>.GetById(id);
		}

		public static IQueryable<T> GetById(int id) {
			return BaseEntity<T>.GetById(id);
		}

		public static IQueryable<T> GetByIds(List<int> ids) {
			return BaseEntity<T>.GetByIds(ids);
		}

		/// <summary>
		/// Use this method if you know that the Entity doesnt have EntityID. The only difference from All is that Query doesnt filter on EntityID
		/// </summary>
		/// <returns></returns>
		public static IQueryable<T> Query() {
			return BaseEntity<T>.Query();
		}

		public static IQueryable<T> All() {
			return BaseEntity<T>.All();
		}

		public static IQueryable<T> Where(T criteria) {
			return BaseEntity<T>.Where(criteria);
		}
	}

	public interface IService<T> where T : BaseEntity {
		IQueryable<T> GetById(int id);
		IQueryable<T> GetByIds(List<int> ids);
		IQueryable<T> All();

		T Create(T obj, bool delaySave = false);
		bool Update(T obj, bool delaySave = false);
		void Delete(int id, bool delaySave = false);
		void Delete<T>(T obj, bool delaySave = false) where T : BaseEntity;
		void Destroy(int id, bool delaySave = false);
		void Destroy<T>(T obj, bool delaySave = false) where T : BaseEntity;
		void SaveChanges();
	}

	public interface IServiceFactory {
		IService<T> Create<T>() where T : BaseEntity;
		void Register<T>(IService<T> service) where T : BaseEntity;
	}
}
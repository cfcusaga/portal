using System;
using System.Data.Entity;
using System.Threading.Tasks;
using cfcusaga.data;

namespace cfcusaga.domain.Membership
{
    public class MembershipServices:IMembershipService, IDisposable
    {
        private readonly PortalDbContext _db;
        private bool _disposed = false;
        public MembershipServices(PortalDbContext db)
        {
            _db = db;
        }

        public async Task<Member> GetMemberInfoFromAspNetUserId(string currentUserId)
        {
            var entity = await _db.Members.FirstOrDefaultAsync(c => c.AspNetUserId == currentUserId);
            if (entity != null)
            {
                var aMember = new Member()
                {
                    Id = entity.Id,
                    LastName = entity.LastName,
                    Firstname = entity.FirstName,
                    Gender = entity.Gender
                };
                return aMember;
            }
            return null;
        }
        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (this._disposed) return;
            // If disposing equals true, dispose all managed 
            // and unmanaged resources. 
            if (disposing)
            {
                _db?.Dispose();
            }

            // Note disposing has been done.
            _disposed = true;
        }
    }
}